using System;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SmartLeadsPortalDotNetApi.Configs;
using SmartLeadsPortalDotNetApi.Model;

namespace SmartLeadsPortalDotNetApi.Services;

public class LeadsPortalHttpService
{
    private readonly HttpClient client;
    private readonly KineticLeadsPortalConfig kineticLeadsPortalConfig;

    public LeadsPortalHttpService(HttpClient client, IOptions<KineticLeadsPortalConfig> kineticLeadsPortalConfig)
    {
        this.client = client;
        this.kineticLeadsPortalConfig = kineticLeadsPortalConfig.Value;
    }

    public async Task<LeadsPortalContactDetailsResponse> GetContactDetailsByEmail(string email)
    {
        var response = await client.GetAsync($"{this.kineticLeadsPortalConfig.BaseUrl}/RobotcrawledcontactsAutoGen/email/{email}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error fetching contact details: {response.StatusCode} - {response.ReasonPhrase}");
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new LeadsPortalContactDetailsResponse();
        }

        var content = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<LeadsPortalContactDetailsResponse>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return responseObject;
    }

    // internal async Task GetleadContactNoByEmail(string email)
    // {
    //     throw new NotImplementedException();
    // }
}
