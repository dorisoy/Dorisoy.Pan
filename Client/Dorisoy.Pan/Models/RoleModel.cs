namespace Dorisoy.Pan.Models;

/// <summary>
/// 角色
/// </summary>
public class RoleModel : BaseModel
{
    public Guid Id { get; set; }
    [Reactive] public string Name { get; set; }
    [Reactive] public string NormalizedName { get; set; }
    [Reactive] public string Description { get; set; } = "";
    [Reactive] public bool IsSystem { get; set; }
    [Reactive] public List<RoleClaimModel> RoleClaims { get; set; }
}

public class PermissionResponse : ReactiveObject
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
    [Reactive] public List<RoleClaimResponseModel> RoleClaims { get; set; }
}

public class PermissionRequest : ReactiveObject
{
    public Guid RoleId { get; set; }
    [Reactive] public IList<RoleClaimResponseModel> RoleClaims { get; set; }
}

public class PermissionRoleClaim : ReactiveObject
{
    public Guid Id { get; set; }
    [Reactive] public string Group { get; set; }
    [Reactive] public int SelectCount { get; set; }
    [Reactive] public string BadgeData { get; set; }
    [Reactive] public int Count { get; set; }
    [Reactive] public List<RoleClaimResponseModel> AllRoleClaimsInGroup { get; set; }
}

public class RoleClaimResponseModel : ReactiveObject
{
    public int Id { get; set; }
    public Guid RoleId { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public string Rmark { get; set; }
    public string Group { get; set; }
    [Reactive] public bool Selected { get; set; }
}
