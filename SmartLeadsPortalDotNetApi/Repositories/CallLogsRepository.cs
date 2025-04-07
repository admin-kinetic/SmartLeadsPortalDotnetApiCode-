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
            try
            {
                string _proc = "";
                var param = new DynamicParameters();
                param.Add("@", keyword.ProspectName);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
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

    }
}
