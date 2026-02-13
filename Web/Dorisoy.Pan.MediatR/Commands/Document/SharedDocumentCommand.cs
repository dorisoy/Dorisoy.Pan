using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class SharedDocumentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public List<Guid> Users { get; set; }
    }
}
