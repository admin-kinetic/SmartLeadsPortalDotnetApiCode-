using Dapper;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class SmartLeadsRepository: SQLDBService
    {
        private readonly string _connectionString;
        public SmartLeadsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQLServerDBConnectionString");
        }
        public async Task<SmartLeadsCallTasksResponseModel<SmartLeadsCallTasks>> GetAllSmartLeadsCallTaskList(SmartLeadsCallTasksRequest model)
        {
            try
            {
                string _proc = "";
                var count = 0;
                var param = new DynamicParameters();
                var param2 = new DynamicParameters();
                IEnumerable<SmartLeadsCallTasks> list = new List<SmartLeadsCallTasks>();

                _proc = "sm_spGetSmartLeadsCallTasks";
                param.Add("@PageNumber", model.Page);
                param.Add("@PageSize", model.PageSize);

                list = await SqlMapper.QueryAsync<SmartLeadsCallTasks>(con, _proc, param, commandType: CommandType.StoredProcedure);

                var countProcedure = "sm_spGetSmartLeadsCallTasksCount";
                count = await con.QueryFirstOrDefaultAsync<int>(countProcedure, commandType: CommandType.StoredProcedure);

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
