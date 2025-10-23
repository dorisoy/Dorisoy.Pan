using Microsoft.AspNetCore.Identity;
using System;

namespace Dorisoy.Pan.Data
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public class UserRole : IdentityUserRole<Guid>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
