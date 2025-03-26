using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ImportLeadsFromLeadsPortal.Models;

namespace ExportedLeadsFromLeadsPortal
{
    public class LeadsPortalService
    {
        public readonly string leadsPortalUrl = "https://app-leads.kineticstaff.com";
        public LeadsPortalService()
        {

        }


        public async Task<List<LeadsPortalResponse>> GetExportedToSmartleadsContacts(DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Create HTTP client instance
                using (HttpClient client = new HttpClient())
                {
                    // Prepare JSON payload
                    var jsonPayload = new Dictionary<string, string>
                    {
                        { "fromDate", fromDate.ToString("yyyy-MM-dd") },
                        { "toDate", toDate.ToString("yyyy-MM-dd") }
                    };

                    // Serialize payload to JSON
                    string jsonContent = JsonSerializer.Serialize(jsonPayload);
                    HttpContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    // Send POST request
                    HttpResponseMessage response = await client.PostAsync(
                        $"{this.leadsPortalUrl}/api/robotcrawledcontactsautogen/smartleads-exported-contacts",
                        content
                    );

                    // Check status code
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new Exception($"HTTP request failed with status code: {(int)response.StatusCode}");
                    }

                    // Read response body
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Deserialize JSON response
                    return JsonSerializer.Deserialize<List<LeadsPortalResponse>>(responseBody);
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
}
