using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.PanClient.Data
{
    public class DocumentStarred
    {
        public Guid DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
