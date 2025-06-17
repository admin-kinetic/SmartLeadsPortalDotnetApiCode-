using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class SmartLeadsRepository: SQLDBService
    {
        private readonly DbConnectionFactory dbConnectionFactory;
        public SmartLeadsRepository(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }
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
        public async Task<SmartLeadsProspectDetails?> GetSmartLeadsProspectDetails(ProspectModelParam request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spLeadProspectCallDetails";
                    var param = new DynamicParameters();
                    param.Add("@email", request.Email);
                    SmartLeadsProspectDetails? result = await connection.QuerySingleOrDefaultAsync<SmartLeadsProspectDetails>(_proc, param, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<SmartLeadsExportedContactLeadGen>> GetAllLeadGenExportedLeads(SmartLeadRequest request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var param = new DynamicParameters();
                    IEnumerable<SmartLeadsExportedContactLeadGen> list = new List<SmartLeadsExportedContactLeadGen>();

                    if (request.EmailAddress == null || request.EmailAddress == "" || request.EmailAddress == "null")
                    {
                        request.EmailAddress = "";
                    }

                    string _proc = "sm_spGetLeadGenExportedLeadsPaginated";
                    param.Add("@Page", request.Page);
                    param.Add("@PageSize", request.PageSize);
                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@isValid", request.HasReview);
                    param.Add("@startDate", request.ExportedDateFrom);
                    param.Add("@endDate", request.ExportedDateTo);

                    list = await connection.QueryAsync<SmartLeadsExportedContactLeadGen>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<int?> GetAllLeadGenExportedLeadsCount(SmartLeadRequest request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var param = new DynamicParameters();

                    if (string.IsNullOrEmpty(request.EmailAddress) || request.EmailAddress == "null")
                    {
                        request.EmailAddress = "";
                    }

                    string _proc = "sm_spGetLeadGenExportedLeadsPaginatedCount";
                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@isValid", request.HasReview);
                    param.Add("@startDate", request.ExportedDateFrom);
                    param.Add("@endDate", request.ExportedDateTo);

                    var countResult = await connection.QueryFirstOrDefaultAsync<SmartLeadsExportedContactLeadGenCount?>(_proc, param, commandType: CommandType.StoredProcedure);

                    return countResult?.TotalCount;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<SmartLeadsExportedLeadsEmailed>> GetAllExportedLeadsEmailed(SmartLeadRequest request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var param = new DynamicParameters();
                    IEnumerable<SmartLeadsExportedLeadsEmailed> list = new List<SmartLeadsExportedLeadsEmailed>();

                    if (request.EmailAddress == null || request.EmailAddress == "" || request.EmailAddress == "null")
                    {
                        request.EmailAddress = "";
                    }

                    string _proc = "sm_spGetLeadsEmailedPaginated";
                    param.Add("@Page", request.Page);
                    param.Add("@PageSize", request.PageSize);
                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@startDate", request.ExportedDateFrom);
                    param.Add("@endDate", request.ExportedDateTo);

                    list = await connection.QueryAsync<SmartLeadsExportedLeadsEmailed>(_proc, param, commandType: CommandType.StoredProcedure);

                    return list;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<int?> GetAllExportedLeadsEmailedCount(SmartLeadRequest request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    var param = new DynamicParameters();

                    if (string.IsNullOrEmpty(request.EmailAddress) || request.EmailAddress == "null")
                    {
                        request.EmailAddress = "";
                    }

                    string _proc = "sm_spGetLeadsEmailedPaginatedCount";
                    param.Add("@email", request.EmailAddress);
                    param.Add("@hasReply", request.HasReply);
                    param.Add("@startDate", request.ExportedDateFrom);
                    param.Add("@endDate", request.ExportedDateTo);

                    var countResult = await connection.QueryFirstOrDefaultAsync<SmartLeadsExportedContactLeadGenCount?>(_proc, param, commandType: CommandType.StoredProcedure);

                    return countResult?.TotalCount;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> UpdateLeadsEmailDetails(SmartLeadsEmailedDetailsRequest request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spUpdateLeadsEmailedDetails";
                    var param = new DynamicParameters();
                    param.Add("@email", request.Email);
                    param.Add("@phone", request.PhoneNumber);
                    param.Add("@country", request.Country);

                    int ret = await connection.ExecuteAsync(_proc, param, commandType: CommandType.StoredProcedure);

                    return ret;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<LeadsDetailsModel?> GetLeadsProspectDetails(ProspectModelParam request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetLeadProspectDetails";
                    var param = new DynamicParameters();
                    param.Add("@email", request.Email);
                    LeadsDetailsModel? result = await connection.QuerySingleOrDefaultAsync<LeadsDetailsModel>(_proc, param, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
