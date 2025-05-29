using System;

namespace SmartLeadsPortalDotNetApi.Entities;

public class SavedTableView
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string? TableName { get; set; }
    public string? ViewName { get; set; }
    public string? ViewFilters { get; set; }
    public int OwnerId { get; set; }
    public int Sharing { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public int ModifiedBy { get; set; }
}
