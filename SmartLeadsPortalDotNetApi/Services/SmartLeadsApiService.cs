using System;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Services;

public class SmartLeadsApiService
{
    private readonly DbConnectionFactory dbConnectionFactory;

    public SmartLeadsApiService(DbConnectionFactory dbConnectionFactory, HttpClient httpClient)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }
}
