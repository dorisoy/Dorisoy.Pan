using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class LoginAuditDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime LoginTime { get; set; }
        public string RemoteIP { get; set; }
        public string Status { get; set; }
        public string Provider { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
