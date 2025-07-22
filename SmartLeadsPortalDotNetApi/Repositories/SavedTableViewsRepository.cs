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
                SELECT COUNT(*) 
                FROM SavedTableViews 
                WHERE OwnerId = @OwnerId 
                    AND TableName = @TableName 
                    AND ViewName = @ViewName
            """;
            var viewNameExists = await connection.ExecuteScalarAsync<int>(viewNameExistsQuery, savedTableView);

            if (viewNameExists > 0)
            {
                var update = """
                    UPDATE SavedTableViews 
                    SET ViewFilters = @ViewFilters, 
                        ModifiedAt = GETDATE(), 
                        ModifiedBy = @ModifiedBy,
                        IsDefault = @IsDefault 
                    WHERE OwnerId = @OwnerId 
                        AND TableName = @TableName 
                        AND ViewName = @ViewName
                """;
                await connection.ExecuteAsync(update, savedTableView);
                return;
            }

            var insert = """
                INSERT INTO SavedTableViews 
                    (GuId, TableName, ViewName, ViewFilters, OwnerId, Sharing, CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsDefault) 
                    VALUES 
                    (NEWID(), @TableName, @ViewName, @ViewFilters, @OwnerId, @Sharing, GETDATE(), @CreatedBy, GETDATE(), @ModifiedBy, @IsDefault)
            """;
            await connection.ExecuteAsync(insert, savedTableView);
        }
    }

    public async Task UpdateTableView(SavedTableView savedTableView)
    {
        using (var connection = this.dbConnectionFactory.GetSqlConnection())
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            using (var transaction = await connection.BeginTransactionAsync())
            {
                try
                {
                    if (savedTableView.IsDefault == true)
                    {
                        var updatePreviousDefault = """
                            UPDATE SavedTableViews 
                            SET IsDefault = 0
                            WHERE OwnerId = @OwnerId 
                                AND TableName = @TableName 
                                AND IsDefault = 1
                                AND Id != @Id
                        """;
                        await connection.ExecuteAsync(updatePreviousDefault, savedTableView, transaction);
                    }

                    var update = """
                        UPDATE SavedTableViews 
                        SET 
                            ViewName = @ViewName, 
                            ViewFilters = @ViewFilters, 
                            ModifiedAt = GETDATE(), 
                            ModifiedBy = @ModifiedBy,
                            IsDefault = @IsDefault
                        WHERE OwnerId = @OwnerId 
                            AND TableName = @TableName 
                            AND Id = @Id
                    """;
                    await connection.ExecuteAsync(update, savedTableView, transaction);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }

            }
        }
    }

    public async Task DeleteTableView(int id)
    {
        await using (var connection = await this.dbConnectionFactory.GetSqlConnectionAsync())
        {
            var delete = """
                DELETE FROM SavedTableViews 
                WHERE Id = @Id
            """;

            await connection.ExecuteAsync(delete, new { Id = id });
        }
    }
}

