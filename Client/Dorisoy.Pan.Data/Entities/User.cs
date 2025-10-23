using Microsoft.AspNetCore.Identity;
using Dorisoy.Pan.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data;

/// <summary>
/// 用户实体
/// </summary>
public class User : IdentityUser<Guid>
{
    public string RaleName { get; set; }
    public Sex Sex { get; set; }
    public bool IsActive { get; set; }
    public string ProfilePhoto { get; set; }
    public string Provider { get; set; }
    public string Address { get; set; }
    public Guid? DepartmentId { get; set; }

    public virtual ICollection<PhysicalFolderUser> Folders { get; set; } = new List<PhysicalFolderUser>();
    public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<UserDocumentClaim> UserDocumentClaims { get; set; } = new List<UserDocumentClaim>();

    public bool IsAdmin { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ModifiedDate { get; set; }
    public Guid? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedDate { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}
