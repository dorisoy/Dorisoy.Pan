using Dorisoy.Pan.Data.Dto;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.Data
{
    public class RecentActivityDto
    {
        public Guid Id { get; set; }
        public Guid? FolderId { get; set; }
        public string Name { get; set; }
        public Guid? DocumentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public RecentActivityType Action { get; set; }
        public bool IsShared { get; set; } = false;
        public string ThumbnailPath { get; set; }
        public DocumentDto Document { get; set; }
        public List<UserInfoDto> Users { get; set; } = new List<UserInfoDto>();
        public User CreatedByUser { get; set; }
        public List<Guid> DeletedUserIds { get; set; }

    }
}
