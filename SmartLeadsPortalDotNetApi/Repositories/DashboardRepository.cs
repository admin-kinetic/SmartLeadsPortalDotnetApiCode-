using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class DashboardRepository
    {
        private readonly DbConnectionFactory dbConnectionFactory;
        public DashboardRepository(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<DashboardTotalModel> GetDashboardUrgentTaskTotal()
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spDashboardUrgentTaskTotal";
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
        public async Task<DashboardTotalModel> GetDashboardHighTaskTotal()
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spDashboardHighTaskTotal";
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
        public async Task<DashboardTotalModel> GetDashboardLowTaskTotal()
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spDashboardLowTaskTotal";
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
        public async Task<DashboardTotalModel> GetDashboardPastDueTaskTotal()
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spDashboardPastDueTaskTotal";
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

        public async Task<DashboardTotalModel> GetDashboardProspectTotal()
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spGetSmartLeadsProspectCount";
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

        public async Task<IEnumerable<DashboardTodoTaskDue>> GetDashboardTodoTaskDue()
        {
            try
            {
                IEnumerable<DashboardTodoTaskDue> list = new List<DashboardTodoTaskDue>();
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var proc = "sm_spDashboardTodoTaskDue";
                    list = await connection.QueryAsync<DashboardTodoTaskDue>(proc, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<IEnumerable<SmartLeadsCallTasks>> GetDashboardUrgentTaskList()
        {
            try
            {
                IEnumerable<SmartLeadsCallTasks> list = new List<SmartLeadsCallTasks>();
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var proc = "sm_spDashboardUrgentTaskList";
                    list = await connection.QueryAsync<SmartLeadsCallTasks>(proc, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
    }
}
