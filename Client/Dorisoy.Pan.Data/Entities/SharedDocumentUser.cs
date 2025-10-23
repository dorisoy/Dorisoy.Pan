using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data;

public class SharedDocumentUser
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}
