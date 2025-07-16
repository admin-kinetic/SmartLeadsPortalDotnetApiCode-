using Common.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.Models;

public class FetchCampaignLeadStatisticsResponse
{
    public bool hasMore { get; set; }
    public List<Datum> data { get; set; }
    public int skip { get; set; }
    public int limit { get; set; }
}


public class Datum
{
    public string lead_id { get; set; }
    public string email_lead_map_id { get; set; }
    public string from { get; set; }
    public string to { get; set; }
    public string status { get; set; }
    [JsonConverter(typeof(DateTimeFromStringConverter))]
    public DateTime created_at { get; set; }
    public int last_seq_num { get; set; }
    public List<History> history { get; set; }
}
