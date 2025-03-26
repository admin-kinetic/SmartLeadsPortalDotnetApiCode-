using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartLeadsPortalDotNetApi.Configs;
using SmartLeadsPortalDotNetApi.Services.Model;
using System.Net.Http.Headers;

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
            var client = httpClientFactory.CreateClient("VoipClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", voIpConfig.ApiKey);
            var response = await client.GetAsync($"{voIpConfig.BaseUrl}/get-user-calls?sort_by=newest_first");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch VoIP data. Status Code: {StatusCode}, Message: {Response}",
                    response.StatusCode, await response.Content.ReadAsStringAsync());
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VoipResponse>(content);
        }

    }
}
