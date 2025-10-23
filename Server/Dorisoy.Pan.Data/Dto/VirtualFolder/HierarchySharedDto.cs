using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class HierarchySharedDto
    {
        public Guid Id { get; set; }
        public bool IsParentShared { get; set; } = false;
        public bool IsChildShared { get; set; } = false;

    }
}
