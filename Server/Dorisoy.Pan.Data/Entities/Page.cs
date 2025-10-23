using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dorisoy.Pan.Data.Entities;

namespace Dorisoy.Pan.Data;

public class Page : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public int Order { get; set; }


    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("ModifiedBy")]
    public virtual User ModifiedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User DeletedByUser { get; set; }

    public virtual ICollection<Operate> Actions { get; set; } = new List<Operate>();
}
