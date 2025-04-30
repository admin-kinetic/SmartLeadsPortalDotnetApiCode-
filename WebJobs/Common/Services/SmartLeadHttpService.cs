using System.Collections.Generic;
using System.Text.Json;
using Common.Models;
using Common.Repositories;

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

    public async Task<FetchCampaignStatisticsByCampaignIdResponse> FetchCampaignStatisticsByCampaignId(int campaignId, int offset = 0, int limit = 500) //, DateTime startDate, DateTime endData, int limit, int offset)
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

    public async Task<LeadByEmailResponse?> LeadByEmail(string email)
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
                var result =  JsonSerializer.Deserialize<LeadByEmailResponse?>(responseBody);

                if (result != null && result.GetType().GetProperties().All(p => p.GetValue(result) == null))
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

    public async Task<MessageHistoryByLeadsResponse> MessageHistoryByLead(int? leadCampaignId, string leadId)
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
}
