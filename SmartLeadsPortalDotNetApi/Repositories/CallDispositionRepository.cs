using Dapper;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class CallDispositionRepository :  SQLDBService
    {
        private readonly string _connectionString;
        public CallDispositionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQLServerDBConnectionString");
        }

        public async Task<int> InsertExcludedKeyword(CallDispositionInsert keyword)
        {
            try
            {
                string _proc = "";
                var param = new DynamicParameters();
                param.Add("@excludedkeywords", keyword.CallDispositionName);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<int> UpdateExcludedKeyword(CallDisposition keyword)
        {
            try
            {
                string _proc = "";
                var param = new DynamicParameters();
                param.Add("@guid", keyword.Guid);
                param.Add("@excludedkeywords", keyword.CallDispositionName);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task<CallDispositionResponseModel<CallDisposition>> GetAllExcludeKeywordsList(ExcludedKeywordsListRequest model)
        {
            try
            {
                string _proc = "";
                var count = 0;
                var param = new DynamicParameters();
                var param2 = new DynamicParameters();
                IEnumerable<CallDisposition> list = new List<CallDisposition>();

                _proc = "";
                param.Add("@PageNumber", model.Page);
                param.Add("@PageSize", model.PageSize);
                param.Add("@Search", model.Search);

                list = await SqlMapper.QueryAsync<CallDisposition>(con, _proc, param, commandType: CommandType.StoredProcedure);

                var countProcedure = "";
                param2.Add("@Search", model.Search);
                count = await con.QueryFirstOrDefaultAsync<int>(countProcedure, param2, commandType: CommandType.StoredProcedure);

                return new CallDispositionResponseModel<CallDisposition>
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

        public async Task<CallDisposition?> GetExcludeKeywordsById(Guid guid)
        {
            try
            {
                string _proc = "";
                var param = new DynamicParameters();
                param.Add("@guid", guid);
                CallDisposition? list = await SqlMapper.QuerySingleOrDefaultAsync<CallDisposition>(con, _proc, param, commandType: CommandType.StoredProcedure);

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
        public async Task<int> DeleteExcludedKeyword(Guid guid)
        {
            try
            {
                string _proc = "";
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
