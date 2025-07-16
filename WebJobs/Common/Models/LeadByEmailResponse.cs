using Common.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.Models;

public class LeadByEmailResponse
{
    public string id { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string email { get; set; }
    [JsonConverter(typeof(DateTimeFromStringConverter))]
    public DateTime? created_at { get; set; }
    public object phone_number { get; set; }
    public string company_name { get; set; }
    public string website { get; set; }
    public string location { get; set; }
    public CustomFields custom_fields { get; set; }
    public object linkedin_profile { get; set; }
    public object company_url { get; set; }
    public bool? is_unsubscribed { get; set; }
    public UnsubscribedClientIdMap unsubscribed_client_id_map { get; set; }
    public List<LeadCampaignDatum> lead_campaign_data { get; set; }
}

//public class CustomFields
//{
//    public string Source { get; set; }
//    public string Currency { get; set; }
//    public string Job_ad_link { get; set; }
//    public string Personal_Note_1 { get; set; }
//    public string Personal_Note_2 { get; set; }
//    public string Role_Advertised { get; set; }
//    public string Candidate_1_cost { get; set; }
//    public string Candidate_2_cost { get; set; }
//    public string Candidate_3_cost { get; set; }
//    public string Resume_list_link { get; set; }
//    public string Landing_page_link { get; set; }
//    public string Role_requirements { get; set; }
//    public string Candidate_1_Qualification_1 { get; set; }
//    public string Candidate_1_Qualification_2 { get; set; }
//    public string Candidate_1_Qualification_3 { get; set; }
//    public string Candidate_1_Qualification_4 { get; set; }
//    public string Candidate_1_Qualification_5 { get; set; }
//    public string Candidate_2_Qualification_1 { get; set; }
//    public string Candidate_2_Qualification_2 { get; set; }
//    public string Candidate_2_Qualification_3 { get; set; }
//    public string Candidate_2_Qualification_4 { get; set; }
//    public string Candidate_2_Qualification_5 { get; set; }
//    public string Candidate_3_Qualification_1 { get; set; }
//    public string Candidate_3_Qualification_2 { get; set; }
//    public string Candidate_3_Qualification_3 { get; set; }
//    public string Candidate_3_Qualification_4 { get; set; }
//    public string Candidate_3_Qualification_5 { get; set; }
//}

public class LeadCampaignDatum
{
    public object client_id { get; set; }
    public int? campaign_id { get; set; }
    public object client_email { get; set; }
    public string campaign_name { get; set; }
    public object lead_category_id { get; set; }
    public int? campaign_lead_map_id { get; set; }
}
public class UnsubscribedClientIdMap
{
}
