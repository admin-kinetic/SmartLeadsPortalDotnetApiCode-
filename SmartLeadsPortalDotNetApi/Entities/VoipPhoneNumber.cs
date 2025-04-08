using System;

namespace SmartLeadsPortalDotNetApi.Entities;

public class VoipPhoneNumber
{
    public int Id { get; set; }
    public Guid GuId { get; set; }
    public string? PhoneNumber { get; set; }
    public int? EmployeeId { get; set; }
}
