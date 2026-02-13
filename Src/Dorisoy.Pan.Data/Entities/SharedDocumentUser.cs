using System;

namespace Dorisoy.Pan.Data
{
    public class SharedDocumentUser
    {
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }
        public Document Document { get; set; }
        public User User { get; set; }
    }
}
