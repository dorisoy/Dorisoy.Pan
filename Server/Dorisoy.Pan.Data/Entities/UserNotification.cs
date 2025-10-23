using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data;

public class UserNotification
{
    public Guid Id { get; set; }
    public ActionEnum Action { get; set; }
    public Guid? DocumentId { get; set; }

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; }

    public Guid? FolderId { get; set; }

    [ForeignKey("FolderId")]
    public virtual VirtualFolder VirtualFolder { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }
    public Guid ToUserId { get; set; } 
    public Guid FromUserId { get; set; }
    public bool IsRead { get; set; } = false;

    public NotificationStatusEnum Status { get; set; } = NotificationStatusEnum.InQueue;
    public string ErrorMsg { get; set; }
}
