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

    public SmartLeadCampaignStatisticsService(DbConnectionFactory connectionFactory, SmartLeadHttpService smartLeadHttpService, IConfiguration  configuration)
    {
        this.connectionFactory = connectionFactory;
        this.smartLeadHttpService = smartLeadHttpService;
        this.configuration = configuration;
    }

    public async Task SaveAllSmartLeadCampaignStatistics(){
        using (var connection = this.connectionFactory.CreateConnection())
        {
            var campaignsConfig = this.configuration.GetSection("Campaigns").Get<List<int>>();
            var campaigns = await this.smartLeadHttpService.ListAllCampaigns();
            campaigns = campaigns.Where(c => campaignsConfig.Contains(c.id.Value)).ToList();
            foreach (var campaign in campaigns.Where(s => s.status == "ACTIVE"))
            {
                var hasData = false;
                var offset = 0;
                var limit = 100;
                do
                {
                    var statistics = await this.smartLeadHttpService.FetchCampaignStatisticsByCampaignId(campaign.id.Value, offset, limit);
                    hasData = statistics.data.Count > 0;
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
                                    smartLeadCampaignStatistics.Select(cs => new {
                                        GuId = Guid.NewGuid(),
                                        cs.LeadEmail,
                                        cs.SequenceNumber,
                                        EmailSubject = cs.EmailSubject.Length > 500 ? cs.EmailSubject.Substring(0, 500) : cs.EmailSubject,
                                        cs.SentTime,
                                        cs.OpenTime,
                                        cs.ClickTime,
                                        cs.ReplyTime,
                                        cs.OpenCount,
                                        cs.ClickCount
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
