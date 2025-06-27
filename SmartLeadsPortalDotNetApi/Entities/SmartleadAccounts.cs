using System;

namespace SmartLeadsPortalDotNetApi.Entities;

public class SmartleadAccount
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ApiKey { get; set; }
    public bool? IsDeleted { get; set; }
}
