using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class PhysicalFolderUserDto
    {
        public Guid FolderId { get; set; }
        public Guid UserId { get; set; }
        public PhysicalFolderDto PhysicalFolder { get; set; }
        public UserDto User { get; set; }
    }
}
