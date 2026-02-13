using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class VirtualFolderUser : BaseEntity
    {
        public Guid FolderId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("FolderId")]
        public VirtualFolder VirtualFolder { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("CreatedBy")]
        public User CreatedByUser { get; set; }
        [ForeignKey("ModifiedBy")]
        public User ModifiedByUser { get; set; }
        [ForeignKey("DeletedBy")]
        public User DeletedByUser { get; set; }
        public bool IsStarred { get; set; }
    }
}
