using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartLeadsPortalDotNetApi.Configs;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace SmartLeadsPortalDotNetApi.Services;

public class SmartLeadsApiService
{
    private readonly DbConnectionFactory dbConnectionFactory;
    public readonly IHttpClientFactory httpClientFactory;
    public readonly IConfiguration _configuration;
    private readonly ILogger<SmartLeadsApiService> _logger;
    private readonly IOptions<SmartLeadConfig> botSmartLeadConfig;
    private readonly IOptions<BdrSmartLeadConfig> bdrSmartLeadConfig;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly UserRepository userRepository;
    private ISmartLeadConfig? smartLeadConfig;

    public SmartLeadsApiService(
        DbConnectionFactory dbConnectionFactory,
        HttpClient httpClient,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<SmartLeadsApiService> logger,
        IOptions<SmartLeadConfig> smartLeadConfig,
        IOptions<BdrSmartLeadConfig> bdrSmartLeadConfig,
        IHttpContextAccessor httpContextAccessor,
        UserRepository userRepository)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
        this.botSmartLeadConfig = smartLeadConfig;
        this.bdrSmartLeadConfig = bdrSmartLeadConfig;
        this.httpContextAccessor = httpContextAccessor;
        this.userRepository = userRepository;
    }

    private async Task InitializeSmartleadConfig()
    {
        if(this.smartLeadConfig != null)
        {
            return;
        }

        var user = this.httpContextAccessor.HttpContext?.User;
        if(user == null)
        {
            throw new Exception("User not found in HttpContext");
        }

        var employeeId = user.FindFirst("employeeId")?.Value;

        if(employeeId == null)
        {
            throw new Exception("Employee ID not found in HttpContext");
        }

        var accountId = await this.userRepository.GetSmartleadAccountByEmployeeId(employeeId);
        switch (accountId)
        {
            case 1:
                this.smartLeadConfig = this.botSmartLeadConfig.Value;
                break;
            case 2:
                this.smartLeadConfig = this.bdrSmartLeadConfig.Value;
                break;
            default:
                throw new Exception("Invalid account ID");
        }
    }

    //Get All Campaigns
    public async Task<List<SmartLeadsCampaign>?> GetSmartLeadsCampaigns()
    {
        await this.InitializeSmartleadConfig();
        if (smartLeadConfig.ApiKey == null)
        {
             throw new Exception("API Key is null");
        }

        using var client = httpClientFactory.CreateClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["api_key"] = smartLeadConfig.ApiKey;

        var response = await client.GetAsync($"{smartLeadConfig.BaseUrl}/campaigns?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var campaigns = JsonConvert.DeserializeObject<List<SmartLeadsCampaign>>(content);
        return campaigns;
    }

    //Get Campaigns details by Id
    public async Task<SmartLeadsCampaign?> GetSmartLeadsCampaignById(int id)
    {
        await this.InitializeSmartleadConfig();
        if (smartLeadConfig.ApiKey == null)
        {
             throw new Exception("API Key is null");
        }

        using var client = httpClientFactory.CreateClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["api_key"] = smartLeadConfig.ApiKey;

        var response = await client.GetAsync($"{smartLeadConfig.BaseUrl}/campaigns/{id}?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var campaigns = JsonConvert.DeserializeObject<SmartLeadsCampaign>(content);
        return campaigns;
    }

    //Get All Leads by Campaign
    public async Task<SmartLeadsResponse?> GetLeadsByCampaignId(int id, int offset, int limit)
    {
        await this.InitializeSmartleadConfig();
        if (smartLeadConfig.ApiKey == null)
        {
             throw new Exception("API Key is null");
        }

        offset = (offset - 1) * limit;
        using var client = httpClientFactory.CreateClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["api_key"] = smartLeadConfig.ApiKey;
        queryString["offset"] = offset.ToString();
        queryString["limit"] = limit.ToString();

        var response = await client.GetAsync($"{smartLeadConfig.BaseUrl}/campaigns/{id}/leads?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var campaigns = JsonConvert.DeserializeObject<SmartLeadsResponse>(content);
        return campaigns;
    }

    //Get lead by email
    public async Task<SmartLeadsByEmailResponse?> GetLeadByEmail(string email)
    {
        await this.InitializeSmartleadConfig();
        if (smartLeadConfig.ApiKey == null)
        {
             throw new Exception("API Key is null");
        }

        using var client = httpClientFactory.CreateClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["api_key"] = smartLeadConfig.ApiKey;
        queryString["email"] = email;

        var response = await client.GetAsync($"{smartLeadConfig.BaseUrl}/leads?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var campaigns = JsonConvert.DeserializeObject<SmartLeadsByEmailResponse>(content);
        return campaigns;
    }

    //Get All Leads in entire Account
    public async Task<SmartLeadsAllLeadsResponse?> GetAllLeadsAllAccount(string? createdDate = null, string? email = null, int offset = 0, int limit = 0)
    {
        await this.InitializeSmartleadConfig();
        if (smartLeadConfig.ApiKey == null)
        {
             throw new Exception("API Key is null");
        }

        offset = (offset - 1) * limit;
        using var client = httpClientFactory.CreateClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["api_key"] = smartLeadConfig.ApiKey;
        queryString["offset"] = offset.ToString();
        queryString["limit"] = limit.ToString();

        if (!string.IsNullOrEmpty(createdDate))
            queryString["created_at_gt"] = createdDate;

        if (!string.IsNullOrEmpty(email))
            queryString["email"] = email;

        var response = await client.GetAsync($"{smartLeadConfig.BaseUrl}/leads/global-leads?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var campaigns = JsonConvert.DeserializeObject<SmartLeadsAllLeadsResponse>(content);
        return campaigns;
    }

    //Get Statistics by Campaign
    public async Task<CampaignStatisticsResponse?> GetStatisticsByCampaign(int id, int offset = 0, int limit = 0)
    {
        await this.InitializeSmartleadConfig();
        if (smartLeadConfig.ApiKey == null)
        {
             throw new Exception("API Key is null");
        }

        offset = (offset - 1) * limit;
        var client = httpClientFactory.CreateClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["api_key"] = smartLeadConfig.ApiKey;
        queryString["offset"] = offset.ToString();
        queryString["limit"] = limit.ToString();

        var response = await client.GetAsync($"{smartLeadConfig.BaseUrl}/campaigns/{id}/statistics?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var campaigns = JsonConvert.DeserializeObject<CampaignStatisticsResponse>(content);
        return campaigns;
    }

    //Get Analytics by Campaign
    public async Task<CampaignAnalytics?> GetAnalyticsByCampaign(int id)
    {
        await this.InitializeSmartleadConfig();
        if (smartLeadConfig.ApiKey == null)
        {
             throw new Exception("API Key is null");
        }

        await Task.Delay(100);

        using var client = httpClientFactory.CreateClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["api_key"] = smartLeadConfig.ApiKey;

        var response = await client.GetAsync($"{smartLeadConfig.BaseUrl}/campaigns/{id}/analytics?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var campaigns = JsonConvert.DeserializeObject<CampaignAnalytics>(content);
        return campaigns;
    }

    //Get Sequence Analytics by Campaign
    public async Task<CampaignAnalyticsResponse?> GetSequenceAnaylyticByCampaign(int id, string? start_date = null, string? end_date = null, string? time_zone = null)
    {
        await this.InitializeSmartleadConfig();
        if (smartLeadConfig.ApiKey == null)
        {
             throw new Exception("API Key is null");
        }

        using var client = httpClientFactory.CreateClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["api_key"] = smartLeadConfig.ApiKey;
        queryString["start_date"] = start_date;
        queryString["end_date"] = end_date;

        if (!string.IsNullOrEmpty(time_zone))
            queryString["time_zone"] = time_zone;

        var response = await client.GetAsync($"{smartLeadConfig.BaseUrl}/campaigns/{id}/sequence-analytics?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var campaigns = JsonConvert.DeserializeObject<CampaignAnalyticsResponse>(content);
        return campaigns;
    }

    //Get Analytics by Campaign Date Range
    public async Task<CampaignAnalyticsDateRange?> GetAnalyticsByCampaignDateRange(int id, string? start_date = null, string? end_date = null)
    {
        await this.InitializeSmartleadConfig();
        if (smartLeadConfig.ApiKey == null)
        {
             throw new Exception("API Key is null");
        }

        using var client = httpClientFactory.CreateClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["api_key"] = smartLeadConfig.ApiKey;
        queryString["start_date"] = start_date;
        queryString["end_date"] = end_date;

        var response = await client.GetAsync($"{smartLeadConfig.BaseUrl}/campaigns/{id}/analytics-by-date?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var campaigns = JsonConvert.DeserializeObject<CampaignAnalyticsDateRange>(content);
        return campaigns;
    }
}

