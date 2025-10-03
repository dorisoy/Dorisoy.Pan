using System;
using System.ComponentModel.DataAnnotations.Schema;
using Dorisoy.PanClient.Data;

namespace Dorisoy.PanClient.Data;

/// <summary>
/// 用户物理目录
/// </summary>
public class PhysicalFolderUser
{
    public Guid FolderId { get; set; }
    public Guid UserId { get; set; }


    [ForeignKey("FolderId")]
    public virtual PhysicalFolder PhysicalFolder { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}
