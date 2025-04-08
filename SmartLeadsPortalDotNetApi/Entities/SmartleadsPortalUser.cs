using System;

namespace SmartLeadsPortalDotNetApi.Entities;

public class SmartleadsPortalUser
{
    public int EmployeeId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
