using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.Data.Dto
{
    public class VirtualFolderInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public Guid PhysicalFolderId { get; set; }
        public bool IsShared { get; set; }
        public bool IsRestore { get; set; }
        public bool IsStarred { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<UserInfoDto> Users { get; set; }
    }
}
