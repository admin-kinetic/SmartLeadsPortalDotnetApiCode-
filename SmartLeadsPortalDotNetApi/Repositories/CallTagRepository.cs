using Dapper;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class CallTagRepository : SQLDBService
    {
        private readonly string _connectionString;
        public CallTagRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQLServerDBConnectionString");
        }
        public async Task<int> InsertCallTags(CallTagsInsert keyword)
        {
            try
            {
                string _proc = "sm_spInsertCallTags";
                var param = new DynamicParameters();
                param.Add("@tagname", keyword.TagName);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<int> UpdateCallTags(CallTags keyword)
        {
            try
            {
                string _proc = "sm_spUpdateCallTags";
                var param = new DynamicParameters();
                param.Add("@guid", keyword.GuId);
                param.Add("@tagname", keyword.TagName);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task<CallTagsResponseModel<CallTags>> GetAllCallTagsList(ExcludedKeywordsListRequest model)
        {
            try
            {
                string _proc = "";
                var count = 0;
                var param = new DynamicParameters();
                var param2 = new DynamicParameters();
                IEnumerable<CallTags> list = new List<CallTags>();

                _proc = "sm_spGetAllCallTagsList";
                param.Add("@PageNumber", model.Page);
                param.Add("@PageSize", model.PageSize);
                param.Add("@Search", model.Search);

                list = await SqlMapper.QueryAsync<CallTags>(con, _proc, param, commandType: CommandType.StoredProcedure);

                var countProcedure = "sm_spGetAllCallTagsListCount";
                param2.Add("@Search", model.Search);
                count = await con.QueryFirstOrDefaultAsync<int>(countProcedure, param2, commandType: CommandType.StoredProcedure);

                return new CallTagsResponseModel<CallTags>
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

        public async Task<CallTags?> GetCallTagsById(Guid guid)
        {
            try
            {
                string _proc = "sm_spGetCallTagsById";
                var param = new DynamicParameters();
                param.Add("@guid", guid);
                CallTags? list = await SqlMapper.QuerySingleOrDefaultAsync<CallTags>(con, _proc, param, commandType: CommandType.StoredProcedure);

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
        public async Task<int> DeleteCallTags(Guid guid)
        {
            try
            {
                string _proc = "sm_spDeleteCallTags";
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
