using ProtoBuf;

namespace Dorisoy.PanClient.Models;

public class UserModel : ReactiveObject
{
    [Reactive] public Guid Id { get; set; }
    [Reactive] public Guid DepartmentId { get; set; }
    [Reactive] public string DepartmentName { get; set; }
    [Reactive] public DepartmentModel Department { get; set; }

    [Reactive] public Sex Sex { get; set; }

    [Reactive] public string Password { get; set; }
    [Reactive] public string RaleName { get; set; }

    [Reactive] public bool IsDeleted { get; set; }
    [Reactive] public bool IsActive { get; set; }

    [Reactive] public string ProfilePhoto { get; set; }
    [Reactive] public string Provider { get; set; }
    [Reactive] public string Address { get; set; }

    [Reactive] public DateTime CreatedDate { get; set; }
    [Reactive] public Guid CreatedBy { get; set; }

    [Reactive] public DateTime ModifiedDate { get; set; }
    [Reactive] public Guid ModifiedBy { get; set; }

    [Reactive] public DateTime DeletedDate { get; set; }
    [Reactive] public Guid DeletedBy { get; set; }

    [Reactive] public bool IsAdmin { get; set; }
    [Reactive] public string UserName { get; set; }
    [Reactive] public string NormalizedUserName { get; set; }

    [Reactive] public string Email { get; set; }
    [Reactive] public string NormalizedEmail { get; set; }
    [Reactive] public bool EmailConfirmed { get; set; }

    [Reactive] public string PasswordHash { get; set; }
    [Reactive] public string SecurityStamp { get; set; }
    [Reactive] public string ConcurrencyStamp { get; set; }

    [Reactive] public string PhoneNumber { get; set; }
    [Reactive] public bool PhoneNumberConfirmed { get; set; }
    [Reactive] public bool TwoFactorEnabled { get; set; }

    [Reactive] public DateTimeOffset? LockoutEnd { get; set; }

    [Reactive] public bool LockoutEnabled { get; set; }
    [Reactive] public int AccessFailedCount { get; set; }

    public bool IsOwner { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string RaleName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string ProfilePhoto { get; set; }
    public string Provider { get; set; }
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }
    public long Size { get; set; }
    public string IP { get; set; }
    public UserClaimDto UserClaims { get; set; } = null;
}


/// <summary>
/// 用于承载对等信息
/// </summary>
[ProtoContract]
public class OnlinUserUserModel : ReactiveObject, IProtoMessage
{
    [ProtoMember(1)]
    public Guid Id { get; set; }

    [ProtoMember(2)]
    public string UserName { get; set; }

    [ProtoMember(3)]
    public string Email { get; set; }

    [ProtoMember(4)]
    public string RaleName { get; set; }

    [ProtoMember(5)]
    public string PhoneNumber { get; set; }

    [ProtoMember(6)]
    public string Address { get; set; }

    [ProtoMember(7)]
    public string ProfilePhoto { get; set; }

    [ProtoMember(8)]
    public string Provider { get; set; }
    public long Size { get; set; }

    [ProtoMember(9)]
    public string IP { get; set; }

    [ProtoMember(10)]
    public int Port { get; }

    [Reactive] public bool IsActive { get; set; }
    [Reactive] public bool IsAdmin { get; set; }
    [Reactive] public bool Connected { get; set; }

    public UserClaimDto UserClaims { get; set; } = null;

    [Reactive] public string PushingIcon { get; set; } = "Video";
    [Reactive] public string PushingLable { get; set; } = "共享";

    /// <summary>
    /// TCP 延迟
    /// </summary>
    [Reactive] public double TcpLatency { get; set; }

    /// <summary>
    /// UDP 延迟
    /// </summary>
    [Reactive] public double UdpLatency { get; set; }
}



public class UserClaimDto
{
    public bool IsFolderCreate { get; set; } = false;
    public bool IsFileUpload { get; set; } = false;
    public bool IsDeleteFileFolder { get; set; } = false;
    public bool IsSharedFileFolder { get; set; } = false;
    public bool IsSendEmail { get; set; } = false;
    public bool IsRenameFile { get; set; } = false;
    public bool IsDownloadFile { get; set; } = false;
    public bool IsCopyFile { get; set; } = false;
    public bool IsCopyFolder { get; set; } = false;
    public bool IsMoveFile { get; set; } = false;
    public bool IsSharedLink { get; set; } = false;

    //

}

public class UserRolesResponse
{
    public List<UserRoleModel> UserRoles { get; set; } = new();
}
public class UserRoleModel : ReactiveObject
{
    [Reactive] public string Name { get; set; }
    [Reactive] public string RoleName { get; set; }
    [Reactive] public string RoleDescription { get; set; }
    [Reactive] public bool Selected { get; set; }
}
public class UpdateUserRolesRequest
{
    public Guid UserId { get; set; }
    public IList<UserRoleModel> UserRoles { get; set; }
}
public class RevokeTokenRequest
{
    public string Token { get; set; }
}
public class UserAuthDto
{
    public UserAuthDto()
    {
        BearerToken = string.Empty;
    }
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string RaleName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string BearerToken { get; set; }
    public bool IsAuthenticated { get; set; }
    public string ProfilePhoto { get; set; }
    public string IP { get; set; }
    public bool IsAdmin { get; set; }
    public List<AppClaimDto> Claims { get; set; }
}

public class AppClaimDto
{
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }
}


public class LoginModel
{
    public string UserName { get; set; } = "admin@gmail.com";
    public string Password { get; set; } = "admin@123";
    public string RemoteIp { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}


public class MessageModel
{
    public string Msg { get; set; }
}

