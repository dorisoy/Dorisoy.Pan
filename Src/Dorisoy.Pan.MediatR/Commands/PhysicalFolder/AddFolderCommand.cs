using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class AddFolderCommand : IRequest<ServiceResponse<VirtualFolderInfoDto>>
    {
        public string Name { get; set; }
        public Guid VirtualParentId { get; set; }
        public Guid PhysicalFolderId { get; set; }
    }
}
