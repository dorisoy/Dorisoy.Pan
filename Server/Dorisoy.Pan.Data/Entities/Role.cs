using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data;

/// <summary>
/// 角色
/// </summary>
public class Role : IdentityRole<Guid>
{
    public string Description { get; set; } = "";

    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public Role() : base()
    {
        RoleClaims = new HashSet<RoleClaim>();
    }

    public Role(string roleName, string roleDescription = null) : base(roleName)
    {
        RoleClaims = new HashSet<RoleClaim>();
        Description = roleDescription;
    }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }


    [Column(TypeName = "datetime")]
    public DateTime ModifiedDate { get; set; }
    public Guid? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedDate { get; set; }
    public Guid? DeletedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("ModifiedBy")]
    public virtual User ModifiedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User DeletedByUser { get; set; }

    public bool IsSystem { get; set; }

    public bool IsDeleted { get; set; } = false;

}
