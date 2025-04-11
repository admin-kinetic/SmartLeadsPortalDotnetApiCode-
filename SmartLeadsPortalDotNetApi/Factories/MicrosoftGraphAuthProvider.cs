using System;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using SmartLeadsPortalDotNetApi.Configs;

namespace SmartLeadsPortalDotNetApi.Factories;

public class MicrosoftGraphAuthProvider
{
    private readonly MicrosoftGraphSettings graphSettings;

    public MicrosoftGraphAuthProvider(IOptions<MicrosoftGraphSettings> microsoftGraphSettings)
    {
        this.graphSettings = microsoftGraphSettings.Value;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        string authority = string.Format($"{this.graphSettings.Authority}/{this.graphSettings.TenantId}");
        var clientApp = ConfidentialClientApplicationBuilder.Create(this.graphSettings.ClientId)
            .WithClientSecret(this.graphSettings.ClientSecret)
            .WithAuthority(new Uri(authority))
            .Build();
        var result = await clientApp.AcquireTokenForClient(new[] { this.graphSettings.Scope }).ExecuteAsync();
        return result.AccessToken;
    }
}
