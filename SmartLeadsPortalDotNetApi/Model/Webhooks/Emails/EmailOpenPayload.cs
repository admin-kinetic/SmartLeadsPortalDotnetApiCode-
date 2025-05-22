using System;

namespace SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Metadata
{
    public DateTime webhook_created_at { get; set; }
}

public class EmailOpenPayload
{
    public string? sl_email_lead_id { get; set; }
    public int? sl_email_lead_map_id { get; set; }
    public string? campaign_status { get; set; }
    public object client_id { get; set; }
    public string? stats_id { get; set; }
    public string? from_email { get; set; }
    public string? to_email { get; set; }
    public string? to_name { get; set; }
    public DateTime? time_opened { get; set; }
    public DateTime? event_timestamp { get; set; }
    public string? campaign_name { get; set; }
    public int? campaign_id { get; set; }
    public int? sequence_number { get; set; }
    public string? subject { get; set; }
    public string? sent_message_body { get; set; }
    public SentMessage? sent_message { get; set; }
    public string? secret_key { get; set; }
    public string? app_url { get; set; }
    public string? ui_master_inbox_link { get; set; }
    public string? description { get; set; }
    public Metadata? metadata { get; set; }
    public string? webhook_url { get; set; }
    public int? webhook_id { get; set; }
    public string? webhook_name { get; set; }
    public string? event_type { get; set; }
}

// public class SentMessage
// {
//     public  string? message_id { get; set; }
//     public  string? html { get; set; }
//     public  string? text { get; set; }
// }

