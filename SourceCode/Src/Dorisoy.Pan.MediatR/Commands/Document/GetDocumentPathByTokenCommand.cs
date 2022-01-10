using Dorisoy.Pan.Helper;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class GetDocumentPathByTokenCommand : IRequest<ServiceResponse<string>>
    {
        public Guid Id { get; set; }
        public Guid Token { get; set; }
        public bool IsVersion { get; set; }
    }
}
