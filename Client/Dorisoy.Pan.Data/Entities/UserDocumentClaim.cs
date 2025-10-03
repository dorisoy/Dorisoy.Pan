using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.PanClient.Data;


public class UserDocumentClaim : BaseEntity
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

}
