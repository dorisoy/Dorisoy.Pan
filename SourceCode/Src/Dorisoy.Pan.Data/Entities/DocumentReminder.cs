using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class DocumentReminder: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        public DocumentFrequency Frequency { get; set; }
        public string Message { get; set; }
        [ForeignKey("DocumentId")]
        public Document Document { get; set; }
    }
}
