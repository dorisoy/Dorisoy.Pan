using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.PanClient.Data;

/// <summary>
/// 用户权限
/// </summary>
public class UserClaim : IdentityUserClaim<Guid>
{
    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    public Guid ActionId { get; set; }

    [ForeignKey("ActionId")]
    public virtual Operate Action { get; set; }
}
