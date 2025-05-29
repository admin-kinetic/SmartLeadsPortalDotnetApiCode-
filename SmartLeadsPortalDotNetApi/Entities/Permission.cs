using System;

namespace SmartLeadsPortalDotNetApi.Entities;


public abstract class PermissionBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class Permission : PermissionBase {
     public int Id { get; set; }
}

public class PermissionUpdate : Permission {}
public class PermissionCreate : PermissionBase { }

public class PermissionWithAssignment : Permission {
    public bool? IsAssigned { get; set; }
}