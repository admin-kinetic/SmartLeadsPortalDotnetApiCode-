using System;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class SavedTableViewsRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;

    public SavedTableViewsRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task GetTableViewsByOwnerId(int ownerId)
    {

    }
}
