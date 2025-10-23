using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data;

/// <summary>
/// 物理目录
/// </summary>
public class PhysicalFolder : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public long SystemFolderName { get; set; }
    public Guid? ParentId { get; set; }

    [ForeignKey("ParentId")]
    public virtual PhysicalFolder Parent { get; set; }

    public string Size { get; set; }

    public virtual ICollection<PhysicalFolder> Children { get; set; } = new List<PhysicalFolder>();
    public virtual List<PhysicalFolderUser> PhysicalFolderUsers { get; set; } = new List<PhysicalFolderUser>();
}
