using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class DeletedVirtualFolderDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsShared { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
