using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class DocumentTokenDto
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public Guid Token { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
