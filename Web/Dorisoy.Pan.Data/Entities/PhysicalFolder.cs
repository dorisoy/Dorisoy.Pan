using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class PhysicalFolder : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SystemFolderName { get; set; }
        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public PhysicalFolder Parent { get; set; }
        public string Size { get; set; }
        public ICollection<PhysicalFolder> Children { get; set; } = new List<PhysicalFolder>();
        public List<PhysicalFolderUser> PhysicalFolderUsers { get; set; } = new List<PhysicalFolderUser>();
    }
}
