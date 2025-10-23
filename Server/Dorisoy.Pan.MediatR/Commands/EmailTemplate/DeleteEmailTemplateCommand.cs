using MediatR;
using System;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class DeleteEmailTemplateCommand : IRequest<ServiceResponse<bool>>
    {
        public Guid Id { get; set; }
    }
}
