using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class CopyFolderCommand : IRequest<VirtualFolderDto>
    {
        public Guid SourceId { get; set; }
        public Guid DistinationParentId { get; set; }
    }
}
