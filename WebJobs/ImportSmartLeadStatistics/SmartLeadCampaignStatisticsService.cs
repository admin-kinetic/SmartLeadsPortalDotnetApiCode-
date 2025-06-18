using Common.Database;
using Common.Entities;
using Common.Models;
using Common.Repositories;
using Common.Services;
using Dapper;
using ImportSmartLeadStatistics.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace ImportSmartLeadStatistics;

public class SmartLeadCampaignStatisticsService
{
    private readonly DbConnectionFactory connectionFactory;
    private readonly SmartLeadHttpService smartLeadHttpService;
    private readonly IConfiguration configuration;
    private readonly ILogger<SmartLeadCampaignStatisticsService> logger;

    public SmartLeadCampaignStatisticsService(
        DbConnectionFactory connectionFactory,
        SmartLeadHttpService smartLeadHttpService,
        IConfiguration configuration,
        ILogger<SmartLeadCampaignStatisticsService> logger)
    {
        this.connectionFactory = connectionFactory;
        this.smartLeadHttpService = smartLeadHttpService;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task SaveAllCampaignStatisticsByCampaignId()
    {
        var apiKeys = this.configuration.GetSection("ApiKeys").Get<List<string>>() ?? new List<string>();
        var daysOffset = this.configuration.GetSection("DaysOffset").Get<int?>() ?? 1;

        foreach (var apiKey in apiKeys)
        {
            using (var connection = this.connectionFactory.CreateConnection())
            {
                var campaignsConfig = this.configuration.GetSection("Campaigns").Get<List<int>>();
                var campaigns = await this.smartLeadHttpService.ListAllCampaigns(apiKey);
                //campaigns = campaigns.Where(c => campaignsConfig.Contains(c.id.Value)).ToList();
                foreach (var campaign in campaigns.Where(s => s.status == "ACTIVE"))
                {
                    var hasData = false;
                    var offset = 300;
                    var limit = 100;
                    do
                    {
                        var statistics = await this.smartLeadHttpService.FetchCampaignStatisticsByCampaignId(campaign.id.Value, apiKey, offset, limit, daysOffset);
                        hasData = statistics.data.Count > 0;

                        if (hasData == false)
                        {
                            continue;
                        }

                        this.logger.LogInformation($"Campaign ID: {campaign.id.Value}, Offset: {offset}, Limit: {limit}, Has Data: {hasData}");

                        if (statistics.data.Count > 0)
                        {
                            var smartLeadCampaignStatistics = statistics.data.Select(statistic => new SmartLeadsEmailStatistics
                            {
                                LeadEmail = statistic.lead_email,
                                LeadName = statistic.lead_name,
                                SequenceNumber = statistic.sequence_number,
                                EmailSubject = statistic.email_subject,
                                SentTime = statistic.sent_time,
                                OpenTime = statistic.open_time,
                                ClickTime = statistic.click_time,
                                ReplyTime = statistic.reply_time,
                                OpenCount = statistic.open_count,
                                ClickCount = statistic.click_count
                            });

                            if (connection.State != System.Data.ConnectionState.Open)
                            {
                                connection.Open();
                            }

                            using (var transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    var upsert = """
                                    MERGE INTO SmartLeadsEmailStatistics AS target
                                    USING (VALUES (@GuId, @LeadEmail, @LeadName, @SequenceNumber, @EmailSubject, @SentTime, @OpenTime, @ClickTime, @ReplyTime, @OpenCount, @ClickCount)) 
                                        AS source (GuId, LeadEmail, LeadName, SequenceNumber, EmailSubject, SentTime, OpenTime, ClickTime, ReplyTime, OpenCount, ClickCount)
                                    ON target.LeadEmail = source.LeadEmail AND target.SequenceNumber = source.SequenceNumber
                                    WHEN MATCHED THEN
                                        UPDATE SET 
                                            SequenceNumber = source.SequenceNumber,
                                            EmailSubject = source.EmailSubject,
                                            SentTime = source.SentTime,
                                            OpenTime = source.OpenTime,
                                            ClickTime = source.ClickTime,
                                            ReplyTime = source.ReplyTime,
                                            OpenCount = source.OpenCount,
                                            ClickCount = source.ClickCount
                                    WHEN NOT MATCHED THEN
                                        INSERT (GuId, LeadEmail, LeadName, SequenceNumber, EmailSubject, SentTime, OpenTime, ClickTime, ReplyTime, OpenCount, ClickCount)
                                        VALUES (source.GuId, source.LeadEmail, source.LeadName, source.SequenceNumber, source.EmailSubject, source.SentTime, source.OpenTime, source.ClickTime, source.ReplyTime, source.OpenCount, source.ClickCount);
                                """;
                                    await connection.ExecuteAsync(
                                        upsert,
                                        smartLeadCampaignStatistics.Select(cs => new
                                        {
                                            GuId = Guid.NewGuid(),
                                            cs.LeadEmail,
                                            cs.SequenceNumber,
                                            EmailSubject = cs.EmailSubject.Length > 500 ? cs.EmailSubject.Substring(0, 500) : cs.EmailSubject,
                                            cs.SentTime,
                                            cs.OpenTime,
                                            cs.ClickTime,
                                            cs.ReplyTime,
                                            cs.OpenCount,
                                            cs.ClickCount,
                                            cs.LeadName
                                        }),
                                        transaction);
                                    transaction.Commit();

                                    this.logger.LogInformation("Successfully saved campaign statistics.");
                                }
                                catch (System.Exception ex)
                                {
                                    this.logger.LogInformation(ex.Message);
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }

                        offset += limit;
                    } while (hasData);
                }
            }
        }
    }

    public async Task SaveAllCampaignLeadStatistics()
    {
        var apiKeys = this.configuration.GetSection("ApiKeys").Get<List<string>>() ?? new List<string>();
        var daysOffset = this.configuration.GetSection("DaysOffset").Get<int?>() ?? 1;

        foreach (var apiKey in apiKeys)
        {

            var hasMore = false;


            var campaignsConfig = this.configuration.GetSection("Campaigns").Get<List<int>>();
            var excludeCampaigns = this.configuration.GetSection("ExcludeCampaigns").Get<List<int>>();
            var campaigns = await this.smartLeadHttpService.ListAllCampaigns(apiKey);
            if (campaignsConfig != null && campaignsConfig.Any())
            {
                campaigns = campaigns.Where(c => campaignsConfig.Contains(c.id.Value)).ToList();
            }

            if (excludeCampaigns != null && excludeCampaigns.Any())
            {
                campaigns = campaigns.Where(c => excludeCampaigns.Contains(c.id.Value) == false).ToList();
            }

            foreach (var campaign in campaigns.Where(s => s.status == "ACTIVE" || s.status == "COMPLETED"))
            {
                var offset = 0;
                var limit = 100;
                do
                {
                    this.logger.LogInformation($"Fetching for Campaign ID: {campaign.id.Value}, Offset: {offset}, Limit: {limit}, Day Offset: {daysOffset}");

                    var statistics = await DbExecution.ExecuteWithRetryAsync(async () =>
                    {
                        return await this.smartLeadHttpService.FetchCampaignLeadStatistics(campaign.id.Value, apiKey, offset, limit, daysOffset);
                    });

                    hasMore = statistics.hasMore;

                    if (!hasMore && statistics.data.Count == 0)
                    {
                        this.logger.LogInformation($"End for Campaign ID: {campaign.id.Value}, Offset: {offset}, Limit: {limit}, Has Data: {hasMore}");
                        break;
                    }

                    this.logger.LogInformation($"Campaign ID: {campaign.id.Value}, Offset: {offset}, Limit: {limit}, Has Data: {hasMore}");

                    if (statistics.data.Count > 0)
                    {
                        //var smartLeadCampaignStatistics = statistics.data.Select(statistic => new SmartLeadsEmailStatistics
                        //{
                        //    LeadEmail = statistic.to,
                        //    SequenceNumber = int.Parse(statistic.history.First().email_seq_number),
                        //    EmailSubject = statistic.history.First().subject,
                        //    SentTime = statistic.history.First().time,
                        //    OpenCount = statistic.history.First().open_count,
                        //    ClickCount = statistic.history.First().click_count
                        //});

                        var smartLeadCampaignStatisticsForEmailSent = new List<SmartLeadsEmailStatistics>();
                        var smartLeadCampaignStatisticsForEmailReply = new List<SmartLeadsEmailStatistics>();

                        foreach (var statistic in statistics.data)
                        {
                            foreach (var historyItem in statistic.history)
                            {
                                if (historyItem.type == "SENT")
                                {
                                    var entry = new SmartLeadsEmailStatistics
                                    {
                                        LeadId = double.Parse(statistic.lead_id),
                                        LeadEmail = statistic.to,
                                        SequenceNumber = int.Parse(historyItem.email_seq_number),
                                        EmailMessageId = historyItem.message_id,
                                        EmailSubject = historyItem.subject,
                                        EmailBody = historyItem.email_body,
                                        SentTime = historyItem.time,
                                        OpenCount = historyItem.open_count,
                                        ClickCount = historyItem.click_count
                                    };

                                    smartLeadCampaignStatisticsForEmailSent.Add(entry);
                                }

                                if (historyItem.type == "REPLY")
                                {
                                    var entry = new SmartLeadsEmailStatistics
                                    {
                                        LeadId = double.Parse(statistic.lead_id),
                                        LeadEmail = statistic.to,
                                        SequenceNumber = int.Parse(historyItem.email_seq_number),
                                        EmailMessageId = historyItem.message_id,
                                        EmailSubject = historyItem.subject,
                                        EmailBody = historyItem.email_body,
                                        ReplyTime = historyItem.time
                                    };

                                    smartLeadCampaignStatisticsForEmailReply.Add(entry);
                                }
                            }
                        }

                        await UpsertForEmailSent(smartLeadCampaignStatisticsForEmailSent);
                        await UpsertForEmailReply(smartLeadCampaignStatisticsForEmailReply);
                    }

                    offset += limit;
                } while (hasMore);
            }

        }
    }

    private async Task UpsertForEmailReply(List<SmartLeadsEmailStatistics> smartLeadCampaignStatisticsForEmailReply)
    {
        using var connection = this.connectionFactory.CreateConnection();
        foreach (var emailReplyItem in smartLeadCampaignStatisticsForEmailReply)
        {
            try
            {
                var messageHistory = new MessageHistory
                {
                    StatsId = emailReplyItem.EmailMessageStatsId,
                    Type = "SENT",
                    MessageId = emailReplyItem.EmailMessageId,
                    Time = emailReplyItem.SentTime,
                    EmailBody = emailReplyItem.EmailBody,
                    Subject = emailReplyItem.EmailSubject,
                    EmailSequenceNumber = emailReplyItem.SequenceNumber,
                    OpenCount = emailReplyItem.OpenCount,
                    ClickCount = emailReplyItem.ClickCount,
                    LeadEmail = emailReplyItem.LeadEmail
                };

                var upsertMessageHistory = """
                    MERGE INTO MessageHistory WITH (ROWLOCK) AS target
                    USING (VALUES (
                        @StatsId, @Type, @MessageId, @Time, @EmailBody,
                        @Subject, @EmailSequenceNumber, @OpenCount, @ClickCount, @LeadEmail
                    )) AS source (
                        StatsId, Type, MessageId, Time, EmailBody,
                        Subject, EmailSequenceNumber, OpenCount, ClickCount, LeadEmail
                    )
                    ON target.MessageId = source.MessageId
                    WHEN MATCHED THEN
                        UPDATE SET
                            StatsId = source.StatsId,
                            Type = source.Type,
                            Time = source.Time,
                            EmailBody = source.EmailBody,
                            Subject = source.Subject,
                            EmailSequenceNumber = source.EmailSequenceNumber,
                            OpenCount = source.OpenCount,
                            ClickCount = source.ClickCount,
                            LeadEmail = source.LeadEmail

                    WHEN NOT MATCHED THEN
                        INSERT (
                            StatsId, Type, MessageId, Time, EmailBody,
                            Subject, EmailSequenceNumber, OpenCount, ClickCount, LeadEmail
                        ) VALUES (
                            @StatsId, @Type, @MessageId, @Time, @EmailBody,
                            @Subject, @EmailSequenceNumber, @OpenCount, @ClickCount, @LeadEmail
                        );
                 """;

                await connection.ExecuteAsync(upsertMessageHistory, messageHistory);

                var upsertForEmailReply = """
                MERGE INTO SmartLeadsEmailStatistics WITH (ROWLOCK) AS target
                USING (VALUES (@GuId, @LeadId, @LeadEmail, @SequenceNumber, @ReplyTime)) 
                    AS source (GuId, LeadId, LeadEmail, SequenceNumber, ReplyTime)
                ON target.LeadEmail = source.LeadEmail AND target.SequenceNumber = source.SequenceNumber
                WHEN MATCHED THEN
                    UPDATE SET 
                        LeadId = source.LeadId,
                        ReplyTime = source.ReplyTime
                WHEN NOT MATCHED THEN
                    INSERT (GuId, LeadId, LeadEmail, SequenceNumber, ReplyTime)
                    VALUES (source.GuId, source.LeadId,source.LeadEmail, source.SequenceNumber, source.ReplyTime);
            """;
                await connection.ExecuteAsync(
                    upsertForEmailReply,
                    new
                    {
                        GuId = Guid.NewGuid(),
                        emailReplyItem.LeadId,
                        emailReplyItem.LeadEmail,
                        emailReplyItem.SequenceNumber,
                        emailReplyItem.ReplyTime
                    });

            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                this.logger.LogInformation($"Error occured on {emailReplyItem.LeadEmail}", ex.Message);
                throw;
            }
        }
    }

    private async Task UpsertForEmailSent(List<SmartLeadsEmailStatistics> smartLeadCampaignStatisticsForEmailSent)
    {
        using var connection = this.connectionFactory.CreateConnection();
        foreach (var emailSentItem in smartLeadCampaignStatisticsForEmailSent)
        {
            try
            {
                var messageHistory = new MessageHistory
                {
                    StatsId = emailSentItem.EmailMessageStatsId,
                    Type = "SENT",
                    MessageId = emailSentItem.EmailMessageId,
                    Time = emailSentItem.SentTime,
                    EmailBody = emailSentItem.EmailBody,
                    Subject = emailSentItem.EmailSubject,
                    EmailSequenceNumber = emailSentItem.SequenceNumber,
                    OpenCount = emailSentItem.OpenCount,
                    ClickCount = emailSentItem.ClickCount,
                    LeadEmail = emailSentItem.LeadEmail
                };

                var upsertMessageHistory = """
                    MERGE INTO MessageHistory WITH (ROWLOCK) AS target
                    USING (VALUES (
                        @StatsId, @Type, @MessageId, @Time, @EmailBody,
                        @Subject, @EmailSequenceNumber, @OpenCount, @ClickCount, @LeadEmail
                    )) AS source (
                        StatsId, Type, MessageId, Time, EmailBody,
                        Subject, EmailSequenceNumber, OpenCount, ClickCount, LeadEmail
                    )
                    ON target.MessageId = source.MessageId
                    WHEN MATCHED THEN
                        UPDATE SET
                            StatsId = source.StatsId,
                            Type = source.Type,
                            Time = source.Time,
                            EmailBody = source.EmailBody,
                            Subject = source.Subject,
                            EmailSequenceNumber = source.EmailSequenceNumber,
                            OpenCount = source.OpenCount,
                            ClickCount = source.ClickCount,
                            LeadEmail = source.LeadEmail

                    WHEN NOT MATCHED THEN
                        INSERT (
                            StatsId, Type, MessageId, Time, EmailBody,
                            Subject, EmailSequenceNumber, OpenCount, ClickCount, LeadEmail
                        ) VALUES (
                            @StatsId, @Type, @MessageId, @Time, @EmailBody,
                            @Subject, @EmailSequenceNumber, @OpenCount, @ClickCount, @LeadEmail
                        );
                 """;

                await connection.ExecuteAsync(upsertMessageHistory, messageHistory);

                var upsertStatiscticsForEmailSent = """
                MERGE INTO SmartLeadsEmailStatistics WITH (ROWLOCK) AS target
                USING (VALUES (@GuId, @LeadId, @LeadEmail, @SequenceNumber, @EmailSubject, @SentTime, @OpenCount, @ClickCount)) 
                    AS source (GuId, LeadId, LeadEmail, SequenceNumber, EmailSubject, SentTime, OpenCount, ClickCount)
                ON target.LeadEmail = source.LeadEmail AND target.SequenceNumber = source.SequenceNumber
                WHEN MATCHED THEN
                    UPDATE SET 
                        LeadId = source.LeadId,
                        SequenceNumber = source.SequenceNumber,
                        EmailSubject = source.EmailSubject,
                        SentTime = source.SentTime,
                        OpenCount = source.OpenCount,
                        ClickCount = source.ClickCount
                WHEN NOT MATCHED THEN
                    INSERT (GuId, LeadId, LeadEmail, SequenceNumber, EmailSubject, SentTime, OpenCount, ClickCount)
                    VALUES (source.GuId, source.LeadId, source.LeadEmail, source.SequenceNumber, source.EmailSubject, source.SentTime, source.OpenCount, source.ClickCount);
            """;
                await connection.ExecuteAsync(
                    upsertStatiscticsForEmailSent,
                    new
                    {
                        GuId = Guid.NewGuid(),
                        emailSentItem.LeadId,
                        emailSentItem.LeadEmail,
                        emailSentItem.SequenceNumber,
                        EmailSubject = emailSentItem.EmailSubject?.Length > 500 ? emailSentItem.EmailSubject.Substring(0, 500) : emailSentItem.EmailSubject,
                        emailSentItem.SentTime,
                        emailSentItem.OpenCount,
                        emailSentItem.ClickCount
                    });
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                this.logger.LogInformation($"Error occured on {emailSentItem.LeadEmail}", ex.Message);
                throw;
            }
        }
    }
}
