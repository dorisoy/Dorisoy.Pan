using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class DocumentUserPermission : BaseEntity
    {
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("DocumentId")]
        public Document Document { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }

}
