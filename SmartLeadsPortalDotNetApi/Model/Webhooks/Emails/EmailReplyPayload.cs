namespace SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;


public class LeadCorrespondence
{
    public  string? targetLeadEmail { get; set; }
    public  string? replyReceivedFrom { get; set; }
    public  string? repliedCompanyDomain { get; set; }
}

public class ReplyMessage
{
    public  string? message_id { get; set; }
    public  string? html { get; set; }
    public  string? text { get; set; }
    public DateTime? time { get; set; }
}

public class EmailReplyPayload
{
    public  string? campaign_status { get; set; }
    public  string? stats_id { get; set; }
    public  string? sl_email_lead_id { get; set; }
    public int? sl_email_lead_map_id { get; set; }
    public  string? sl_lead_email { get; set; }
    public  string? from_email { get; set; }
    public List<object>? cc_emails { get; set; }
    public  string? subject { get; set; }
    public  string? sent_message_body { get; set; }
    public SentMessage? sent_message { get; set; }
    public  string? to_email { get; set; }
    public  string? to_name { get; set; }
    public DateTime? time_replied { get; set; }
    public DateTime? event_timestamp { get; set; }
    public ReplyMessage? reply_message { get; set; }
    public  string? reply_body { get; set; }
    public  string? message_id { get; set; }
    public  string? preview_text { get; set; }
    public  string? campaign_name { get; set; }
    public int? campaign_id { get; set; }
    public object? client_id { get; set; }
    public int? sequence_number { get; set; }
    public  string? secret_key { get; set; }
    public int? reply_category { get; set; }
    public  string? app_url { get; set; }
    public  string? ui_master_inbox_link { get; set; }
    public  string? description { get; set; }
    public Metadata? metadata { get; set; }
    public  string? promptType { get; set; }
    public LeadCorrespondence? leadCorrespondence { get; set; }
    public  string? webhook_url { get; set; }
    public int? webhook_id { get; set; }
    public  string? webhook_name { get; set; }
    public  string? event_type { get; set; }
}


