using System;
using Common.Models;
using Common.Services;
using ImportSmartLeadStatistics.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using Common.Database;

namespace ImportSmartLeadStatistics;

public class SmartLeadCampaignStatisticsService
{
    private readonly DbConnectionFactory connectionFactory;
    private readonly SmartLeadHttpService smartLeadHttpService;
    private readonly IConfiguration configuration;

    public SmartLeadCampaignStatisticsService(DbConnectionFactory connectionFactory, SmartLeadHttpService smartLeadHttpService, IConfiguration configuration)
    {
        this.connectionFactory = connectionFactory;
        this.smartLeadHttpService = smartLeadHttpService;
        this.configuration = configuration;
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
                    var offset = 0;
                    var limit = 100;
                    do
                    {
                        var statistics = await this.smartLeadHttpService.FetchCampaignStatisticsByCampaignId(campaign.id.Value, apiKey, offset, limit, daysOffset);
                        hasData = statistics.data.Count > 0;

                        if (hasData == false)
                        {
                            continue;
                        }

                        Console.WriteLine($"Campaign ID: {campaign.id.Value}, Offset: {offset}, Limit: {limit}, Has Data: {hasData}");

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

                                    Console.WriteLine("Successfully saved campaign statistics.");
                                }
                                catch (System.Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
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

            using (var connection = this.connectionFactory.CreateConnection())
            {
                var campaignsConfig = this.configuration.GetSection("Campaigns").Get<List<int>>();
                var excludeCampaigns = this.configuration.GetSection("ExcludeCampaigns").Get<List<int>>();
                var campaigns = await this.smartLeadHttpService.ListAllCampaigns(apiKey);
                if (campaignsConfig != null && campaignsConfig.Any())
                {
                    campaigns = campaigns.Where(c => campaignsConfig.Contains(c.id.Value)).ToList();
                }

                if (excludeCampaigns!= null && excludeCampaigns.Any())
                {
                    campaigns = campaigns.Where(c => excludeCampaigns.Contains(c.id.Value) == false).ToList();
                }
               
                foreach (var campaign in campaigns.Where(s => s.status == "ACTIVE" || s.status == "COMPLETED"))
                {
                    var offset = 0;
                    var limit = 100;
                    do
                    {
                        Console.WriteLine($"Fetching for Campaign ID: {campaign.id.Value}, Offset: {offset}, Limit: {limit}, Day Offset: {daysOffset}");
                        var statistics = await this.smartLeadHttpService.FetchCampaignLeadStatistics(campaign.id.Value, apiKey, offset, limit, daysOffset);
                        hasMore = statistics.hasMore;

                        if (!hasMore)
                        {
                            Console.WriteLine($"End for Campaign ID: {campaign.id.Value}, Offset: {offset}, Limit: {limit}, Has Data: {hasMore}");
                            break;
                        }

                        Console.WriteLine($"Campaign ID: {campaign.id.Value}, Offset: {offset}, Limit: {limit}, Has Data: {hasMore}");

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
                                            EmailSubject = historyItem.subject,
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
                                            ReplyTime = historyItem.time
                                        };

                                        smartLeadCampaignStatisticsForEmailReply.Add(entry);
                                    }
                                }
                            }

                            if (connection.State != System.Data.ConnectionState.Open)
                            {
                                connection.Open();
                            }

                            using (var transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    if (smartLeadCampaignStatisticsForEmailSent.Count > 0)
                                    {
                                        var upsertForEmailSent = """
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
                                            upsertForEmailSent,
                                            smartLeadCampaignStatisticsForEmailSent.Select(cs => new
                                            {
                                                GuId = Guid.NewGuid(),
                                                cs.LeadId,
                                                cs.LeadEmail,
                                                cs.SequenceNumber,
                                                EmailSubject = cs.EmailSubject?.Length > 500 ? cs.EmailSubject.Substring(0, 500) : cs.EmailSubject,
                                                cs.SentTime,
                                                cs.OpenCount,
                                                cs.ClickCount
                                            }),
                                            transaction);
                                    }

                                    if (smartLeadCampaignStatisticsForEmailReply.Count > 0)
                                    {


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
                                            smartLeadCampaignStatisticsForEmailReply.Select(cs => new
                                            {
                                                GuId = Guid.NewGuid(),
                                                cs.LeadId,
                                                cs.LeadEmail,
                                                cs.SequenceNumber,
                                                cs.ReplyTime
                                            }),
                                            transaction);
                                    }

                                    transaction.Commit();

                                    Console.WriteLine("Successfully saved campaign statistics.");
                                }
                                catch (System.Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }

                        offset += limit;
                    } while (hasMore);
                }
            }
        }
    }
}
