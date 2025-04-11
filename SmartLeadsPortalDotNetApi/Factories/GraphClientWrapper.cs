using System;
using Microsoft.Graph;

namespace SmartLeadsPortalDotNetApi.Factories;

public class GraphClientWrapper
{
    private readonly MicrosoftGraphServiceClientFactory _factory;
    private GraphServiceClient _graphClient;

    public GraphClientWrapper(MicrosoftGraphServiceClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<GraphServiceClient?> GetClientAsync()
    {
        if (_graphClient == null)
        {
            _graphClient = await _factory.GetAuthenticatedGraphClient();
        }
        return _graphClient;
    }
}
