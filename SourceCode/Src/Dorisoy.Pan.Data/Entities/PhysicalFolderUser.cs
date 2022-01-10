using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class PhysicalFolderUser
    {
        public Guid FolderId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("FolderId")]
        public PhysicalFolder PhysicalFolder { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
