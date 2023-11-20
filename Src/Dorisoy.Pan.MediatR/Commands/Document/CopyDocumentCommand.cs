using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class CopyDocumentCommand : IRequest<bool>
    {
        public Guid DocumentId { get; set; }
        public Guid DestinationFolderId { get; set; }
    }
}
