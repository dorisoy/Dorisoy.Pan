using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data;

public class EmailSMTPSetting : BaseEntity
{
    public Guid Id { get; set; }
    [Required]
    public string Host { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public bool IsEnableSSL { get; set; }
    [Required]
    public int Port { get; set; }
    [Required]
    public bool IsDefault { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("ModifiedBy")]
    public virtual User ModifiedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User DeletedByUser { get; set; }
}
