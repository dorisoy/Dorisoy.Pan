using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class AddChildFoldersCommand : IRequest<ServiceResponse<List<VirtualFolderInfoDto>>>
    {
        public Guid VirtualFolderId { get; set; }
        public Guid PhysicalFolderId { get; set; }
        public List<string> Paths { get; set; }
    }
}
