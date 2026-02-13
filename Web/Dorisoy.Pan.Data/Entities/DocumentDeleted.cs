using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class DocumentDeleted : BaseEntity
    {
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }
        public Document Document { get; set; }
        public User User { get; set; }
        [ForeignKey("CreatedBy")]
        public User CreatedByUser { get; set; }
        [ForeignKey("ModifiedBy")]
        public User ModifiedByUser { get; set; }
        [ForeignKey("DeletedBy")]
        public User DeletedByUser { get; set; }
    }
}
