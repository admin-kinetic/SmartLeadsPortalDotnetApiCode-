using System;

namespace SmartLeadsPortalDotNetApi.Entities;

public class LeadClicks
{
    public int? Id { get; set; }
    public int? LeadId { get; set; }
    public int? ClickCount { get; set; }
    public DateTime LatestClickDateTime { get; set; }
}
