using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetVirtualFoldersQuery : IRequest<List<VirtualFolderInfoDto>>
    {
        public Guid? Id { get; set; }
    }
}
