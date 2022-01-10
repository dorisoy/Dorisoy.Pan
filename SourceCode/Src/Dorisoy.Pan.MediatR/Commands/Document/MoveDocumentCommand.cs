using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class MoveDocumentCommand : IRequest<bool>
    {
        public Guid DocumentId { get; set; }
        public Guid DestinationFolderId { get; set; }
    }
}
