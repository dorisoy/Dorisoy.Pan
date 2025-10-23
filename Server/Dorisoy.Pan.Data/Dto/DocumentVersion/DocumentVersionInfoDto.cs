using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class DocumentVersionInfoDto
    {
        public Guid Id { get; set; }
        public long Size { get; set; }
        public string UserName { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsCurrentVersion { get; set; }
    }
}
