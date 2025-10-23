using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Role = Dorisoy.Pan.Data.Role;

namespace Dorisoy.Pan.Services;

public static class ClaimsHelper
{
    public static void GetAllPermissions(this List<RoleClaimResponse> allPermissions)
    {
        var modules = typeof(Permissions).GetNestedTypes();

        foreach (var module in modules)
        {
            var moduleName = string.Empty;
            var moduleDescription = string.Empty;

            if (module.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                .FirstOrDefault() is DisplayNameAttribute displayNameAttribute)
                moduleName = displayNameAttribute.DisplayName;

            if (module.GetCustomAttributes(typeof(DescriptionAttribute), true)
                .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                moduleDescription = descriptionAttribute.Description;

            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var fi in fields)
            {
                var propertyValue = fi.GetValue(null);

                var fiDescription = "";
                if (fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() is DescriptionAttribute fidescriptionAttribute)
                    fiDescription = fidescriptionAttribute.Description;


                if (propertyValue is not null)
                {
                    var pr = new RoleClaimResponse
                    {
                        Value = propertyValue.ToString(),
                        Type = ApplicationClaimTypes.Permission,
                        Group = moduleName,
                        Description = moduleDescription,
                        Rmark = fiDescription
                    };
                    allPermissions.Add(pr);
                }
            }
        }
    }

    public static async Task<IdentityResult> AddPermissionClaim(this RoleManager<Role> roleManager, Role role, string permission)
    {
        var allClaims = await roleManager.GetClaimsAsync(role);
        if (!allClaims.Any(a => a.Type == ApplicationClaimTypes.Permission && a.Value == permission))
        {
            return await roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission));
        }

        return IdentityResult.Failed();
    }
}
