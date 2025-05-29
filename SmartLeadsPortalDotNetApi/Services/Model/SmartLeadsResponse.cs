namespace SmartLeadsPortalDotNetApi.Services.Model
{
    public class SmartLeadsResponse
    {
        public int? total_leads { get; set; }
        public List<SmartLeadsData>? data { get; set; }
        public int? offset { get; set; }
        public int? limit { get; set; }

    }

    public class SmartLeadsData
    {
        public string? campaign_lead_map_id { get; set; }
        public int? lead_category_id { get; set; }
        public string? status { get; set; }
        public string? created_at { get; set; }
        public Leads? lead { get; set; }
    }

    public class Leads
    {
        public int? id { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? email { get; set; }
        public string? phone_number { get; set; }
        public string? company_name { get; set; }
        public string? website { get; set; }
        public string? location { get; set; }
        public LeadCustomFields? custom_fields { get; set; }
        public string? linkedin_profile { get; set; }
        public string? company_url { get; set; }
        public bool? is_unsubscribed { get; set; }
        public object? unsubscribed_client_id_map { get; set; }
    }

    public class LeadCustomFields
    {
        public string? Source { get; set; }
        public string? Currency { get; set; }
        public string? Personal_Note_1 { get; set; }
        public string? Personal_Note_2 { get; set; }
        public string? Role_Advertised { get; set; }
        public string? Candidate_1_cost { get; set; }
        public string? Candidate_2_cost { get; set; }
        public string? Candidate_3_cost { get; set; }
        public string? Landing_page_link { get; set; }
        public string? Candidate_1_Qualification_1 { get; set; }
        public string? Candidate_1_Qualification_2 { get; set; }
        public string? Candidate_1_Qualification_3 { get; set; }
        public string? Candidate_1_Qualification_4 { get; set; }
        public string? Candidate_1_Qualification_5 { get; set; }
        public string? Candidate_2_Qualification_1 { get; set; }
        public string? Candidate_2_Qualification_2 { get; set; }
        public string? Candidate_2_Qualification_3 { get; set; }
        public string? Candidate_2_Qualification_4 { get; set; }
        public string? Candidate_2_Qualification_5 { get; set; }
        public string? Candidate_3_Qualification_1 { get; set; }
    }

    public class SmartLeadsAllLeadsResponse
    {
        public List<SmartLeadsAllLeadsData>? data { get; set; }
        public int? skip { get; set; }
        public int? limit { get; set; }
        public bool? hasMore { get; set; }
    }

    public class SmartLeadsAllLeadsData
    {
        public string? id { get; set; }
        public string? email { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? company_name { get; set; }
        public string? website { get; set; }
        public string? company_url { get; set; }
        public string? phone_number { get; set; }
        public string? location { get; set; }
        public LeadCustomFields? custom_fields { get; set; }
        public string? linkedin_profile { get; set; }
        public string? created_at { get; set; }
        public int? user_id { get; set; }
        public List<Campaigns>? campaigns { get; set; }

    }

    public class Campaigns
    {
        public int? campaign_id { get; set; }
        public string? lead_status { get; set; }
        public string? campaign_name { get; set; }
        public string? lead_added_at { get; set; }
        public string? campaign_status { get; set; }
        public int? email_lead_map_id { get; set; }
        public int? lead_last_seq_number { get; set; }
    }

    public class SmartLeadsCampaignResponse
    {
        public List<SmartLeadsCampaign>? Campaigns { get; set; }
    }

    public class SmartLeadsCampaign
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string? created_at { get; set; }
        public string? updated_at { get; set; }
        public string? status { get; set; }
        public string? name { get; set; }
        public List<string>? track_settings { get; set; }
        public SchedulerCronValue? scheduler_cron_value { get; set; }
        public int? min_time_btwn_emails { get; set; }
        public int? max_leads_per_day { get; set; }
        public string? stop_lead_settings { get; set; }
        public bool? enable_ai_esp_matching { get; set; }
        public bool? send_as_plain_text { get; set; }
        public int follow_up_percentage { get; set; }
        public string? unsubscribe_text { get; set; }
        public int? parent_campaign_id { get; set; }
        public int? client_id { get; set; }
    }

    public class SchedulerCronValue
    {
        public string? tz { get; set; }
        public List<int>? days { get; set; }
        public string? endHour { get; set; }
        public string? startHour { get; set; }
    }

    public class CampaignAnalyticsResponse
    {
        public bool? ok { get; set; }
        public List<CampaignAnalyticsData>? data { get; set; }
    }
    public class CampaignAnalyticsData
    {
        public int? email_campaign_seq_id { get; set; }
        public int? sent_count { get; set; }
        public int? skipped_count { get; set; }
        public int? open_count { get; set; }
        public int? click_count { get; set; }
        public int? reply_count { get; set; }
        public int? bounce_count { get; set; }
        public int? unsubscribed_count { get; set; }
        public int? failed_count { get; set; }
        public int? stopped_count { get; set; }
        public int? ln_connection_req_pending_count { get; set; }
        public int? ln_connection_req_accepted_count { get; set; }
        public int? ln_connection_req_skipped_sent_msg_count { get; set; }
        public int? positive_reply_count { get; set; }
    }

    public class CampaignAnalyticsDateRange
    {
        public int? id { get; set; }
        public int? user_id { get; set; }
        public string? created_at { get; set; }
        public string? status { get; set; }
        public string? name { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public string? sent_count { get; set; }
        public string? unique_sent_count { get; set; }
        public string? open_count { get; set; }
        public string? unique_open_count { get; set; }
        public string? click_count { get; set; }
        public string? unique_click_count { get; set; }
        public string? reply_count { get; set; }
        public string? block_count { get; set; }
        public string? total_count { get; set; }
        public string? drafted_count { get; set; }
        public string? bounce_count { get; set; }
        public string? unsubscribed_count { get; set; }
    }

    public class CampaignStatisticsResponse
    {
        public string? total_stats { get; set; }
        public List<CampaignStatisticsData>? data { get; set; }
        public int? offset { get; set; }
        public int? limit { get; set; }
    }

    public class CampaignStatisticsData
    {
        public string? lead_name { get; set; }
        public string? lead_email { get; set; }
        public int? lead_category { get; set; }
        public int? sequence_number { get; set; }
        public int? email_campaign_seq_id { get; set; }
        public int? seq_variant_id { get; set; }
        public string? email_subject { get; set; }
        public string? email_message { get; set; }
        public string? sent_time { get; set; }
        public string? open_time { get; set; }
        public string? click_time { get; set; }
        public string? reply_time { get; set; }
        public int? open_count { get; set; }
        public int? click_count { get; set; }
        public bool? is_unsubscribed { get; set; }
        public bool? is_bounced { get; set; }
    }

    public class CampaignAnalytics
    {
        public int? id { get; set; }
        public int? user_id { get; set; }
        public string? created_at { get; set; }
        public string? status { get; set; }
        public string? name { get; set; }
        public string? sent_count { get; set; }
        public string? open_count { get; set; }
        public string? click_count { get; set; }
        public string? reply_count { get; set; }
        public string? block_count { get; set; }
        public string? total_count { get; set; }
        public string? sequence_count { get; set; }
        public string? drafted_count { get; set; }

        public List<Tag>? tags { get; set; }
        public string? unique_sent_count { get; set; }
        public string? unique_open_count { get; set; }
        public string? unique_click_count { get; set; }
        public string? client_id { get; set; }
        public string? bounce_count { get; set; }
        public string? parent_campaign_id { get; set; }
        public string? unsubscribed_count { get; set; }
        public CampaignLeadStatsData? campaign_lead_stats { get; set; }
        public string? team_member_id { get; set; }
        public bool? send_as_plain_text { get; set; }
        public string? client_name { get; set; }
        public string? client_email { get; set; }
        public string? client_company_name { get; set; }
    }

    public class Tag
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? color { get; set; }
    }
    
    public class CampaignLeadStatsData
    {
        public int? total { get; set; }
        public int? paused { get; set; }
        public int? blocked { get; set; }
        public int? revenue { get; set; }
        public int? stopped { get; set; }
        public int? completed { get; set; }
        public int? inprogress { get; set; }
        public int? interested { get; set; }
        public int? notStarted { get; set; }
    }
}
