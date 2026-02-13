using MediatR;
using System;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class DeleteEmailSMTPSettingCommand : IRequest<ServiceResponse<EmailSMTPSettingDto>>
    {
        public Guid Id { get; set; }
    }
}
