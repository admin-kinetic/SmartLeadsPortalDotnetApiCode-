using Dapper;
using MySqlConnector;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class ExcludedKeywordsRepository : SQLDBService
    {
        private readonly string _mysqlconnectionString;
        private readonly string _connectionString;
        public ExcludedKeywordsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SmartLeadsSQLServerDBConnectionString");
            _mysqlconnectionString = configuration.GetConnectionString("MySQLDBConnectionString");
        }

        //MS SQL
        public async Task<int> InsertExcludedKeyword(ExcludedKeywordsInsert keyword)
        {
            try
            {
                string _proc = "sm_spInsertExcludeKeywords";
                var param = new DynamicParameters();
                param.Add("@excludedkeywords", keyword.ExludedKeywords);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task<int> UpdateExcludedKeyword(ExcludedKeywordsModel keyword)
        {
            try
            {
                string _proc = "sm_spUpdateExcludeKeywords";
                var param = new DynamicParameters();
                param.Add("@guid", keyword.Guid);
                param.Add("@excludedkeywords", keyword.ExcludedKeyword);
                param.Add("@isactive", keyword.IsActive);

                int ret = await SqlMapper.ExecuteAsync(con, _proc, param, commandType: CommandType.StoredProcedure);

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }

        public async Task<ExcludedKeywordsResponseModel<ExcludedKeywordsModel>> GetAllExcludeKeywordsList(ExcludedKeywordsListRequest model)
        {
            try
            {
                string _proc = "";
                var count = 0;
                var param = new DynamicParameters();
                var param2 = new DynamicParameters();
                IEnumerable<ExcludedKeywordsModel> list = new List<ExcludedKeywordsModel>();

                _proc = "sm_spGetAllExcludeKeywordsList";
                param.Add("@PageNumber", model.Page);
                param.Add("@PageSize", model.PageSize);
                param.Add("@Search", model.Search);

                list = await SqlMapper.QueryAsync<ExcludedKeywordsModel>(con, _proc, param, commandType: CommandType.StoredProcedure);

                var countProcedure = "sm_spGetAllExcludeKeywordsListCount";
                param2.Add("@Search", model.Search);
                count = await con.QueryFirstOrDefaultAsync<int>(countProcedure, param2, commandType: CommandType.StoredProcedure);

                return new ExcludedKeywordsResponseModel<ExcludedKeywordsModel>
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

        public async Task<ExcludedKeywordsModel?> GetExcludeKeywordsById(Guid guid)
        {
            try
            {
                string _proc = "sm_spGetExcludeKeywordsById";
                var param = new DynamicParameters();
                param.Add("@guid", guid);
                ExcludedKeywordsModel? list = await SqlMapper.QuerySingleOrDefaultAsync<ExcludedKeywordsModel>(con, _proc, param, commandType: CommandType.StoredProcedure);

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
        public async Task<IEnumerable<ExcludedKeywordsModel>> GetAllExcludeKeywordsMap()
        {
            try
            {
                string _proc = "sm_spGetAllExcludeKeywordsMap";
                IEnumerable<ExcludedKeywordsModel> list = await SqlMapper.QueryAsync<ExcludedKeywordsModel>(con, _proc, commandType: CommandType.StoredProcedure);
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
        public async Task<int> DeleteExcludedKeyword(Guid guid)
        {
            try
            {
                string _proc = "sm_spDeleteExcludeKeywords";
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



        //MYSQL Code
        // Insert Keyword
        public async Task<bool> InsertKeyword(ExcludedKeywordsInsert keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword.ExludedKeywords))
                    throw new Exception("No valid data provided.");

                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = @"INSERT INTO ExcludeKeywords (ExludedKeywords, IsActive)VALUES (@ExludedKeywords, @IsActive)";

                    var result = await connection.ExecuteAsync(query, new
                    {
                        keyword.ExludedKeywords,
                        keyword.IsActive
                    });

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<ExcludedKeywordsResponseModel<ExcludedKeywords>> GetAllKeywords(ExcludedKeywordsListRequest request)
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM ExcludeKeywords ORDER BY Id ASC LIMIT @Limit OFFSET @Offset";
                    var countQuery = "SELECT COUNT(Id) as Count FROM ExcludeKeywords";

                    var parameters = new DynamicParameters();
                    parameters.Add("Limit", request.PageSize);
                    parameters.Add("Offset", (request.Page - 1) * request.PageSize);

                    var items = await connection.QueryAsync<ExcludedKeywords>(query, parameters);
                    var total = await connection.QueryFirstOrDefaultAsync<int>(countQuery);

                    return new ExcludedKeywordsResponseModel<ExcludedKeywords> { Items = items.ToList(), Total = total };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<ExcludedKeywords>> GetAllKeywordsMap()
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM ExcludeKeywords ORDER BY Id ASC";
                    var result = await connection.QueryAsync<ExcludedKeywords>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<bool> UpdateKeyword(ExcludedKeywords keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword.ExludedKeywords))
                    throw new Exception("No keyword provided.");

                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = @"UPDATE ExcludeKeywords SET ExludedKeywords = @ExludedKeywords, IsActive = @IsActive WHERE Id = @Id";

                    var result = await connection.ExecuteAsync(query, new
                    {
                        keyword.ExludedKeywords,
                        keyword.IsActive,
                        keyword.Id
                    });

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update keyword: " + ex.Message);
            }
        }

        public async Task<ExcludedKeywords> GetKeywordById(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM ExcludeKeywords WHERE Id = @Id LIMIT 1";
                    var result = await connection.QueryFirstOrDefaultAsync<ExcludedKeywords>(query, new { Id = id });
                    return result ?? new ExcludedKeywords();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch keyword: " + ex.Message);
            }
        }
        public async Task<bool> DeleteKeyword(int id)
        {
            try
            {
                if (id <= 0)
                    throw new Exception("No valid id provided.");

                using (var connection = new MySqlConnection(_mysqlconnectionString))
                {
                    await connection.OpenAsync();
                    var query = "DELETE FROM ExcludeKeywords WHERE Id = @Id";

                    var result = await connection.ExecuteAsync(query, new { Id = id });

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete keyword: " + ex.Message);
            }
        }
    }
}
