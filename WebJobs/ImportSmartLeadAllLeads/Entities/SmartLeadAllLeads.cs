using System;

namespace SmartLeadsAllLeadsToPortal.Entities;

public class SmartLeadAllLeads
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public int LeadId { get; set; }
    public int? CampaignId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CompanyName { get; set; }
    public string? LeadStatus { get; set; }
    public string? BDR { get; set; }
    public string? CreatedBy { get; set; }
    public string? QABy { get; set; }
}
