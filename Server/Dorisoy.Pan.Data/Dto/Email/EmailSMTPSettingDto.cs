using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class EmailSMTPSettingDto
    {
        public Guid Id { get; set; }
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsEnableSSL { get; set; }
        public int Port { get; set; }
        public bool IsDefault { get; set; }
    }
}
