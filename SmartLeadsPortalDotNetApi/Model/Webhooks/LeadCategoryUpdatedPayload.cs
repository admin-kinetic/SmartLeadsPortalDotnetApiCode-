namespace SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;

public class LeadCategoryUpdatedPayload
{
    public string campaign_status { get; set; }
    public int client_id { get; set; }
    public long lead_id { get; set; }
    public string lead_email { get; set; }
    public string lead_name { get; set; }
    public LeadData lead_data { get; set; }
    public string category { get; set; }
    public int lead_category_id { get; set; }
    public DateTime event_timestamp { get; set; }
    public LeadCategory lead_category { get; set; }
    public int current_sequence { get; set; }
    public string campaign_name { get; set; }
    public int campaign_id { get; set; }
    public string from { get; set; }
    public string from_email { get; set; }
    public string to { get; set; }
    public string to_email { get; set; }
    public string to_name { get; set; }
    public List<History> history { get; set; }
    public LastReply lastReply { get; set; }
    public LastReply last_reply { get; set; }
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


public class Category
{
    public string name { get; set; }
    public object sentiment_type { get; set; }
}

public class CustomFields
{
}

public class History
{
    public string stats_id { get; set; }
    public string type { get; set; }
    public string message_id { get; set; }
    public DateTime time { get; set; }
    public string email_body { get; set; }
    public string subject { get; set; }
    public List<object> cc_emails { get; set; }
}

public class LastReply
{
    public string stats_id { get; set; }
    public string type { get; set; }
    public string message_id { get; set; }
    public DateTime time { get; set; }
    public string email_body { get; set; }
    public List<object> cc_emails { get; set; }
}

public class LastReply2
{
    public string stats_id { get; set; }
    public string type { get; set; }
    public string message_id { get; set; }
    public DateTime time { get; set; }
    public string email_body { get; set; }
    public List<object> cc_emails { get; set; }
}

public class LeadCategory
{
    public object old_id { get; set; }
    public object old_name { get; set; }
    public int new_id { get; set; }
    public string new_name { get; set; }
}

public class LeadData
{
    public string email { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public object phone_number { get; set; }
    public string company_name { get; set; }
    public object website { get; set; }
    public object location { get; set; }
    public CustomFields custom_fields { get; set; }
    public object linkedin_profile { get; set; }
    public object company_url { get; set; }
    public Category category { get; set; }
}
