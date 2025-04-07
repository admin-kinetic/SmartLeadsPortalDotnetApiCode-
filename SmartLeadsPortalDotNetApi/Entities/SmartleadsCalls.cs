using System;

namespace SmartLeadsPortalDotNetApi.Entities;

public class SmartleadsCalls
{
    public int Id { get; set; }
    public string? User { get; set; }
    public string? ProspectName { get; set; }
    public string? ProspectPhoneNumber { get; set; }
    public string? DateTime { get; set; }
    public string? State { get; set; }
    public string? Duration { get; set; }
    public string? Purpose { get; set; }
    public string? Disposition { get; set; }
    public string? Notes { get; set; }
}
