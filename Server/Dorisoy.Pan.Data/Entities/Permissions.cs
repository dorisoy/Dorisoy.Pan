using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Data;

public static class ApplicationClaimTypes
{
    public const string Permission = "Permission";
}

public static class RoleConstants
{
    public const string AdministratorRole = "Administrator";
    public const string BasicRole = "Basic";
    public const string DoctorRole = "Doctor";
    public const string DefaultPassword = "123Pa$$word!";
}

public static class UserConstants
{
    public const string DefaultPassword = "123Pa$$word!";
}

public class RoleClaimResponse
{
    public int Id { get; set; }
    public Guid RoleId { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public string Rmark { get; set; }
    public string Group { get; set; }
    public bool Selected { get; set; }
}


/// <summary>
/// 访问控制
/// </summary>
public class Permissions
{
    [DisplayName("用户管理")]
    [Description("用户管理访问控制")]
    public static class Users
    {
        [Description("查看用户")]
        public const string View = "Permissions.Users.View";

        [Description("添加用户")]
        public const string Create = "Permissions.Users.Create";

        [Description("编辑用户")]
        public const string Edit = "Permissions.Users.Edit";

        [Description("删除用户")]
        public const string Delete = "Permissions.Users.Delete";

        [Description("导出用户")]
        public const string Export = "Permissions.Users.Export";

        [Description("查找用户")]
        public const string Search = "Permissions.Users.Search";
    }

    [DisplayName("角色管理")]
    [Description("角色管理访问控制")]
    public static class Roles
    {
        [Description("查看角色")]
        public const string View = "Permissions.Roles.View";

        [Description("添加角色")]
        public const string Create = "Permissions.Roles.Create";

        [Description("编辑角色")]
        public const string Edit = "Permissions.Roles.Edit";

        [Description("删除角色")]
        public const string Delete = "Permissions.Roles.Delete";

        [Description("查找角色")]
        public const string Search = "Permissions.Roles.Search";
    }

    [DisplayName("权限记录")]
    [Description("角色权限访问控制")]
    public static class RoleClaims
    {
        [Description("查看角色权限")]
        public const string View = "Permissions.RoleClaims.View";

        [Description("添加角色权限")]
        public const string Create = "Permissions.RoleClaims.Create";

        [Description("编辑角色权限")]
        public const string Edit = "Permissions.RoleClaims.Edit";

        [Description("删除角色权限")]
        public const string Delete = "Permissions.RoleClaims.Delete";

        [Description("查找角色权限")]
        public const string Search = "Permissions.RoleClaims.Search";
    }
}
