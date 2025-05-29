using System;

namespace SmartLeadsPortalDotNetApi.Model;

public class AddVoipPhoneNumberRequest
{
    public string? PhoneNumber { get; set; }
    public int? EmployeeId { get; set; }
}
