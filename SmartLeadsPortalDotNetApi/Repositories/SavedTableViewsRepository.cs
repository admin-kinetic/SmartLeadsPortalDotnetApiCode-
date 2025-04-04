using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class SavedTableViewsRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;

    public SavedTableViewsRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<List<SavedTableView>?> GetTableViewsByOwnerId(int ownerId, string tableName)
    {
        using (var connection = this.dbConnectionFactory.GetSqlConnection())
        {
            var query = """
                SELECT * FROM SavedTableViews Where OwnerId = @OwnerId AND TableName = @TableName
                ORDER BY CreatedAt DESC
            """;

            var result = await connection.QueryAsync<SavedTableView>(query, new { ownerId, tableName });
            return result.ToList();
        }
    }

    public async Task SaveTableView(SavedTableView savedTableView)
    {
        using (var connection = this.dbConnectionFactory.GetSqlConnection())
        {
            var viewNameExistsQuery = """
                SELECT COUNT(*) FROM SavedTableViews WHERE OwnerId = @OwnerId AND TableName = @TableName AND ViewName = @ViewName
            """;
            var viewNameExistsParam = new
            {
                OwnerId = savedTableView.OwnerId,
                TableName = savedTableView.TableName,
                ViewName = savedTableView.ViewName
            };
            var viewNameExists = await connection.ExecuteScalarAsync<int>(viewNameExistsQuery, viewNameExistsParam);    

            if (viewNameExists > 0)
            {
                var update = """
                    UPDATE SavedTableViews 
                    SET ViewFilters = @ViewFilters, ModifiedAt = GETDATE(), ModifiedBy = @ModifiedBy 
                    WHERE OwnerId = @OwnerId AND TableName = @TableName AND ViewName = @ViewName
                """;
                var updateParam = new
                {
                    ViewFilters = savedTableView.ViewFilters,
                    ModifiedBy = savedTableView.ModifiedBy,
                    OwnerId = savedTableView.OwnerId,
                    TableName = savedTableView.TableName,
                    ViewName = savedTableView.ViewName
                };
                await connection.ExecuteAsync(update, updateParam);
                return;
            }

            var insert = """
                INSERT INTO SavedTableViews 
                    (GuId, TableName, ViewName, ViewFilters, OwnerId, Sharing, CreatedAt, CreatedBy, ModifiedAt, ModifiedBy) 
                    VALUES 
                    (NEWID(), @TableName, @ViewName, @ViewFilters, @OwnerId, @Sharing, GETDATE(), @CreatedBy, GETDATE(), @ModifiedBy)
            """;
            var insertParam = new
            {

            };
            await connection.ExecuteAsync(insert, savedTableView);
        }
    }
}
