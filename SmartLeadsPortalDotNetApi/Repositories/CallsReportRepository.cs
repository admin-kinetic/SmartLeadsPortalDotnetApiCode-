using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class CallsReportRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<SmartLeadsAllLeadsRepository> logger;
        public CallsReportRepository(DbConnectionFactory dbConnectionFactory, ILogger<SmartLeadsAllLeadsRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            this.logger = logger;
        }
        public async Task<IEnumerable<CallsReport>> GetAllCalls(CallsParam request)
        {
            try
            {
                IEnumerable<CallsReport> list = new List<CallsReport>();

                using (var connection = this._dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spCallsReportPaginated";
                    var param = new DynamicParameters();
                    param.Add("@Type", request.Type);
                    param.Add("@StartDate", request.StartDate);
                    param.Add("@EndDate", request.EndDate);
                    param.Add("@Search", request.Search);
                    param.Add("@Bdr", request.Bdr);
                    param.Add("@PageNumber", request.Page);
                    param.Add("@PageSize", request.PageSize);

                    list = await connection.QueryAsync<CallsReport>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task<int> GetAllCallsCount(CallsParam filter)
        {
            try
            {
                using (var connection = this._dbConnectionFactory.GetSqlConnection())
                {
                    var countProcedure = "sm_spCallsReportPaginatedCount";
                    var param = new DynamicParameters();
                    param.Add("@Type", filter.Type);
                    param.Add("@StartDate", filter.StartDate);
                    param.Add("@EndDate", filter.EndDate);
                    param.Add("@Search", filter.Search);
                    param.Add("@Bdr", filter.Bdr);

                    int affectedRows = await connection.QuerySingleAsync<int>(countProcedure, param, commandType: CommandType.StoredProcedure);

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<IEnumerable<BdrDropdown>> GetBdrCalls()
        {
            try
            {
                IEnumerable<BdrDropdown> list = new List<BdrDropdown>();

                using (var connection = this._dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spCallsBdrDropdown";

                    list = await connection.QueryAsync<BdrDropdown>(_proc, commandType: CommandType.StoredProcedure);

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
