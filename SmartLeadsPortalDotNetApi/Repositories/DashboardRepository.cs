using Dapper;
using Microsoft.Graph.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using System.Data;
using System.Threading;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class DashboardRepository
    {
        private readonly DbConnectionFactory dbConnectionFactory;
        private readonly ILogger<DashboardRepository> logger;

        public DashboardRepository(DbConnectionFactory dbConnectionFactory, ILogger<DashboardRepository> logger)
        {
            this.dbConnectionFactory = dbConnectionFactory;
            this.logger = logger;
        }
        public async Task<DashboardTotalModel> GetDashboardUrgentTaskTotal(CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spDashboardUrgentTaskTotal";
                    var command = new CommandDefinition(countProcedure, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    var count = await connection.QueryFirstOrDefaultAsync<DashboardTotalModel>(command);

                    if (count == null)
                    {
                        count = new DashboardTotalModel { Total = 0 };
                    }

                    return count;
                }
            }
            catch (OperationCanceledException ex)
            {
                this.logger.LogInformation($"Operation Cancelled: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<DashboardTotalModel> GetDashboardHighTaskTotal(CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spDashboardHighTaskTotal";
                    var command = new CommandDefinition(countProcedure, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    var count = await connection.QueryFirstOrDefaultAsync<DashboardTotalModel>(command);

                    if (count == null)
                    {
                        count = new DashboardTotalModel { Total = 0 };
                    }

                    return count;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<DashboardTotalModel> GetDashboardLowTaskTotal(CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spDashboardLowTaskTotal";
                    var command = new CommandDefinition(countProcedure, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    var count = await connection.QueryFirstOrDefaultAsync<DashboardTotalModel>(command);

                    if (count == null)
                    {
                        count = new DashboardTotalModel { Total = 0 };
                    }

                    return count;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<DashboardTotalModel> GetDashboardPastDueTaskTotal(CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spDashboardPastDueTaskTotal";
                    var command = new CommandDefinition(countProcedure, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    var count = await connection.QueryFirstOrDefaultAsync<DashboardTotalModel>(command);

                    if (count == null)
                    {
                        count = new DashboardTotalModel { Total = 0 };
                    }

                    return count;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task<DashboardTotalModel> GetDashboardProspectTotal(CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetSmartLeadsProspectCount";
                    var command = new CommandDefinition(countProcedure, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    var count = await connection.QueryFirstOrDefaultAsync<DashboardTotalModel>(countProcedure, commandType: CommandType.StoredProcedure);

                    if (count == null)
                    {
                        count = new DashboardTotalModel { Total = 0 };
                    }

                    return count;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task<IEnumerable<DashboardTodoTaskDue>> GetDashboardTodoTaskDue(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<DashboardTodoTaskDue> list = new List<DashboardTodoTaskDue>();
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var proc = "sm_spDashboardTodoTaskDue";
                    var command = new CommandDefinition(proc, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    list = await connection.QueryAsync<DashboardTodoTaskDue>(command);

                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<IEnumerable<SmartLeadsCallTasks>> GetDashboardUrgentTaskList(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<SmartLeadsCallTasks> list = new List<SmartLeadsCallTasks>();
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var proc = "sm_spDashboardUrgentTaskList";
                    var command = new CommandDefinition(proc, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    list = await connection.QueryAsync<SmartLeadsCallTasks>(command);

                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        //New Dashboard
        public async Task<IEnumerable<DashboardEmailCampaignModel>> GetDashboardEmailCampaignBDR(DashboardDateParameter request)
        {
            try
            {
                IEnumerable<DashboardEmailCampaignModel> list = new List<DashboardEmailCampaignModel>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spDashboardEmailCampaignBDR";
                    var param = new DynamicParameters();
                    param.Add("@startDate", request.StartDate);
                    param.Add("@endDate", request.EndDate);
                    list = await connection.QueryAsync<DashboardEmailCampaignModel>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<SmartLeadAllLeads>> GetDashboardEmailCampaignBdrForCsv(DashboardDateParameter request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string query = """
                        SELECT *
                        FROM 
                            [dbo].[SmartLeadAllLeads]
                        WHERE BDR IS NOT NULL
                            AND CreatedAt >= @startDate 
                            AND CreatedAt < DATEADD(DAY, 1, @endDate)
                    """;
                    var param = new DynamicParameters();
                    param.Add("@startDate", request.StartDate);
                    param.Add("@endDate", request.EndDate);
                    var queryResult = await connection.QueryAsync<SmartLeadAllLeads>(query, param);
                    return queryResult.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<DashboardEmailCampaignModel>> GetDashboardEmailCampaignCreatedBy(DashboardDateParameter request)
        {
            try
            {
                IEnumerable<DashboardEmailCampaignModel> list = new List<DashboardEmailCampaignModel>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spDashboardEmailCampaignCreatedBy";
                    var param = new DynamicParameters();
                    param.Add("@startDate", request.StartDate);
                    param.Add("@endDate", request.EndDate);
                    list = await connection.QueryAsync<DashboardEmailCampaignModel>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<DashboardEmailCampaignModel>> GetDashboardEmailCampaignQaBy(DashboardDateParameter request)
        {
            try
            {
                IEnumerable<DashboardEmailCampaignModel> list = new List<DashboardEmailCampaignModel>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spDashboardEmailCampaignQaBy";
                    var param = new DynamicParameters();
                    param.Add("@startDate", request.StartDate);
                    param.Add("@endDate", request.EndDate);
                    list = await connection.QueryAsync<DashboardEmailCampaignModel>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<DashboardSmartLeadCampaignsActive>> GetDashboardSmartLeadCampaignsActive()
        {
            IEnumerable<DashboardSmartLeadCampaignsActive> list = new List<DashboardSmartLeadCampaignsActive>();

            using (var connection = this.dbConnectionFactory.GetSqlConnection())
            {
                string _proc = "sm_spSmartLeadCampaignsActive";
                list = await connection.QueryAsync<DashboardSmartLeadCampaignsActive>(_proc, commandType: CommandType.StoredProcedure);

                return list;
            }
        }
        
        public async Task<IEnumerable<DashboardAutomatedCampaignLeadgen>> GetDashboardJobAdChartsEmailSequenceLeadgen(DashboardDateParameter request)
        {
            try
            {
                IEnumerable<DashboardAutomatedCampaignLeadgen> list = new List<DashboardAutomatedCampaignLeadgen>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spDashboardAutomatedCampaignLeadgen";
                    var param = new DynamicParameters();
                    param.Add("@startDate", request.StartDate);
                    param.Add("@endDate", request.EndDate);
                    list = await connection.QueryAsync<DashboardAutomatedCampaignLeadgen>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<DashboardAutomatedCampaign>> GetDashboardJobAdChartsEmailSequenceFullyAutomated(DashboardDateParameter request)
        {
            try
            {
                IEnumerable<DashboardAutomatedCampaign> list = new List<DashboardAutomatedCampaign>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spDashboardAutomatedCampaignEmailSequence";
                    var param = new DynamicParameters();
                    param.Add("@startDate", request.StartDate);
                    param.Add("@endDate", request.EndDate);
                    list = await connection.QueryAsync<DashboardAutomatedCampaign>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<DashboardEmailStatistics?> GetDashboardEmailStatisticsTotalSent(DashboardDateParameter request)
        {
            try
            {
                var campaignList = await GetDashboardSmartLeadCampaignsActive();
                var campaignIdsJson = JsonConvert.SerializeObject(campaignList.Select(c => c.Id));

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetSmartLeadsEmailStatisticsSent";

                    var param = new DynamicParameters();
                    param.Add("@startDate", request.StartDate);
                    param.Add("@endDate", request.EndDate);
                    param.Add("@campaignIds", campaignIdsJson);

                    DashboardEmailStatistics? list = await connection.QuerySingleOrDefaultAsync<DashboardEmailStatistics?>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<DashboardDropdownList>> GetDashboardBDRList(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<DashboardDropdownList> list = new List<DashboardDropdownList>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var proc = "sm_spGetDashboardBDRList";
                    var command = new CommandDefinition(proc, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    list = await connection.QueryAsync<DashboardDropdownList>(command);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<DashboardDropdownList>> GetDashboardCampaignsList(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<DashboardDropdownList> list = new List<DashboardDropdownList>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var proc = "sm_spGetDashboardCampaignsList";
                    var command = new CommandDefinition(proc, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    list = await connection.QueryAsync<DashboardDropdownList>(command);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<DashboardDropdownList>> GetDashboardLeadgenList(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<DashboardDropdownList> list = new List<DashboardDropdownList>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var proc = "sm_spGetDashboardLeadgenList";
                    var command = new CommandDefinition(proc, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    list = await connection.QueryAsync<DashboardDropdownList>(command);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<DashboardDropdownList>> GetDashboardQaList(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<DashboardDropdownList> list = new List<DashboardDropdownList>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var proc = "sm_spGetDashboardQaList";
                    var command = new CommandDefinition(proc, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
                    list = await connection.QueryAsync<DashboardDropdownList>(command);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //Smartleads Analytics Dashboard
        public async Task<DashboardAnalyticsTotalSent> GetDashboardSmartLeadAnalyticsTotalSent(DashboardFilterModel filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter.Bdr) || filter.Bdr == "null")
                {
                    filter.Bdr = "";
                }

                if (string.IsNullOrEmpty(filter.CreatedBy) || filter.CreatedBy == "null")
                {
                    filter.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(filter.QaBy) || filter.QaBy == "null")
                {
                    filter.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetDashboardSmartLeadAnalyticsTotalSent";
                    var param = new DynamicParameters();
                    param.Add("@startDate", filter.StartDate);
                    param.Add("@endDate", filter.EndDate);
                    param.Add("@bdr", filter.Bdr);
                    param.Add("@createdby", filter.CreatedBy);
                    param.Add("@qaby", filter.QaBy);
                    param.Add("@campaignid", filter.CampaignId);
                    var countResult = await connection.QueryFirstOrDefaultAsync<int?>(countProcedure, param, commandType: CommandType.StoredProcedure);

                    return new DashboardAnalyticsTotalSent { TotalSent = countResult };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<DashboardAnalyticsTotalReplied> GetDashboardSmartLeadAnalyticsTotalReply(DashboardFilterModel filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter.Bdr) || filter.Bdr == "null")
                {
                    filter.Bdr = "";
                }

                if (string.IsNullOrEmpty(filter.CreatedBy) || filter.CreatedBy == "null")
                {
                    filter.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(filter.QaBy) || filter.QaBy == "null")
                {
                    filter.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetDashboardSmartLeadAnalyticsTotalReply";
                    var param = new DynamicParameters();
                    param.Add("@startDate", filter.StartDate);
                    param.Add("@endDate", filter.EndDate);
                    param.Add("@bdr", filter.Bdr);
                    param.Add("@createdby", filter.CreatedBy);
                    param.Add("@qaby", filter.QaBy);
                    param.Add("@campaignid", filter.CampaignId);
                    var countResult = await connection.QueryFirstOrDefaultAsync<int?>(countProcedure, param, commandType: CommandType.StoredProcedure);

                    return new DashboardAnalyticsTotalReplied { TotalReplied = countResult };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<DashboardAnalyticsTotalOpened> GetDashboardSmartLeadAnalyticsTotalOpened(DashboardFilterModel filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter.Bdr) || filter.Bdr == "null")
                {
                    filter.Bdr = "";
                }

                if (string.IsNullOrEmpty(filter.CreatedBy) || filter.CreatedBy == "null")
                {
                    filter.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(filter.QaBy) || filter.QaBy == "null")
                {
                    filter.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetDashboardSmartLeadAnalyticsTotalOpened";
                    var param = new DynamicParameters();
                    param.Add("@startDate", filter.StartDate);
                    param.Add("@endDate", filter.EndDate);
                    param.Add("@bdr", filter.Bdr);
                    param.Add("@createdby", filter.CreatedBy);
                    param.Add("@qaby", filter.QaBy);
                    param.Add("@campaignid", filter.CampaignId);
                    var countResult = await connection.QueryFirstOrDefaultAsync<int?>(countProcedure, param, commandType: CommandType.StoredProcedure);

                    return new DashboardAnalyticsTotalOpened { TotalOpened = countResult };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<DashboardAnalyticsTotalUniqueOpened> GetDashboardSmartLeadAnalyticsTotalUniqueOpened(DashboardFilterModel filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter.Bdr) || filter.Bdr == "null")
                {
                    filter.Bdr = "";
                }

                if (string.IsNullOrEmpty(filter.CreatedBy) || filter.CreatedBy == "null")
                {
                    filter.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(filter.QaBy) || filter.QaBy == "null")
                {
                    filter.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetDashboardSmartLeadAnalyticsTotalUniqueOpened";
                    var param = new DynamicParameters();
                    param.Add("@startDate", filter.StartDate);
                    param.Add("@endDate", filter.EndDate);
                    param.Add("@bdr", filter.Bdr);
                    param.Add("@createdby", filter.CreatedBy);
                    param.Add("@qaby", filter.QaBy);
                    param.Add("@campaignid", filter.CampaignId);
                    var countResult = await connection.QueryFirstOrDefaultAsync<int?>(countProcedure, param, commandType: CommandType.StoredProcedure);

                    return new DashboardAnalyticsTotalUniqueOpened { TotalUniqueOpened = countResult };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<DashboardAnalyticsTotalBounced> GetDashboardSmartLeadAnalyticsTotalBounced(DashboardFilterModel filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter.Bdr) || filter.Bdr == "null")
                {
                    filter.Bdr = "";
                }

                if (string.IsNullOrEmpty(filter.CreatedBy) || filter.CreatedBy == "null")
                {
                    filter.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(filter.QaBy) || filter.QaBy == "null")
                {
                    filter.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetDashboardSmartLeadAnalyticsTotalBounced";
                    var param = new DynamicParameters();
                    param.Add("@startDate", filter.StartDate);
                    param.Add("@endDate", filter.EndDate);
                    param.Add("@bdr", filter.Bdr);
                    param.Add("@createdby", filter.CreatedBy);
                    param.Add("@qaby", filter.QaBy);
                    param.Add("@campaignid", filter.CampaignId);
                    var countResult = await connection.QueryFirstOrDefaultAsync<int?>(countProcedure, param, commandType: CommandType.StoredProcedure);

                    return new DashboardAnalyticsTotalBounced { TotalBounced = countResult };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<DashboardAnalyticsTotalInterested> GetDashboardSmartLeadAnalyticsTotalInterested(DashboardFilterModel filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter.Bdr) || filter.Bdr == "null")
                {
                    filter.Bdr = "";
                }

                if (string.IsNullOrEmpty(filter.CreatedBy) || filter.CreatedBy == "null")
                {
                    filter.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(filter.QaBy) || filter.QaBy == "null")
                {
                    filter.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetDashboardSmartLeadAnalyticsTotalInterested";
                    var param = new DynamicParameters();
                    param.Add("@startDate", filter.StartDate);
                    param.Add("@endDate", filter.EndDate);
                    param.Add("@bdr", filter.Bdr);
                    param.Add("@createdby", filter.CreatedBy);
                    param.Add("@qaby", filter.QaBy);
                    param.Add("@campaignid", filter.CampaignId);
                    var countResult = await connection.QueryFirstOrDefaultAsync<int?>(countProcedure, param, commandType: CommandType.StoredProcedure);

                    return new DashboardAnalyticsTotalInterested { TotalInterested = countResult };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<DashboardAnalyticsTotalOutOfOffice> GetDashboardSmartLeadAnalyticsTotalOutOfOffice(DashboardFilterModel filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter.Bdr) || filter.Bdr == "null")
                {
                    filter.Bdr = "";
                }

                if (string.IsNullOrEmpty(filter.CreatedBy) || filter.CreatedBy == "null")
                {
                    filter.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(filter.QaBy) || filter.QaBy == "null")
                {
                    filter.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetDashboardSmartLeadAnalyticsTotalOutOfOffice";
                    var param = new DynamicParameters();
                    param.Add("@startDate", filter.StartDate);
                    param.Add("@endDate", filter.EndDate);
                    param.Add("@bdr", filter.Bdr);
                    param.Add("@createdby", filter.CreatedBy);
                    param.Add("@qaby", filter.QaBy);
                    param.Add("@campaignid", filter.CampaignId);
                    var countResult = await connection.QueryFirstOrDefaultAsync<int?>(countProcedure, param, commandType: CommandType.StoredProcedure);

                    return new DashboardAnalyticsTotalOutOfOffice { TotalOutOfOffice = countResult };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<DashboardAnalyticsTotalIncorrectContact> GetDashboardSmartLeadAnalyticsTotalIncorrectContact(DashboardFilterModel filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter.Bdr) || filter.Bdr == "null")
                {
                    filter.Bdr = "";
                }

                if (string.IsNullOrEmpty(filter.CreatedBy) || filter.CreatedBy == "null")
                {
                    filter.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(filter.QaBy) || filter.QaBy == "null")
                {
                    filter.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetDashboardSmartLeadAnalyticsTotalIncorrectContact";
                    var param = new DynamicParameters();
                    param.Add("@startDate", filter.StartDate);
                    param.Add("@endDate", filter.EndDate);
                    param.Add("@bdr", filter.Bdr);
                    param.Add("@createdby", filter.CreatedBy);
                    param.Add("@qaby", filter.QaBy);
                    param.Add("@campaignid", filter.CampaignId);
                    var countResult = await connection.QueryFirstOrDefaultAsync<int?>(countProcedure, param, commandType: CommandType.StoredProcedure);

                    return new DashboardAnalyticsTotalIncorrectContact { TotalIncorrectContact = countResult };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        //Smartleads Email Campaign BDR, Created by, QA'd By
        public async Task<IEnumerable<DashboardEmailCampaignModel>> GetDashboardEmailCampaignBDRChart(DashboardFilterModel request)
        {
            try
            {
                IEnumerable<DashboardEmailCampaignModel> list = new List<DashboardEmailCampaignModel>();
                if (string.IsNullOrEmpty(request.Bdr) || request.Bdr == "null")
                {
                    request.Bdr = "";
                }

                if (string.IsNullOrEmpty(request.CreatedBy) || request.CreatedBy == "null")
                {
                    request.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(request.QaBy) || request.QaBy == "null")
                {
                    request.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spDashboardEmailCampaignBdrChart";
                    var param = new DynamicParameters();
                    param.Add("@startDate", request.StartDate);
                    param.Add("@endDate", request.EndDate);
                    param.Add("@bdr", request.Bdr);
                    param.Add("@createdby", request.CreatedBy);
                    param.Add("@qaby", request.QaBy);
                    param.Add("@campaignid", request.CampaignId);
                    list = await connection.QueryAsync<DashboardEmailCampaignModel>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<DashboardEmailCampaignModel>> GetDashboardEmailCampaignCreatedByChart(DashboardFilterModel request)
        {
            try
            {
                IEnumerable<DashboardEmailCampaignModel> list = new List<DashboardEmailCampaignModel>();
                if (string.IsNullOrEmpty(request.Bdr) || request.Bdr == "null")
                {
                    request.Bdr = "";
                }

                if (string.IsNullOrEmpty(request.CreatedBy) || request.CreatedBy == "null")
                {
                    request.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(request.QaBy) || request.QaBy == "null")
                {
                    request.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spDashboardEmailCampaignCreatedByChart";
                    var param = new DynamicParameters();
                    param.Add("@startDate", request.StartDate);
                    param.Add("@endDate", request.EndDate);
                    param.Add("@bdr", request.Bdr);
                    param.Add("@createdby", request.CreatedBy);
                    param.Add("@qaby", request.QaBy);
                    param.Add("@campaignid", request.CampaignId);
                    list = await connection.QueryAsync<DashboardEmailCampaignModel>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<DashboardEmailCampaignModel>> GetDashboardEmailCampaignQaByChart(DashboardFilterModel request)
        {
            try
            {
                IEnumerable<DashboardEmailCampaignModel> list = new List<DashboardEmailCampaignModel>();
                if (string.IsNullOrEmpty(request.Bdr) || request.Bdr == "null")
                {
                    request.Bdr = "";
                }

                if (string.IsNullOrEmpty(request.CreatedBy) || request.CreatedBy == "null")
                {
                    request.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(request.QaBy) || request.QaBy == "null")
                {
                    request.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spDashboardEmailCampaignQaByChart";
                    var param = new DynamicParameters();
                    param.Add("@startDate", request.StartDate);
                    param.Add("@endDate", request.EndDate);
                    param.Add("@bdr", request.Bdr);
                    param.Add("@createdby", request.CreatedBy);
                    param.Add("@qaby", request.QaBy);
                    param.Add("@campaignid", request.CampaignId);
                    list = await connection.QueryAsync<DashboardEmailCampaignModel>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //Smartleads Email Statistics Exported
        public async Task<DashboardAnalyticsTotalExported> GetDashboardSmartLeadAnalyticsTotalExported(DashboardFilterModel filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter.Bdr) || filter.Bdr == "null")
                {
                    filter.Bdr = "";
                }

                if (string.IsNullOrEmpty(filter.CreatedBy) || filter.CreatedBy == "null")
                {
                    filter.CreatedBy = "";
                }

                if (string.IsNullOrEmpty(filter.QaBy) || filter.QaBy == "null")
                {
                    filter.QaBy = "";
                }

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetDashboardSmartLeadStatisticsTotalExported";
                    var param = new DynamicParameters();
                    param.Add("@startDate", filter.StartDate);
                    param.Add("@endDate", filter.EndDate);
                    param.Add("@bdr", filter.Bdr);
                    param.Add("@createdby", filter.CreatedBy);
                    param.Add("@qaby", filter.QaBy);
                    param.Add("@campaignid", filter.CampaignId);
                    var countResult = await connection.QueryFirstOrDefaultAsync<int?>(countProcedure, param, commandType: CommandType.StoredProcedure);

                    return new DashboardAnalyticsTotalExported { TotalExported = countResult };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
    }
}
