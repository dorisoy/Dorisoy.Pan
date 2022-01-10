using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class VirtualFolder : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public VirtualFolder Parent { get; set; }
        public string Size { get; set; }
        public bool IsShared { get; set; } = false;
        public Guid PhysicalFolderId { get; set; }
        [ForeignKey("PhysicalFolderId")]
        public PhysicalFolder PhysicalFolder { get; set; }
        public ICollection<VirtualFolder> Children { get; set; } = new List<VirtualFolder>();
        public List<VirtualFolderUser> VirtualFolderUsers { get; set; } = new List<VirtualFolderUser>();
    }
}
