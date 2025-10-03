using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dorisoy.PanClient.Data;

namespace Dorisoy.PanClient.Data;

/// <summary>
/// 删除文档实体
/// </summary>
public class DocumentDeleted : BaseEntity
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }

    public virtual Document Document { get; set; }
    public virtual User User { get; set; }


    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("ModifiedBy")]
    public virtual User ModifiedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User DeletedByUser { get; set; }
}
