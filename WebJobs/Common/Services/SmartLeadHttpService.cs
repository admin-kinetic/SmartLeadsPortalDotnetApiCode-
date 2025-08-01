using Common.Models;
using Common.Repositories;
using System.Collections.Generic;
using System.Text.Json;
using System.Web;

namespace Common.Services;

public class SmartLeadHttpService
{
    private readonly string baseUrl = "https://server.smartlead.ai/api/v1";
    //private readonly string apiKey = "0f47052b-e08b-488b-8ec3-dd949eec520a_umaccv1";

    //private readonly string apiKey = "54251016-7093-4899-8111-63cc96e9757c_4dp5h84";

    //private readonly string[] apiKeys = new string[] {
    //    "0f47052b-e08b-488b-8ec3-dd949eec520a_umaccv1",
    //    "54251016-7093-4899-8111-63cc96e9757c_4dp5h84"
    //};

    public SmartLeadHttpService()
    {

    }

    public async Task<FetchAllLeadsFromEntireAccountResponse> FetchAllLeadsFromEntireAccount(DateTime fromDate, int offset, int limit, string apiKey)
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

    public async Task<FetchAllLeadsFromEntireAccountResponse> FetchAllLeadsFromEntireAccountByEmail(string email, string apiKey)
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

    public async Task<FetchCampaignStatisticsByCampaignIdResponse> FetchCampaignStatisticsByCampaignId(int campaignId, string apiKey, int offset = 0, int limit = 500, int daysOffset = 7) //, DateTime startDate, DateTime endData, int limit, int offset)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {

                var weekAgo = DateTime.Now.AddDays(-daysOffset);

                var queryParams = new Dictionary<string, string>
                {
                    // { "created_at_gt", fromDate.ToString("yyyy-MM-dd") },
                    { "api_key", apiKey },
                    { "limit", limit.ToString() },
                    { "offset", offset.ToString() },
                    { "email_status", "opened"},
                    { "sent_time_start_date", weekAgo.ToString("yyyy-MM-dd") },
                    { "sent_time_end_date", DateTime.Now.ToString("yyyy-MM-dd") } 
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

    public async Task<FetchCampaignLeadStatisticsResponse> FetchCampaignLeadStatistics(int campaignId, string apiKey, int offset = 0, int limit = 500, int daysOffset = 7) //, DateTime startDate, DateTime endData, int limit, int offset)
    {
        using (var httpClient = new HttpClient())
        {

            var offsetDate = DateTime.Now.AddDays(-daysOffset);

            var queryParams = new Dictionary<string, string>
                {
                    { "api_key", apiKey },
                    { "event_time_gt", offsetDate.ToString("yyyy-MM-dd") },
                    { "limit", limit.ToString() },
                    { "offset", offset.ToString() }
                };

            // Serialize the dictionary into a query string
            var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            var response = await httpClient.GetAsync($"{this.baseUrl}/campaigns/{campaignId}/leads-statistics?{queryString}");
            // Check status code
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"HTTP request failed with status code: {(int)response.StatusCode}");
            }

            // Read response body
            string responseBody = await response.Content.ReadAsStringAsync();

            // Deserialize JSON response
            return JsonSerializer.Deserialize<FetchCampaignLeadStatisticsResponse>(responseBody);
        }
    }

    public async Task<List<ListAllCampaignsResponse>> ListAllCampaigns(string apiKey)
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

    public async Task<SmartLeadsByEmailResponse?> LeadByEmail(string email, string apiKey)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {

                var weekAgo = DateTime.Now.AddDays(-7);

                var queryParams = new Dictionary<string, string>
                {
                    // { "created_at_gt", fromDate.ToString("yyyy-MM-dd") },
                    { "api_key", apiKey },
                    { "email", email },
                };

                // Serialize the dictionary into a query string
                var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                var response = await httpClient.GetAsync($"{this.baseUrl}/leads?{queryString}");
                // Check status code
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"HTTP request failed with status code: {(int)response.StatusCode}");
                }

                response.Headers.TryGetValues("X-RateLimit-Remaining", out var rateLimitRemainingValues);
                Console.WriteLine($"Rate limit remaining: {rateLimitRemainingValues?.First()}");

                // Read response body
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response
                var result =  JsonSerializer.Deserialize<SmartLeadsByEmailResponse?>(responseBody);

                if (result != null && result.GetType().GetProperties().All(p => p.GetValue(result) == null))
                {
                    return null;
                }

                if (result != null  && result.email == null)
                {
                    return null;
                }

                return result;
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

    public async Task<MessageHistoryByLeadsResponse> MessageHistoryByLead(int? leadCampaignId, string leadId, string apiKey)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {

                var weekAgo = DateTime.Now.AddDays(-7);

                var queryParams = new Dictionary<string, string>
                {
                    { "api_key", apiKey },
                };

                // Serialize the dictionary into a query string
                var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                var response = await httpClient.GetAsync($"{this.baseUrl}/campaigns/{leadCampaignId}/leads/{leadId}/message-history?{queryString}");
                // Check status code
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"HTTP request failed with status code: {(int)response.StatusCode}");
                }

                response.Headers.TryGetValues("X-RateLimit-Remaining", out var rateLimitRemainingValues);
                Console.WriteLine($"Rate limit remaining: {rateLimitRemainingValues?.First()}");

                // Read response body
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response
                return JsonSerializer.Deserialize<MessageHistoryByLeadsResponse>(responseBody);
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

    //Get lead by email
    public async Task<SmartLeadsByEmailResponse?> GetLeadByEmail(string email, string? apiKey = null)
    {
        using var client = new HttpClient();
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["api_key"] = apiKey;
        queryString["email"] = email;

        var response = await client.GetAsync($"{this.baseUrl}/leads?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching data: {response.StatusCode} - {errorMessage}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var campaigns = JsonSerializer.Deserialize<SmartLeadsByEmailResponse>(content);
        return campaigns;
    }

}
