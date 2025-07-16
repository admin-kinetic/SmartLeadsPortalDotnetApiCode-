using Common.Converters;
using System;
using System.Text.Json.Serialization;

namespace Common.Models;

public class FetchAllLeadsFromEntireAccountResponse
{
    public List<AllLeadsFromEntireAccountDatum> data { get; set; }
    public int skip { get; set; }
    public int limit { get; set; }
    public bool hasMore { get; set; }
}


public class Campaign
{
    public int campaign_id { get; set; }
    public string lead_status { get; set; }
    public string campaign_name { get; set; }
    [JsonConverter(typeof(DateTimeFromStringConverter))]
    public DateTime? lead_added_at { get; set; }
    public string campaign_status { get; set; }
    public int email_lead_map_id { get; set; }
    public int lead_last_seq_number { get; set; }
}

public class CustomFields
{
    public string Location { get; set; }
    public string BDR { get; set; }
    public string Created_by { get; set; }
    public string QA_by { get; set; }
    public string Source { get; set; }
    public string Weekday { get; set; }
    public string Currency { get; set; }
    public string Personal_Note_1 { get; set; }
    public string Personal_Note_2 { get; set; }
    public string Role_Advertised { get; set; }
    public string Candidate_1_Cost { get; set; }
    public string Candidate_2_Cost { get; set; }
    public string Candidate_3_Cost { get; set; }
    public string Landing_page_link { get; set; }
    public string Role_Requirements { get; set; }
    public string Candidate_1_Qualification_1 { get; set; }
    public string Candidate_1_Qualification_2 { get; set; }
    public string Candidate_1_Qualification_3 { get; set; }
    public string Candidate_1_Qualification_4 { get; set; }
    public string Candidate_1_Qualification_5 { get; set; }
    public string Candidate_2_Qualification_1 { get; set; }
    public string Candidate_2_Qualification_2 { get; set; }
    public string Candidate_2_Qualification_3 { get; set; }
    public string Candidate_2_Qualification_4 { get; set; }
    public string Candidate_2_Qualification_5 { get; set; }
    public string Candidate_3_Qualification_1 { get; set; }
    public string Candidate_3_Qualification_2 { get; set; }
    public string Candidate_3_Qualification_3 { get; set; }
    public string Candidate_3_Qualification_4 { get; set; }
    public string Candidate_3_Qualification_5 { get; set; }
}

public class AllLeadsFromEntireAccountDatum
{
    public string id { get; set; }
    public string email { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string company_name { get; set; }
    public string website { get; set; }
    public object company_url { get; set; }
    public string phone_number { get; set; }
    public string? location { get; set; }
    public CustomFields custom_fields { get; set; }
    public object linkedin_profile { get; set; }
    [JsonConverter(typeof(DateTimeFromStringConverter))]
    public DateTime created_at { get; set; }
    public int user_id { get; set; }
    public List<Campaign> campaigns { get; set; }
}

public class Root
{
    public List<AllLeadsFromEntireAccountDatum> data { get; set; }
    public int skip { get; set; }
    public int limit { get; set; }
    public bool hasMore { get; set; }
}


