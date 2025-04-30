using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models;

public class MessageHistoryByLeadsResponse
{
    public List<History> history { get; set; }
    public string from { get; set; }
    public string to { get; set; }
}

public class ClickDetails
{
}

public class History
{
    public string stats_id { get; set; }
    public string type { get; set; }
    public string message_id { get; set; }
    public DateTime? time { get; set; }
    public string email_body { get; set; }
    public string subject { get; set; }
    public string email_seq_number { get; set; }
    public int? open_count { get; set; }
    public int? click_count { get; set; }
    public ClickDetails click_details { get; set; }
    public List<object> cc { get; set; }
}

public class LeadEmailHistory : History
{
    public string? email { get; set; }
}
    
