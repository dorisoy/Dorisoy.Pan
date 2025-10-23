using Dorisoy.Pan.Helper;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class ToggleVirtualFolderStarredCommand : IRequest<ServiceResponse<bool>>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}
