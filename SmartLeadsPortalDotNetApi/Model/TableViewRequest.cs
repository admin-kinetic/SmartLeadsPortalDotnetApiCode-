using System;

namespace SmartLeadsPortalDotNetApi.Model;

public class TableViewRequest
{
    public List<Filter>? Filters { get; set; }
    public string? TableName { get; set; }
    public string? ViewName { get; set; }
    public bool? IsDefault { get; set; }
}
