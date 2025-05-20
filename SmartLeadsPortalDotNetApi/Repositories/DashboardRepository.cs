using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using System.Data;

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
            try
            {
                IEnumerable<DashboardSmartLeadCampaignsActive> list = new List<DashboardSmartLeadCampaignsActive>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spSmartLeadCampaignsActive";
                    list = await connection.QueryAsync<DashboardSmartLeadCampaignsActive>(_proc, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
    }
}
