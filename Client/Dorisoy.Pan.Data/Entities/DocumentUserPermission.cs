using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.PanClient.Data
{
    public class DocumentUserPermission : BaseEntity
    {
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

}
