using System;

namespace Common.Models;


public class FetchCampaignStatisticsByCampaignIdDatum
{
    public string lead_name { get; set; }
    public string lead_email { get; set; }
    public object lead_category { get; set; }
    public int sequence_number { get; set; }
    public string stats_id { get; set; }
    public int email_campaign_seq_id { get; set; }
    public int seq_variant_id { get; set; }
    public string email_subject { get; set; }
    public string email_message { get; set; }
    public DateTime sent_time { get; set; }
    public DateTime? open_time { get; set; }
    public DateTime? click_time { get; set; }
    public DateTime? reply_time { get; set; }
    public int open_count { get; set; }
    public int click_count { get; set; }
    public bool is_unsubscribed { get; set; }
    public bool is_bounced { get; set; }
}

public class FetchCampaignStatisticsByCampaignIdResponse
{
    public string total_stats { get; set; }
    public List<FetchCampaignStatisticsByCampaignIdDatum> data { get; set; }
    public int offset { get; set; }
    public int limit { get; set; }
}
