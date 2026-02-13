using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetVirtualFolderForMoveAndCopyQuery : IRequest<List<VirtualFolderInfoDto>>
    {
        public Guid ParentId { get; set; }
        public Guid SourceId { get; set; }
    }
}
