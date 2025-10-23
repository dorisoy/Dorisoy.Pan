using System;
using System.ComponentModel;


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

    [DisplayName("录制管理")]
    [Description("用户视频管理访问控制")]
    public static class Videos
    {
        [Description("预览视频")]
        public const string View = "Permissions.Videos.View";

        [Description("预览图片")]
        public const string ViewImage = "Permissions.Image.View";

        [Description("添加标注")]
        public const string Create = "Permissions.Remarks.Create";

        [Description("添加患者")]
        public const string CreatePatient = "Permissions.Patients.Create";

        [Description("编辑患者")]
        public const string Edit = "Permissions.Patients.Edit";

        [Description("删除患者")]
        public const string Delete = "Permissions.Patients.Delete";
    }

    [DisplayName("文档管理")]
    [Description("用户文档存储访问控制")]
    public static class Documents
    {
        [Description("查看文档")]
        public const string View = "Permissions.Documents.View";

        [Description("添加文档")]
        public const string Create = "Permissions.Documents.Create";

        [Description("编辑文档")]
        public const string Edit = "Permissions.Documents.Edit";

        [Description("删除文档")]
        public const string Delete = "Permissions.Documents.Delete";
    }

    //[DisplayName("协作")]
    //[Description("用户示教协作访问控制")]
    //public static class Lobbies
    //{
    //    [Description("查看小组")]
    //    public const string View = "Permissions.Lobbies.View";

    //    [Description("添加小组")]
    //    public const string Create = "Permissions.Lobbies.Create";

    //    [Description("编辑小组")]
    //    public const string Edit = "Permissions.Lobbies.Edit";

    //    [Description("删除小组")]
    //    public const string Delete = "Permissions.Lobbies.Delete";
    //}

    [DisplayName("配置")]
    [Description("系统配置访问控制")]
    public static class Settings
    {
        [Description("查看配置")]
        public const string View = "Permissions.Settings.View";

        [Description("更改配置")]
        public const string Edit = "Permissions.Settings.Edit";
    }
}
