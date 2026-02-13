using MediatR;
using System;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class UpdateEmailTemplateCommand : IRequest<ServiceResponse<EmailTemplateDto>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
