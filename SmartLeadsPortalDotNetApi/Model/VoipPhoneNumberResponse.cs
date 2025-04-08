using System;
using SmartLeadsPortalDotNetApi.Entities;

namespace SmartLeadsPortalDotNetApi.Model;

public class VoipPhoneNumberResponse : VoipPhoneNumber
{
    public string? UserFullName { get; set; }
}
