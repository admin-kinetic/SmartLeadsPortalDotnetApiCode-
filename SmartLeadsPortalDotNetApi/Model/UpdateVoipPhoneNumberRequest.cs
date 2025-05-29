using System;

namespace SmartLeadsPortalDotNetApi.Model;

public class UpdateVoipPhoneNumberRequest
{
    public string? PhoneNumber { get; set; }
    public int EmployeeId { get; set; }
}
