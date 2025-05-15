using System;
using System.Transactions;
using Dapper;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using SmartLeadsAllLeadsToPortal.Entities;
using Common.Services;
using Common.Database;
using Microsoft.Extensions.Configuration;

namespace SmartLeadsAllLeadsToPortal;

public class SmartLeadAllLeadsService
{
    private readonly DbConnectionFactory dbConnectionFactory;
    private readonly SmartLeadHttpService smartLeadHttpService;
    private readonly IConfigurationRoot configuration;

    public SmartLeadAllLeadsService(DbConnectionFactory dbConnectionFactory, SmartLeadHttpService smartLeadHttpService, IConfigurationRoot configuration)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.smartLeadHttpService = smartLeadHttpService;
        this.configuration = configuration;
    }

    public async Task SaveAllLeads()
    {
        var daysOffset = int.TryParse(this.configuration["DaysOffset"], out var days) ? days : 1;
        var date =  DateTime.Now.AddDays(-daysOffset);
        var limit = 100;
        var offset = 100;
        var hasMore = false;
        do
        {
            var allLeads = await this.smartLeadHttpService.FetchAllLeadsFromEntireAccount(date, offset, limit);
            
            hasMore = allLeads.hasMore;

            var smartLeadAllLeads = allLeads.data.Select(lead => new SmartLeadAllLeads
            {
                LeadId = int.Parse(lead.id),
                Email = lead.email,
                CampaignId = lead.campaigns.First()?.campaign_id,
                FirstName = lead.first_name,
                LastName = lead.last_name,
                CreatedAt = lead.created_at,
                PhoneNumber = lead.phone_number,
                CompanyName = lead.company_name,
                LeadStatus = lead.campaigns.First()?.lead_status,
                BDR  = lead.custom_fields.BDR ?? string.Empty,
                CreatedBy =lead.custom_fields.Created_by ?? string.Empty,
                QABy = lead.custom_fields.QA_by ?? string.Empty
            });
            Console.WriteLine($"Saving another batch of {smartLeadAllLeads.Count()} leads");
            using (var connection = this.dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        //var insert = """
                        //    INSERT INTO SmartLeadAllLeads (LeadId, Email, CampaignId, FirstName, LastName, CreatedAt, PhoneNumber, CompanyName, LeadStatus, BDR, CreatedBy, QABy)
                        //    VALUES (@LeadId, @Email, @CampaignId, @FirstName, @LastName, @CreatedAt, @PhoneNumber, @CompanyName, @LeadStatus, @BDR, @CreatedBy, @QABy)
                        //""";

                        var upsert = """
                            MERGE INTO SmartLeadAllLeads AS target
                            USING (SELECT 
                                      @LeadId AS LeadId,
                                      @Email AS Email,
                                      @CampaignId AS CampaignId,
                                      @FirstName AS FirstName,
                                      @LastName AS LastName,
                                      @CreatedAt AS CreatedAt,
                                      @PhoneNumber AS PhoneNumber,
                                      @CompanyName AS CompanyName,
                                      @LeadStatus AS LeadStatus,
                                      @BDR AS BDR,
                                      @CreatedBy AS CreatedBy,
                                      @QABy AS QABy) AS source
                            ON (target.LeadId = source.LeadId)
                            WHEN MATCHED THEN
                                UPDATE SET
                                    Email = source.Email,
                                    CampaignId = source.CampaignId,
                                    FirstName = source.FirstName,
                                    LastName = source.LastName,
                                    CreatedAt = source.CreatedAt,
                                    PhoneNumber = source.PhoneNumber,
                                    CompanyName = source.CompanyName,
                                    LeadStatus = source.LeadStatus,
                                    BDR = source.BDR,
                                    CreatedBy = source.CreatedBy,
                                    QABy = source.QABy
                            WHEN NOT MATCHED THEN
                                INSERT (LeadId, Email, CampaignId, FirstName, LastName, CreatedAt, PhoneNumber, CompanyName, LeadStatus, BDR, CreatedBy, QABy)
                                VALUES (source.LeadId, source.Email, source.CampaignId, source.FirstName, source.LastName, source.CreatedAt, 
                                        source.PhoneNumber, source.CompanyName, source.LeadStatus, source.BDR, source.CreatedBy, source.QABy);
                            """;

                        await connection.ExecuteAsync(upsert, smartLeadAllLeads, transaction);
                        Console.WriteLine($"Inserted/Updated {smartLeadAllLeads.Count()} records into SmartLeadAllLeads table.");
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine("Rollback transaction: " + ex.Message);
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            offset += limit;
        } while (hasMore);


    }

    public async Task SaveAllLeadsByEmail()
    {
        var date = DateTime.Now.AddDays(-1);
        var limit = 100;
        var offset = 100;
        var hasMore = false;
        var smartLeadAllLeads = new List<SmartLeadAllLeads>();

        do
        {
            // var email = "lizanne.kindler@chicos.com";
            var emails = new List<string> {
                "resumes@balon.com",
                "aaron.medema@notionsmarketing.com",
                "lgibbons@247hotels.com",
                "tony@peraltaengineering.com",
                "gretchen.meyer@aventiv.com",
                "zbeutler@horsepowerbrands.com",
                "struckenbrod@horsepowerbrands.com",
                "trevor.john-gregory@sagroup.co.uk",
                "priya.adhikari@phsa.ca",
                "ryan.toncheff@gvhomes.com",
                "scontino@opalfuels.com",
                "khiguchi@queens.org",
                "john@scscsudbury.ca",
                "ashok.muthu@msalabs.com",
                "compliance@propertyreceivables.com",
                "stan.farnsworth@pulseforge.com",
                "njohnson@redriverst.com",
                "cpomorski@fti-net.com",
                "julius@maricann.com",
                "shennelly@propio-ls.com",
                "thomas.kristie@ace.aaa.com",
                "paul.wilson@alberici.com",
                "EdanT@jfrog.com",
                "rwong@wac.net",
                "nathan@diaconia.com",
                "lmarkson@storecapital.com",
                "lee.ericson@husqvarnagroup.com",
                "john@rycorhvac.com",
                "rkooch@investdavenport.com",
                "melissa.camp@progressiveus.com",
                "Charlie@RenfertUSA.com",
                "jbrooks@nnogc.com",
                "rueban.vignarajah@tngoc.com",
                "takayoshi_oshima@alliedtelesyn.com",
                "wbogumil@rwcatskills.com",
                "kbirk@toddsservices.com",
                "julie.washington@trinity-health.org",
                "todd.leombruno@parker.com",
                "bobr@baumgardrose.cpa",
                "info@envirotrac.com",
                "rick.gradoni@armtecdefense.com",
                "erika_zakrevsky@exportpackers.com",
                "lgrays@jlschwieters.com",
                "clayton.buckingham@fraserhealth.ca",
                "harley@getbetterbookkeeping.com",
                "joe.whelan@richlandindustries.com",
                "gerhard@danhard.com",
                "ppasko@stbarnabashealthsystem.com",
                "william.davenport@aimhosp.com",
                "smoorehead@cleaverbrooks.com",
                "sean.hildebrand@quility.com",
                "lminier@bchmed.org",
                "rfabiano@uaudio.com",
                "scott.ashton@summitde.com",
                "slong@kraftcpas.com",
                "matthew.ball@inserocpa.com",
                "dan.irwin@midwaytrucks.com",
                "mark.eubanks@brinks.com",
                "devin.morris@mwpetroleum.com",
                "shanson@klielawoffices.com",
                "alicavoli@jasint.com",
            };

            foreach (var email in emails)
            {
                await Task.Delay(1000);
                Console.WriteLine($"Fetching leads for email: {email}");
                var allLeads = await this.smartLeadHttpService.FetchAllLeadsFromEntireAccountByEmail(email);
                hasMore = allLeads.hasMore;

                smartLeadAllLeads.AddRange(allLeads.data.Select(lead => new SmartLeadAllLeads
                {
                    LeadId = int.Parse(lead.id),
                    Email = lead.email,
                    CampaignId = lead.campaigns.First()?.campaign_id,
                    FirstName = lead.first_name,
                    LastName = lead.last_name,
                    CreatedAt = lead.created_at,
                    PhoneNumber = lead.phone_number,
                    CompanyName = lead.company_name,
                    LeadStatus = lead.campaigns.First()?.lead_status
                }));
            }

            using (var connection = this.dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var insert = """
                            INSERT INTO SmartLeadAllLeads (LeadId, Email, CampaignId, FirstName, LastName, CreatedAt, PhoneNumber, CompanyName, LeadStatus)
                            VALUES (@LeadId, @Email, @CampaignId, @FirstName, @LastName, @CreatedAt, @PhoneNumber, @CompanyName, @LeadStatus)
                        """;

                        await connection.ExecuteAsync(insert, smartLeadAllLeads, transaction);

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
        } while (hasMore);
    }
}
