using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class HierarchyFolder
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public long SystemFolderName { get; set; }
        public Guid? ParentId { get; set; }
        public int Level { get; set; }
        public Guid PhysicalFolderId { get; set; }
        [NotMapped]
        public string Path { get; set; }
        public bool IsShared { get; set; }
        [NotMapped]
        public List<HierarchyFolder> Children { get; set; } = new List<HierarchyFolder>();
    }
}
