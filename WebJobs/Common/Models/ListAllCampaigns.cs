using System;

namespace Common.Models;

public class ListAllCampaignsResponse
{
    public int? id { get; set; }
    public int? user_id { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public string? status { get; set; }
    public string? name { get; set; }
    public List<string>? track_settings { get; set; }
    public SchedulerCronValue? scheduler_cron_value { get; set; }
    public int? min_time_btwn_emails { get; set; }
    public int? max_leads_per_day { get; set; }
    public string? stop_lead_settings { get; set; }
    public bool? enable_ai_esp_matching { get; set; }
    public bool? send_as_plain_text { get; set; }
    public int? follow_up_percentage { get; set; }
    public string? unsubscribe_text { get; set; }
    public object? parent_campaign_id { get; set; }
    public object? client_id { get; set; }
}

public class SchedulerCronValue
{
    public string? tz { get; set; }
    public List<int>? days { get; set; }
    public string? endHour { get; set; }
    public string? startHour { get; set; }
}
