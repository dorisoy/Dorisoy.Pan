using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dorisoy.Pan.Data;

namespace Dorisoy.Pan.Data;

/// <summary>
/// 文档模型
/// </summary>
public class Document : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid PhysicalFolderId { get; set; }

    [ForeignKey("PhysicalFolderId")]
    public virtual PhysicalFolder Folder { get; set; }

    public string Extension { get; set; }
    public string Path { get; set; }
    public long Size { get; set; }
    public string ThumbnailPath { get; set; }
    public Guid? PatienterId { get; set; }
    public bool IsAttachment { get; set; }

    public virtual List<DocumentDeleted> DocumentDeleteds { get; set; } = new List<DocumentDeleted>();

    [NotMapped]
    public string OriginalPath { get; set; }

    [NotMapped]
    public string OriginalThumbnailPath { get; set; }


    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }

    [ForeignKey("ModifiedBy")]
    public virtual User ModifiedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User DeletedByUser { get; set; }


    public virtual List<DocumentStarred> DocumentStarreds { get; set; } = new();
    public virtual ICollection<SharedDocumentUser> SharedDocumentUsers { get; set; } = new List<SharedDocumentUser>();

}
