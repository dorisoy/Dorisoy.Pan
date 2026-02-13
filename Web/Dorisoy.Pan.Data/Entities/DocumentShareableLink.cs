using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class DocumentShareableLink
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public Document Document { get; set; }
        [Column(TypeName ="datetime")]
        public DateTime? LinkExpiryTime { get; set; }
        public string Password { get; set; }
        public string LinkCode { get; set; }
        public bool IsLinkExpired { get; set; }
        public bool IsAllowDownload { get; set; }
    }
}
