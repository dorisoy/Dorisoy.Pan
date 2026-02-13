using MediatR;
using System;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class UpdateEmailSMTPSettingCommand : IRequest<ServiceResponse<EmailSMTPSettingDto>>
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
