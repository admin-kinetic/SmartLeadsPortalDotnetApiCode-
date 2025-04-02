using Dapper;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class CallStateRepository : SQLDBService
    {
        private readonly string _connectionString;
        public CallStateRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQLServerDBConnectionString");
        }
        public async Task<int> InsertCallState(CallStateInsert keyword)
        {
            try
            {
                string _proc = "sm_spInsertCallState";
                var param = new DynamicParameters();
                param.Add("@callstate", keyword.StateName);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<int> UpdateCallState(CallState keyword)
        {
            try
            {
                string _proc = "sm_spUpdateCallState";
                var param = new DynamicParameters();
                param.Add("@guid", keyword.GuId);
                param.Add("@callstate", keyword.StateName);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task<CallStateResponseModel<CallState>> GetAllCallStateList(ExcludedKeywordsListRequest model)
        {
            try
            {
                string _proc = "";
                var count = 0;
                var param = new DynamicParameters();
                var param2 = new DynamicParameters();
                IEnumerable<CallState> list = new List<CallState>();

                _proc = "sm_spGetAllCallStateList";
                param.Add("@PageNumber", model.Page);
                param.Add("@PageSize", model.PageSize);
                param.Add("@Search", model.Search);

                list = await SqlMapper.QueryAsync<CallState>(con, _proc, param, commandType: CommandType.StoredProcedure);

                var countProcedure = "sm_spGetAllCallStateListCount";
                param2.Add("@Search", model.Search);
                count = await con.QueryFirstOrDefaultAsync<int>(countProcedure, param2, commandType: CommandType.StoredProcedure);

                return new CallStateResponseModel<CallState>
                {
                    Items = list.ToList(),
                    Total = count
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Dispose();
            }
        }

        public async Task<CallState?> GetCallStateById(Guid guid)
        {
            try
            {
                string _proc = "sm_spGetCallStateById";
                var param = new DynamicParameters();
                param.Add("@guid", guid);
                CallState? list = await SqlMapper.QuerySingleOrDefaultAsync<CallState>(con, _proc, param, commandType: CommandType.StoredProcedure);

                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Dispose();
            }
        }
        public async Task<int> DeleteCallState(Guid guid)
        {
            try
            {
                string _proc = "sm_spDeleteCallState";
                var param = new DynamicParameters();
                param.Add("@guid", guid);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
    }
}
