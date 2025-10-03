using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.PanClient.Data;

public class VirtualFolder : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }

    public string Size { get; set; }

    public bool IsShared { get; set; } = true;

    public Guid PhysicalFolderId { get; set; }

    [ForeignKey("ParentId")]
    public virtual VirtualFolder Parent { get; set; }

    [ForeignKey("PhysicalFolderId")]
    public virtual PhysicalFolder PhysicalFolder { get; set; }



    public virtual ICollection<VirtualFolder> Children { get; set; } = new List<VirtualFolder>();
    public virtual List<VirtualFolderUser> VirtualFolderUsers { get; set; } = new List<VirtualFolderUser>();

}
