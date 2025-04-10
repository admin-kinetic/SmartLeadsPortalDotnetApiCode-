using Dapper;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;
using System.Drawing;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class CallLogsRepository : SQLDBService
    {
        public async Task<int> InsertCallLogs(CallsInsert keyword)
        {
            //Add VOIP service API to get duration, call datetime

            try
            {
                string _proc = "sm_spInsertCallLogs";
                var param = new DynamicParameters();
                param.Add("@usercaller", keyword.UserCaller);
                param.Add("@userphonenumber", keyword.UserPhoneNumber);
                param.Add("@leademail", keyword.LeadEmail);
                param.Add("@prospectname", keyword.ProspectName);
                param.Add("@prospectnumber", keyword.ProspectNumber);
                param.Add("@callpurposeid", keyword.CallPurposeId);
                param.Add("@calldispositionid", keyword.CallDispositionId);
                param.Add("@calldirectionid", keyword.CallDirectionId);
                param.Add("@notes", keyword.Notes);
                param.Add("@calltagsid", keyword.CallTagsId);
                param.Add("@callstateid", keyword.CallStateId);
                param.Add("@addedby", keyword.AddedBy);
                param.Add("@statisticid", keyword.StatisticId);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
            finally
            {
                con.Dispose();
            }
        }
        public async Task<CallLogLeadNo?> GetleadContactNoByEmail(string email)
        {
            try
            {
                string _proc = "sm_spGetLeadProspectPhoneNumber";
                var param = new DynamicParameters();
                param.Add("@email", email);
                CallLogLeadNo? list = await SqlMapper.QuerySingleOrDefaultAsync<CallLogLeadNo>(leadcon, _proc, param, commandType: CommandType.StoredProcedure);

                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                leadcon.Dispose();
            }
        }
        public async Task<int> UpdateCallLogs(CallsUpdate request)
        {
            try
            {
                string _proc = "sm_spUpdateCallLogs";
                var param = new DynamicParameters();
                param.Add("@guid", request.Guid);
                param.Add("@callpurposeid", request.CallPurposeId);
                param.Add("@calldispositionid", request.CallDispositionId);
                param.Add("@calltagsid", request.CallTagsId);
                param.Add("@notes", request.Notes);

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
