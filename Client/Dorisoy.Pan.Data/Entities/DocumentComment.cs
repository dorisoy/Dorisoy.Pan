using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dorisoy.PanClient.Data;

namespace Dorisoy.PanClient.Data;

public class DocumentComment: BaseEntity
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }

    public string Comment { get; set; }

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("ModifiedBy")]
    public virtual User ModifiedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User DeletedByUser { get; set; }

}
