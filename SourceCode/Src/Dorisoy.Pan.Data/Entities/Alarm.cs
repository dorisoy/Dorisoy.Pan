using System;

namespace Dorisoy.Pan.Data
{
    public class Alarm : BaseEntity
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
        public DateTime AlarmDate { get; set; }
        public Guid DocumentId { get; set; }
        public Document Document { get; set; }

    }
}
