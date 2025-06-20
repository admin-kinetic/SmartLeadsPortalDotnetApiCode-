using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;
using System.Text.RegularExpressions;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class CallLogsRepository : SQLDBService
    {
        private readonly DbConnectionFactory dbConnectionFactory;
        public CallLogsRepository(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<ApiResponse> InsertCallLogs(CallsInsert keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword.UserPhoneNumber) || string.IsNullOrEmpty(keyword.ProspectNumber))
                {
                    return new ApiResponse(false, "UserPhoneNumber and ProspectNumber cannot be null or empty.", "INVALID_INPUT");
                }

                CallLogsOutbound? callLogsOutbound;
                CallProspectNameEmail? callProspectNameEmail;
                string cleanUserPhoneNumber = Regex.Replace(keyword.UserPhoneNumber, @"[\s()-]", "");
                string cleanProspectNumber = Regex.Replace(keyword.ProspectNumber, @"[\s()-]", "");

                if (keyword.CallDirectionId == 1)
                {
                    callLogsOutbound = await GetInboundcallsInfo(cleanProspectNumber, cleanUserPhoneNumber);
                }
                else
                {
                    callLogsOutbound = await GetOutboundcallsInfo(cleanUserPhoneNumber, cleanProspectNumber);
                }

                if (callLogsOutbound == null)
                {
                    return new ApiResponse(false, "Failed to retrieve call information.", "CALL_INFO_NOT_FOUND");
                }

                callProspectNameEmail = keyword.ProspectName != null ? await GetProspectEmailNameById(keyword.ProspectName)  : new CallProspectNameEmail { Email = null, FullName = null };
                CallLogFullName? callername = await GetProspectNameByPhone(cleanUserPhoneNumber);

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spInsertCallLogs";
                    var param = new DynamicParameters();
                    param.Add("@usercaller", string.IsNullOrEmpty(keyword.UserCaller) ? callername?.FullName : keyword.UserCaller);
                    param.Add("@userphonenumber", callLogsOutbound.CallerId);
                    param.Add("@leademail", callProspectNameEmail?.Email);
                    param.Add("@prospectname", callProspectNameEmail?.FullName);
                    param.Add("@prospectnumber", callLogsOutbound?.DestNumber);
                    param.Add("@callpurposeid", keyword.CallPurposeId);
                    param.Add("@calldispositionid", keyword.CallDispositionId);
                    param.Add("@calldirectionid", keyword.CallDirectionId);
                    param.Add("@notes", keyword.Notes);
                    param.Add("@calltagsid", keyword.CallTagsId);
                    param.Add("@callstateid", keyword.CallStateId);
                    param.Add("@duration", callLogsOutbound?.ConversationDuration);
                    param.Add("@addedby", keyword.AddedBy);
                    param.Add("@statisticid", keyword.StatisticId);
                    param.Add("@due", keyword.Due);
                    param.Add("@userid", keyword.UserId);
                    param.Add("@uniquecallid", callLogsOutbound?.UniqueCallId);
                    param.Add("@calleddate", callLogsOutbound?.CallStartAt);

                    int rowsAffected = await connection.ExecuteAsync(_proc, param, commandType: CommandType.StoredProcedure);

                    if (rowsAffected > 0)
                    {
                        return new ApiResponse(true, "Call logs saved successfully.");
                    }
                    else
                    {
                        return new ApiResponse(false, "Failed to save call logs.", "DATABASE_OPERATION_FAILED");
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, $"Database error: {ex.Message}", "DATABASE_ERROR");
            }
        }
        public async Task<int> InsertInboundCallLogs(CallsInsertInbound keyword)
        {
            await Task.Delay(3000);
            if (string.IsNullOrEmpty(keyword.UserPhoneNumber) || string.IsNullOrEmpty(keyword.ProspectNumber))
            {
                throw new ArgumentException("UserPhoneNumber, UserCaller, and ProspectNumber cannot be null or empty.");
            }

            CallLogsOutbound? callLogsOutbound = await GetInboundcallsInfo(keyword.ProspectNumber, keyword.UserPhoneNumber);

            if (callLogsOutbound == null)
            {
                throw new InvalidOperationException("Failed to retrieve outbound call information.");
            }

            CallLogFullName? callername = await GetProspectNameByPhone(keyword.UserPhoneNumber);

            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spInsertCallLogsInbound";
                    var param = new DynamicParameters();
                    param.Add("@usercaller", string.IsNullOrEmpty(keyword.UserCaller) ? callername?.FullName : keyword.UserCaller);
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
                    param.Add("@userid", keyword.UserId);
                    param.Add("@uniquecallid", callLogsOutbound.UniqueCallId);
                    param.Add("@calleddate", callLogsOutbound.CallStartAt);

                    int ret = await connection.ExecuteAsync(_proc, param, commandType: CommandType.StoredProcedure);

                    return ret;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<CallLogLeadNo?> GetleadContactNoByEmail(string email)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetLeadProspectPhoneNumber";
                    var param = new DynamicParameters();
                    param.Add("@email", email);

                    var result = await connection.QuerySingleOrDefaultAsync<CallLogLeadNo>(_proc, param, commandType: CommandType.StoredProcedure);

                    return result ?? new CallLogLeadNo { phone = null };
                }
            }
            catch (Exception)
            {
                return new CallLogLeadNo { phone = null };
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
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetOutboundCallsByCallLogs";
                    var param = new DynamicParameters();
                    param.Add("@callerid", callerid);
                    param.Add("@destnumber", destnumber);
                    CallLogsOutbound? result = await connection.QuerySingleOrDefaultAsync<CallLogsOutbound>(_proc, param, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<CallLogsOutbound?> GetInboundcallsInfo(string callerid, string destnumber)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetInboundCallsByCallLogs";
                    var param = new DynamicParameters();
                    param.Add("@callerid", callerid);
                    param.Add("@destnumber", destnumber);
                    CallLogsOutbound? result = await connection.QuerySingleOrDefaultAsync<CallLogsOutbound>(_proc, param, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<CallProspectNameEmail?> GetProspectEmailNameById(string leadid)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetProspectEmailNameById";
                    var param = new DynamicParameters();
                    param.Add("@leadid", leadid);
                    var result = await connection.QuerySingleOrDefaultAsync<CallProspectNameEmail>(_proc, param, commandType: CommandType.StoredProcedure);

                    return result ?? new CallProspectNameEmail { Email=null, FullName = null };
                }
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
        public async Task<int> DeleteCallLogs(CallsUpdate request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spDeleteCallLogs";
                    var param = new DynamicParameters();
                    param.Add("@guid", request.Guid);

                    int ret = await connection.ExecuteAsync(_proc, param);

                    return ret;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<CallLogFullName?> GetEmployeeNameByPhonenumber(CallLogLeadNo request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetEmployeeNameByPhonenumber";
                    var param = new DynamicParameters();
                    param.Add("@phoneNumber", request.phone);

                    var result = await connection.QuerySingleOrDefaultAsync<CallLogFullName>(_proc, param, commandType: CommandType.StoredProcedure);

                    return result ?? new CallLogFullName { FullName = null };
                }
            }
            catch (Exception)
            {
                return new CallLogFullName { FullName = null };
            }
        }
        public async Task<CallLogFullName?> GetProspectNameByPhone(string phonenumber)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetProspectNameByPhoneNumber";
                    var param = new DynamicParameters();
                    param.Add("@phone", phonenumber);
                    var result = await connection.QuerySingleOrDefaultAsync<CallLogFullName>(_proc, param, commandType: CommandType.StoredProcedure);

                    return result ?? new CallLogFullName { FullName = null };
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
