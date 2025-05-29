using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartLeadsPortalDotNetApi.Configs;
using SmartLeadsPortalDotNetApi.Services.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SmartLeadsPortalDotNetApi.Services
{
    public class VoipHttpService
    {
        public readonly IHttpClientFactory httpClientFactory;
        public readonly IConfiguration _configuration;
        private readonly ILogger<VoipHttpService> _logger;
        public readonly VoIpConfig voIpConfig;

        public VoipHttpService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<VoipHttpService> logger, IOptions<VoIpConfig> voipConfig)
        {
            this.httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            this.voIpConfig = voipConfig.Value;
        }

        public async Task<VoipResponse?> GetVoipData()
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("token", voIpConfig.ApiKey);

            var response = await client.GetAsync($"{voIpConfig.BaseUrl}/get-user-calls?sort_by=newest_first");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VoipResponse?>(content);
        }

        public async Task<VoipResponse?> GetUsersCalls( string sortBy = "newest_first", string? fromDate = null, string? toDate = null, int offset = 0, int limit = 100, string? uniqueCallId = null)
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("token", voIpConfig.ApiKey);

            var queryParams = new System.Text.StringBuilder();
            queryParams.Append($"?sort_by={sortBy}");

            if (!string.IsNullOrEmpty(fromDate))
                queryParams.Append($"&from_date={fromDate}");

            if (!string.IsNullOrEmpty(toDate))
                queryParams.Append($"&to_date={toDate}");

            queryParams.Append($"&offset={offset}");
            queryParams.Append($"&limit={limit}");

            if (!string.IsNullOrEmpty(uniqueCallId))
                queryParams.Append($"&unique_call_id={uniqueCallId}");

            var url = $"{voIpConfig.BaseUrl}/get-user-calls{queryParams}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"VoIP API request failed with status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VoipResponse?>(content);
        }

        public async Task<VoipResponse?> GetQueueCalls(string sortBy = "newest_first", string? fromDate = null, string? toDate = null, string[]? queues = null, int offset = 0, int limit = 100, string? uniqueCallId = null)
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("token", voIpConfig.ApiKey);

            var queryParams = new System.Text.StringBuilder();
            queryParams.Append($"?sort_by={sortBy}");

            if (!string.IsNullOrEmpty(fromDate))
                queryParams.Append($"&from_date={fromDate}");

            if (!string.IsNullOrEmpty(toDate))
                queryParams.Append($"&to_date={toDate}");

            if (queues != null && queues.Length > 0)
                queryParams.Append($"&queues_names={string.Join(",", queues)}");

            queryParams.Append($"&offset={offset}");
            queryParams.Append($"&limit={limit}");

            if (!string.IsNullOrEmpty(uniqueCallId))
                queryParams.Append($"&unique_call_id={uniqueCallId}");

            var url = $"{voIpConfig.BaseUrl}/get-queue-calls{queryParams}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"VoIP API request failed with status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VoipResponse?>(content);
        }

        public async Task<VoipResponse?> GetRingGroupCalls(string sortBy = "newest_first", string? fromDate = null, string? toDate = null, string? ringGroups = null, int offset = 0, int limit = 100, string? uniqueCallId = null)
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("token", voIpConfig.ApiKey);

            var queryParams = new System.Text.StringBuilder();
            queryParams.Append($"?sort_by={sortBy}");

            if (!string.IsNullOrEmpty(fromDate))
                queryParams.Append($"&from_date={fromDate}");

            if (!string.IsNullOrEmpty(toDate))
                queryParams.Append($"&to_date={toDate}");

            if (!string.IsNullOrEmpty(ringGroups))
                queryParams.Append($"&ringgroups_names={ringGroups}");

            queryParams.Append($"&offset={offset}");
            queryParams.Append($"&limit={limit}");

            if (!string.IsNullOrEmpty(uniqueCallId))
                queryParams.Append($"&unique_call_id={uniqueCallId}");

            var url = $"{voIpConfig.BaseUrl}/get-ringgroup-calls{queryParams}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"VoIP API request failed with status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VoipResponse?>(content);
        }
        public async Task<CallToNumberResponse?> InitiateCallToNumber(CallToNumberRequest request)
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("token", voIpConfig.ApiKey);

            var jsonContent = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{voIpConfig.BaseUrl}/call-to-number", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"VoIP API request failed with status code: {response.StatusCode}. Response: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CallToNumberResponse?>(responseContent);
        }

    }
}
