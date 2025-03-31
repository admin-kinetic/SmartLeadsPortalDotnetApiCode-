using System.Text.Json;
using Common.Models;

namespace Common.Services;

public class SmartLeadHttpService
{
    private readonly string baseUrl = "https://server.smartlead.ai/api/v1";
    private readonly string apiKey = "0f47052b-e08b-488b-8ec3-dd949eec520a_umaccv1";
    public SmartLeadHttpService()
    {

    }

    public async Task<FetchAllLeadsFromEntireAccountResponse> FetchAllLeadsFromEntireAccount(DateTime fromDate, int offset, int limit)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                var queryParams = new Dictionary<string, string>
                {
                    { "created_at_gt", fromDate.ToString("yyyy-MM-dd") },
                    { "api_key", apiKey },
                    { "limit", limit.ToString() },
                    { "offset", offset.ToString() }
                };

                // Serialize the dictionary into a query string
                var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                var response = await httpClient.GetAsync($"{this.baseUrl}/leads/global-leads?{queryString}");
                // Check status code
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"HTTP request failed with status code: {(int)response.StatusCode}");
                }

                // Read response body
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response
                return JsonSerializer.Deserialize<FetchAllLeadsFromEntireAccountResponse>(responseBody);
            }
        }
        catch (HttpRequestException ex)
        {
            // Handle HTTP client exceptions
            throw new Exception("HTTP request failed: " + ex.Message);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw ex;
        }
    }

    public async Task<FetchAllLeadsFromEntireAccountResponse> FetchAllLeadsFromEntireAccountByEmail(string email)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                var queryParams = new Dictionary<string, string>
                {
                    { "api_key", apiKey },
                    { "email", email }
                };

                // Serialize the dictionary into a query string
                var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                var response = await httpClient.GetAsync($"{this.baseUrl}/leads/global-leads?{queryString}");
                // Check status code
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"HTTP request failed with status code: {(int)response.StatusCode}");
                }

                // Read response body
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response
                return JsonSerializer.Deserialize<FetchAllLeadsFromEntireAccountResponse>(responseBody);
            }
        }
        catch (HttpRequestException ex)
        {
            // Handle HTTP client exceptions
            throw new Exception("HTTP request failed: " + ex.Message);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw ex;
        }
    }

    public async Task<FetchCampaignStatisticsByCampaignIdResponse> FetchCampaigncStatisticsByCampaignId(int campaignId, int offset = 0, int limit = 500) //, DateTime startDate, DateTime endData, int limit, int offset)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                var queryParams = new Dictionary<string, string>
                {
                    // { "created_at_gt", fromDate.ToString("yyyy-MM-dd") },
                    { "api_key", apiKey },
                    { "limit", limit.ToString() },
                    { "offset", offset.ToString() },
                    { "email_status", "opened"},
                    // { "sent_time_start_date", "2025-05-26" }, // Example start date
                    // { "sent_time_end_date", "2025-05-31" } // Example end date
                };

                // Serialize the dictionary into a query string
                var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                var response = await httpClient.GetAsync($"{this.baseUrl}/campaigns/{campaignId}/statistics?{queryString}");
                // Check status code
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"HTTP request failed with status code: {(int)response.StatusCode}");
                }

                // Read response body
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response
                return JsonSerializer.Deserialize<FetchCampaignStatisticsByCampaignIdResponse>(responseBody);
            }
        }
        catch (HttpRequestException ex)
        {
            // Handle HTTP client exceptions
            throw new Exception("HTTP request failed: " + ex.Message);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw ex;
        }
    }

     public async Task<List<ListAllCampaignsResponse>> ListAllCampaigns()
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                var queryParams = new Dictionary<string, string>
                {
                    { "api_key", apiKey },
                };

                // Serialize the dictionary into a query string
                var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                var response = await httpClient.GetAsync($"{this.baseUrl}/campaigns?{queryString}");
                // Check status code
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"HTTP request failed with status code: {(int)response.StatusCode}");
                }

                // Read response body
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response
                return JsonSerializer.Deserialize<List<ListAllCampaignsResponse>>(responseBody);
            }
        }
        catch (HttpRequestException ex)
        {
            // Handle HTTP client exceptions
            throw new Exception("HTTP request failed: " + ex.Message);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw ex;
        }
    }



}
