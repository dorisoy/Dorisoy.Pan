using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class RecentActivity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public Guid? FolderId { get; set; }
        [ForeignKey("FolderId")]
        public VirtualFolder VirtualFolder { get; set; }
        public Guid? DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public Document Document { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        public RecentActivityType Action { get; set; }
    }
    public enum RecentActivityType
    {
        VIEWED,
        CREATED
    }
}
