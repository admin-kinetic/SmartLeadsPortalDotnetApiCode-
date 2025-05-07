namespace SmartLeadsPortalDotNetApi.Entities;

public abstract class RoleBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class Role: RoleBase {
    public int Id { get; set; }
}

public class RoleWithPermission : Role {
    public List<Permission>? Permissions { get; set; }
}

public class RoleUpdate : Role {}

public class RoleCreate : RoleBase { }
