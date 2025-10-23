using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data;

public class DocumentAuditTrail : BaseEntity
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }


    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; }

    public string Comment { get; set; }
}
