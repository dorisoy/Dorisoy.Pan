using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.PanClient.Data;

public class RecentActivity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    public Guid? FolderId { get; set; }

    [ForeignKey("FolderId")]
    public virtual VirtualFolder VirtualFolder { get; set; }

    public Guid? DocumentId { get; set; }

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public RecentActivityType Action { get; set; }
}
public enum RecentActivityType
{
    VIEWED,
    CREATED
}
