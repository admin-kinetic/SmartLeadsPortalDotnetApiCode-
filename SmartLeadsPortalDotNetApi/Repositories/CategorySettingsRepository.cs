using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using System.Data;
using System.Drawing;
using static Dapper.SqlMapper;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class CategorySettingsRepository
    {
        private readonly DbConnectionFactory dbConnectionFactory;
        public CategorySettingsRepository(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<IEnumerable<CategorySettings>> GetCategorySettings()
        {
            try
            {
                IEnumerable<CategorySettings> list = new List<CategorySettings>();

                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    string _proc = "sm_spGetCategorySettings";

                    list = await connection.QueryAsync<CategorySettings>(_proc, commandType: CommandType.StoredProcedure);

                    return list;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
        public async Task<int> UpdateCategorySettings(List<CategorySettings> request)
        {
            try
            {
                using (var connection = this.dbConnectionFactory.GetSqlConnection())
                {
                    int ret = 0;
                    foreach (var item in request)
                    {
                        string _proc = "sm_spUpdateCategorySettings";
                        var param = new DynamicParameters();
                        param.Add("@id", item.Id);
                        param.Add("@opencount", item.OpenCount);
                        param.Add("@clickcount", item.ClickCount);

                        ret = await connection.ExecuteAsync(_proc, param);
                    }
                    return ret;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }
        }
    }
}
