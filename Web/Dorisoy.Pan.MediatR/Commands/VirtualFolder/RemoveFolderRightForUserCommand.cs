using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class RemoveFolderRightForUserCommand : IRequest<bool>
    {
        public Guid FolderId { get; set; }
        public Guid PhysicalFolderId { get; set; }
        public Guid UserId { get; set; }
    }
}
