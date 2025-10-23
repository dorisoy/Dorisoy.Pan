using Dorisoy.Pan.Helper;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class SendDocumentCommand : IRequest<ServiceResponse<bool>>
    {
        public string Subject { get; set; }
        public string ToAddress { get; set; }
        public string CCAddress { get; set; }
        public string Body { get; set; }
        public Guid Id { get; set; }
    }
}
