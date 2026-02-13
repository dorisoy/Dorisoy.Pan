using Dorisoy.Pan.Helper;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class AddFolderUserPermissionCommand : IRequest<ServiceResponse<bool>>
    {
        public Guid FolderId { get; set; }
        public List<Guid> Users { get; set; }
    }
}
