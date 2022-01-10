using MediatR;
using System;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetEmailTemplateQuery : IRequest<ServiceResponse<EmailTemplateDto>>
    {
        public Guid Id { get; set; }
    }
}
