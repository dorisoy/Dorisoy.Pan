using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Data
{
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


    public static class StringExtensions
    {
        /// <summary>
        /// 判断字符串是否为Null、空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNull(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }


        /// <summary>
        /// 判断字符串是否不为Null、空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool NotNull(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }
        /// <summary>
        /// 判断字符串是否不为Null、空
        /// </summary>
        /// <param name="s"></param>
        /// <param name="action">不为null或空执行回调</param>
        /// <returns></returns>
        public static bool NotNullIf(this string s, Action action)
        {
            var res = !string.IsNullOrWhiteSpace(s);
            if (res) action();
            return res;
        }

        /// <summary>
        /// 判断字符串是否为Null、空，如果是则返回字符串本身，如果不是则返回默认值
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string IsNull(this string s, string defaultValue)
        {
            return s.IsNull() ? defaultValue : s;
        }

        /// <summary>
        /// 与字符串进行比较，忽略大小写
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string s, string value)
        {
            return s.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 匹配字符串结尾，忽略大小写
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EndsWithIgnoreCase(this string s, string value)
        {
            return s.EndsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 匹配字符串开头，忽略大小写
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(this string s, string value)
        {
            return s.StartsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 首字母转小写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FirstCharToLower(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            string str = s.First().ToString().ToLower() + s.Substring(1);
            return str;
        }

        /// <summary>
        /// 首字母转大写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            string str = s.First().ToString().ToUpper() + s.Substring(1);
            return str;
        }

        /// <summary>
        /// 转为Base64，UTF-8格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToBase64(this string s)
        {
            return s.ToBase64(Encoding.UTF8);
        }

        /// <summary>
        /// 转为Base64
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToBase64(this string s, Encoding encoding)
        {
            if (s.IsNull())
                return string.Empty;

            var bytes = encoding.GetBytes(s);
            return bytes.ToBase64();
        }

        /// <summary>
        /// 转换为Base64
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToBase64(this byte[] bytes)
        {
            if (bytes == null)
                return null;

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64解析
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FromBase64(this string s)
        {
            byte[] data = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// 构建Where根条件字符串
        /// </summary>
        /// <param name="whereStr"></param>
        /// <param name="isBraces"></param>
        /// <returns></returns>
        public static string WhereRootBuildStr(this string whereStr, bool isBraces = false)
        {
            return whereStr.NotNull() ? $" WHERE {whereStr.Trim()} " : String.Empty;
        }
    }
}