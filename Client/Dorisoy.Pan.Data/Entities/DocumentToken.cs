using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.PanClient.Data
{
    public class DocumentToken
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public Guid Token { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        public Guid? DocumentVersionId { get; set; }
    }
}
