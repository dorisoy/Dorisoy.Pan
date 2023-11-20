using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class DeletedDocumentInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailPath { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
