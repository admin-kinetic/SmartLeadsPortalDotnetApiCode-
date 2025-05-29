using Dapper;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class CallPurposeRepository : SQLDBService
    {
        public async Task<int> InsertCallPurpose(CallPurposeInsert keyword)
        {
            try
            {
                string _proc = "sm_spInsertCallPurpose";
                var param = new DynamicParameters();
                param.Add("@callpurpose", keyword.CallPurposeName);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<int> UpdateCallPurpose(CallPurpose keyword)
        {
            try
            {
                string _proc = "sm_spUpdateCallPurpose";
                var param = new DynamicParameters();
                param.Add("@guid", keyword.GuId);
                param.Add("@callpurpose", keyword.CallPurposeName);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task<CallPurposeResponseModel<CallPurpose>> GetAllCallPurposeList(ExcludedKeywordsListRequest model)
        {
            try
            {
                string _proc = "";
                var count = 0;
                var param = new DynamicParameters();
                var param2 = new DynamicParameters();
                IEnumerable<CallPurpose> list = new List<CallPurpose>();

                _proc = "sm_spGetAllCallPurposeList";
                param.Add("@PageNumber", model.Page);
                param.Add("@PageSize", model.PageSize);
                param.Add("@Search", model.Search);

                list = await SqlMapper.QueryAsync<CallPurpose>(con, _proc, param, commandType: CommandType.StoredProcedure);

                var countProcedure = "sm_spGetAllCallPurposeListCount";
                param2.Add("@Search", model.Search);
                count = await con.QueryFirstOrDefaultAsync<int>(countProcedure, param2, commandType: CommandType.StoredProcedure);

                return new CallPurposeResponseModel<CallPurpose>
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
        public async Task<IEnumerable<CallPurpose>> GetCallPurposeRetrievedAll()
        {
            try
            {
                string _proc = "";
                var param = new DynamicParameters();
                IEnumerable<CallPurpose> list = new List<CallPurpose>();

                _proc = "sm_spGetCallPurposeRetrieveAll";

                list = await SqlMapper.QueryAsync<CallPurpose>(con, _proc, commandType: CommandType.StoredProcedure);
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
        public async Task<CallPurpose?> GetCallPurposeById(Guid guid)
        {
            try
            {
                string _proc = "sm_spGetCallPurposeById";
                var param = new DynamicParameters();
                param.Add("@guid", guid);
                CallPurpose? list = await SqlMapper.QuerySingleOrDefaultAsync<CallPurpose>(con, _proc, param, commandType: CommandType.StoredProcedure);

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
        public async Task<int> DeleteCallPurpose(Guid guid)
        {
            try
            {
                string _proc = "sm_spDeleteCallPurpose";
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
