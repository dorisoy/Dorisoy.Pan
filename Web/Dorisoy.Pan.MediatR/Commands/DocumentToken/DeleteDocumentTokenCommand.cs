using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class DeleteDocumentTokenCommand : IRequest<bool>
    {
        public Guid Token { get; set; }
    }
}
