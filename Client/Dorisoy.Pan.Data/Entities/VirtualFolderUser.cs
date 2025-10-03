using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dorisoy.PanClient.Data;

namespace Dorisoy.PanClient.Data;

public class VirtualFolderUser : BaseEntity
{
    public Guid Id { get; set; }

    public Guid FolderId { get; set; }

    public Guid UserId { get; set; }

    [ForeignKey("FolderId")]
    public virtual VirtualFolder VirtualFolder { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("ModifiedBy")]
    public virtual User ModifiedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User DeletedByUser { get; set; }

    public bool IsStarred { get; set; }
}
