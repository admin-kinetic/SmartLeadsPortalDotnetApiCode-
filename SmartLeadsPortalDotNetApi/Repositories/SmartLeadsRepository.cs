using Dapper;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class SmartLeadsRepository: SQLDBService
    {
        public async Task<SmartLeadsCallTasksResponseModel<SmartLeadsCallTasks>> GetAllSmartLeadsCallTaskList(SmartLeadsCallTasksRequest model, string employeeId)
        {
            try
            {
                string _proc = "";
                var count = 0;
                var param = new DynamicParameters();
                IEnumerable<SmartLeadsCallTasks> list = new List<SmartLeadsCallTasks>();

                _proc = "sm_spGetSmartLeadsCallTasks";
                param.Add("@PageNumber", model.Page);
                param.Add("@PageSize", model.PageSize);
                param.Add("@EmployeeId", employeeId);

                list = await SqlMapper.QueryAsync<SmartLeadsCallTasks>(con, _proc, param, commandType: CommandType.StoredProcedure);

                var countProcedure = "sm_spGetSmartLeadsCallTasksCount";
                var countParam = new
                {
                    employeeId
                };

                count = await con.QueryFirstOrDefaultAsync<int>(countProcedure, countParam, commandType: CommandType.StoredProcedure);

                return new SmartLeadsCallTasksResponseModel<SmartLeadsCallTasks>
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
    }
}
