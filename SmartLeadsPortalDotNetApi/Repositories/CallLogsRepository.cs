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
            var filename = "https://files-test.kineticstaff.com/callrecordings";
            // Add VOIP service API to get duration, call datetime
            if (string.IsNullOrEmpty(keyword.UserPhoneNumber) || string.IsNullOrEmpty(keyword.ProspectNumber))
            {
                throw new ArgumentException("UserPhoneNumber, UserCaller, and ProspectNumber cannot be null or empty.");
            }

            CallLogsOutbound? callLogsOutbound = await GetOutboundcallsInfo(keyword.UserPhoneNumber, keyword.ProspectNumber);

            if (callLogsOutbound == null)
            {
                throw new InvalidOperationException("Failed to retrieve outbound call information.");
            }

            try
            {
                string _proc = "sm_spInsertCallLogs";
                var param = new DynamicParameters();
                param.Add("@usercaller", keyword.UserCaller);
                param.Add("@userphonenumber", callLogsOutbound.CallerId);
                param.Add("@leademail", keyword.LeadEmail);
                param.Add("@prospectname", keyword.ProspectName);
                param.Add("@prospectnumber", callLogsOutbound.DestNumber);
                param.Add("@callpurposeid", keyword.CallPurposeId);
                param.Add("@calldispositionid", keyword.CallDispositionId);
                param.Add("@calldirectionid", keyword.CallDirectionId);
                param.Add("@notes", keyword.Notes);
                param.Add("@calltagsid", keyword.CallTagsId);
                param.Add("@callstateid", keyword.CallStateId);
                param.Add("@duration", callLogsOutbound.ConversationDuration);
                param.Add("@addedby", keyword.AddedBy);
                param.Add("@statisticid", keyword.StatisticId);
                param.Add("@due", keyword.Due);
                param.Add("@userid", keyword.UserId);
                param.Add("@uniquecallid", callLogsOutbound.UniqueCallId);
                param.Add("@calleddate", callLogsOutbound.CallStartAt);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                if (!string.IsNullOrEmpty(callLogsOutbound.UniqueCallId))
                {
                    filename = filename+"/"+callLogsOutbound.UniqueCallId+".mp3";
                    await UpdateOutboundCallsInfo(callLogsOutbound.UniqueCallId, filename);
                }

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

        public async Task<CallLogsOutbound?> GetOutboundcallsInfo(string callerid, string destnumber)
        {
            try
            {
                string _proc = "sm_spGetOutboundCallsByCallLogs";
                var param = new DynamicParameters();
                param.Add("@callerid", callerid);
                param.Add("@destnumber", destnumber);
                CallLogsOutbound? result = await SqlMapper.QuerySingleOrDefaultAsync<CallLogsOutbound>(con, _proc, param, commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task UpdateOutboundCallsInfo(string uniquecallid, string filename)
        {
            try
            {
                string _proc = "sm_spUpdateOutboundCallInfo";
                var param = new DynamicParameters();
                param.Add("@uniquecallid", uniquecallid);
                param.Add("@filename", filename);

                await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure); ;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

    }
}
