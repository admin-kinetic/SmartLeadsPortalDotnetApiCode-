using Dapper;
using MySqlConnector;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public interface IExcludedKeywordsRepository
    {
        Task<bool> InsertKeyword(ExcludedKeywords keyword);
        Task<ExcludedKeywordsResponseModel<ExcludedKeywords>> GetAllKeywords(ExcludedKeywordsListRequest request);
        Task<IEnumerable<ExcludedKeywords>> GetAllKeywordsMap();
        Task<bool> UpdateKeyword(ExcludedKeywords keyword);
        Task<ExcludedKeywords> GetKeywordById(int id);
        Task<bool> DeleteKeyword(int id);
    }
    public class ExcludedKeywordsRepository : SQLDBService, IExcludedKeywordsRepository
    {
        private readonly string _connectionString;
        public ExcludedKeywordsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySQLDBConnectionString");
        }

        // Insert Keyword
        public async Task<bool> InsertKeyword(ExcludedKeywords keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword.ExludedKeywords))
                    throw new Exception("No valid data provided.");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"INSERT INTO ExcludeKeywords (ExludedKeywords, IsActive) 
                                  VALUES (@ExcludedKeywords, @IsActive)";

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
                using (var connection = new MySqlConnection(_connectionString))
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
                using (var connection = new MySqlConnection(_connectionString))
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

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"UPDATE ExcludeKeywords 
                                  SET ExludedKeywords = @ExcludedKeywords, IsActive = @IsActive 
                                  WHERE Id = @Id";

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
                using (var connection = new MySqlConnection(_connectionString))
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

                using (var connection = new MySqlConnection(_connectionString))
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
