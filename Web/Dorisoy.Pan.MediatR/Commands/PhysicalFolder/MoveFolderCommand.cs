using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class MoveFolderCommand: IRequest<bool>
    {
        public Guid SourceId { get; set; }
        public Guid DistinationParentId { get; set; }
    }
}
