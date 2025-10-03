using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dorisoy.PanClient.Data;

namespace Dorisoy.PanClient.Data;

public abstract class BaseEntity
{

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }


    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }
    public Guid? ModifiedBy { get; set; }


    [Column(TypeName = "datetime")]
    public DateTime? DeletedDate { get; set; }
    public Guid? DeletedBy { get; set; }

    public bool IsDeleted { get; set; } = false;
}
