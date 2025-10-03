using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.PanClient.Data;

public class DocumentVersion: BaseEntity
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string Path { get; set; }
    public string Message { get; set; }
    public long Size { get; set; }

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("ModifiedBy")]
    public virtual User ModifiedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User DeletedByUser { get; set; }
}
