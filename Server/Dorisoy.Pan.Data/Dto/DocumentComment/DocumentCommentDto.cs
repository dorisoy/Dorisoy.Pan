using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class DocumentCommentDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Comment { get; set; }
        public string UserName { get; set; }
    }
}
