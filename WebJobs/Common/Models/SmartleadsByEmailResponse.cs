namespace Common.Models;

public class SmartLeadsByEmailResponse
{
    public string? id { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }
    public string? email { get; set; }
    public DateTime created_at { get; set; }
    public string? phone_number { get; set; }
    public string? company_name { get; set; }
    public string? website { get; set; }
    public string? location { get; set; }
    public CustomFields custom_fields { get; set; }
    public object linkedin_profile { get; set; }
    public object company_url { get; set; }
    public bool is_unsubscribed { get; set; }
    public UnsubscribedClientIdMap unsubscribed_client_id_map { get; set; }
    public List<LeadCampaignDatum> lead_campaign_data { get; set; }
}
