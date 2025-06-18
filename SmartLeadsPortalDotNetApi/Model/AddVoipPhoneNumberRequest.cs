using System;

namespace SmartLeadsPortalDotNetApi.Model;

public class AddVoipPhoneNumberRequest
{
    public int? Id { get; set; }
    public string? PhoneNumber { get; set; }
    public int? EmployeeId { get; set; }
}
