using Dapper;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class CallDispositionRepository :  SQLDBService
    {
        public async Task<int> InsertCallDisposition(CallDispositionInsert keyword)
        {
            try
            {
                string _proc = "sm_spCallDisposition";
                var param = new DynamicParameters();
                param.Add("@calldisposition", keyword.CallDispositionName);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<int> UpdateCallDisposition(CallDisposition keyword)
        {
            try
            {
                string _proc = "sm_spUpdateCallDisposition";
                var param = new DynamicParameters();
                param.Add("@guid", keyword.Guid);
                param.Add("@calldisposition", keyword.CallDispositionName);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task<CallDispositionResponseModel<CallDisposition>> GetAllCallDispositionList(ExcludedKeywordsListRequest model)
        {
            try
            {
                string _proc = "";
                var count = 0;
                var param = new DynamicParameters();
                var param2 = new DynamicParameters();
                IEnumerable<CallDisposition> list = new List<CallDisposition>();

                _proc = "sm_spGetAllCallDispositionList";
                param.Add("@PageNumber", model.Page);
                param.Add("@PageSize", model.PageSize);
                param.Add("@Search", model.Search);

                list = await SqlMapper.QueryAsync<CallDisposition>(con, _proc, param, commandType: CommandType.StoredProcedure);

                var countProcedure = "sm_spGetAllCallDispositionListCount";
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
        public async Task<IEnumerable<CallDisposition>> GetCallDispositionRetrievedAll()
        {
            try
            {
                string _proc = "";
                var param = new DynamicParameters();
                IEnumerable<CallDisposition> list = new List<CallDisposition>();

                _proc = "sm_spGetCallDispositionRetrieveAll";

                list = await SqlMapper.QueryAsync<CallDisposition>(con, _proc, commandType: CommandType.StoredProcedure);
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
        public async Task<CallDisposition?> GetCallDispositionById(Guid guid)
        {
            try
            {
                string _proc = "sm_spGetCallDispositionById";
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
        public async Task<int> DeleteCallDisposition(Guid guid)
        {
            try
            {
                string _proc = "sm_spDeleteCallDisposition";
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
