using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class DocumentComment: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string Comment { get; set; }
        [ForeignKey("DocumentId")]
        public Document Document { get; set; }
        [ForeignKey("CreatedBy")]
        public User CreatedByUser { get; set; }
        [ForeignKey("ModifiedBy")]
        public User ModifiedByUser { get; set; }
        [ForeignKey("DeletedBy")]
        public User DeletedByUser { get; set; }
    }
}
