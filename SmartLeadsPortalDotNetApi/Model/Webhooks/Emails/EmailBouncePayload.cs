using System;

namespace SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class BounceMessage
{
    public string message_id { get; set; }
    public string html { get; set; }
    public string text { get; set; }
    public DateTime time { get; set; }
}

public class EmailBouncePayload
{
    public string campaign_status { get; set; }
    public object client_id { get; set; }
    public string stats_id { get; set; }
    public string from_email { get; set; }
    public string to_email { get; set; }
    public string to_name { get; set; }
    public DateTime time_sent { get; set; }
    public DateTime event_timestamp { get; set; }
    public string campaign_name { get; set; }
    public int campaign_id { get; set; }
    public int sequence_number { get; set; }
    public string custom_subject { get; set; }
    public string custom_email_message { get; set; }
    public string sent_message_body { get; set; }
    public SentMessage sent_message { get; set; }
    public string subject { get; set; }
    public string message_id { get; set; }
    public bool is_bounced { get; set; }
    public bool is_sender_originated_bounce { get; set; }
    public string bounce_reply_message_id { get; set; }
    public string bounce_reply_email { get; set; }
    public string bounce_reply_email_preview { get; set; }
    public BounceMessage bounce_message { get; set; }
    public string secret_key { get; set; }
    public string app_url { get; set; }
    public string ui_master_inbox_link { get; set; }
    public string description { get; set; }
    public Metadata metadata { get; set; }
    public string webhook_url { get; set; }
    public int webhook_id { get; set; }
    public string webhook_name { get; set; }
    public string event_type { get; set; }
}