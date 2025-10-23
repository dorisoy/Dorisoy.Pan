using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data;

public class Operate : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public int Order { get; set; }


    public Guid PageId { get; set; }

    [ForeignKey("PageId")]
    public virtual Page Page { get; set; }

    public string Code { get; set; }


    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("ModifiedBy")]
    public virtual User ModifiedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User DeletedByUser { get; set; }
}
