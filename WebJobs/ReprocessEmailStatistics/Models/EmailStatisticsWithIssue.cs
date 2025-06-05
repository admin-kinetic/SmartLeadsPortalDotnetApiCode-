using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReprocessEmailStatistics.Models;

internal class EmailStatisticsWithIssue
{
    public int? AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? ApiKey { get; set; }
    public int? CampaignId { get; set; }
    public string? LeadId { get; set; }
    public string? LeadEmail { get; set; }
    public int? SequenceNumber { get; set; }
    public string? LeadName { get; set; }

}
