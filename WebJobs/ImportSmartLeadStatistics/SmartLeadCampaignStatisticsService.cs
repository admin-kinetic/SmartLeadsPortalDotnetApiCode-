using System;
using Common.Services;
using ImportSmartLeadStatistics.Entities;
using Dapper;

namespace ImportSmartLeadStatistics.Services;

public class SmartLeadCampaignStatisticsService
{
    private readonly DbConnectionFactory connectionFactory;
    private readonly SmartLeadHttpService smartLeadHttpService;

    public SmartLeadCampaignStatisticsService(DbConnectionFactory connectionFactory, SmartLeadHttpService smartLeadHttpService)
    {
        this.connectionFactory = connectionFactory;
        this.smartLeadHttpService = smartLeadHttpService;
    }

    public async Task SaveAllSmartLeadCampaignStatistics(){
        using (var connection = this.connectionFactory.CreateConnection())
        {
            var campaigns = await this.smartLeadHttpService.ListAllCampaigns();
            foreach (var campaign in campaigns.Where(s => s.status == "ACTIVE"))
            {
                var hasData = false;
                var offset = 0;
                var limit = 100;
                do
                {
                    var statistics = await this.smartLeadHttpService.FetchCampaigncStatisticsByCampaignId(campaign.id, offset, limit);
                    hasData = statistics.data.Count > 0;

                    if (statistics.data.Count > 0)
                    {
                        var smartLeadCampaignStatistics = statistics.data.Select(statistic => new SmartLeadsEmailStatistics
                        {
                            LeadEmail = statistic.lead_email,
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
                                var insert = """
                                    INSERT INTO SmartLeadsEmailStatistics (GuId, LeadEmail, SequenceNumber, EmailSubject, SentTime, OpenTime, ClickTime, ReplyTime, OpenCount, ClickCount)
                                    VALUES (NEWID(), @LeadEmail, @SequenceNumber, @EmailSubject, @SentTime, @OpenTime, @ClickTime, @ReplyTime, @OpenCount, @ClickCount)
                                """;
                                await connection.ExecuteAsync(insert, smartLeadCampaignStatistics, transaction);
                                transaction.Commit();
                            }
                            catch (System.Exception ex)
                            {
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
