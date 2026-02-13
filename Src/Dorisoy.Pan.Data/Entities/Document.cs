using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class Document : BaseEntity
    {
        public Guid Id { get; set; }
        public string Md5 { get; set; }
        public string Name { get; set; }
        public Guid PhysicalFolderId { get; set; }
        [ForeignKey("PhysicalFolderId")]
        public PhysicalFolder Folder { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string ThumbnailPath { get; set; }
        [ForeignKey("CreatedBy")]
        public User CreatedByUser { get; set; }
        [ForeignKey("ModifiedBy")]
        public User ModifiedByUser { get; set; }
        [ForeignKey("DeletedBy")]
        public User DeletedByUser { get; set; }
        public List<DocumentStarred> DocumentStarreds { get; set; } = new List<DocumentStarred>();
        public List<DocumentDeleted> DocumentDeleteds { get; set; } = new List<DocumentDeleted>();
        public ICollection<SharedDocumentUser> SharedDocumentUsers { get; set; } = new List<SharedDocumentUser>();
        [NotMapped]
        public string OriginalPath { get; set; }
        [NotMapped]
        public string OriginalThumbnailPath { get; set; }
    }
}
