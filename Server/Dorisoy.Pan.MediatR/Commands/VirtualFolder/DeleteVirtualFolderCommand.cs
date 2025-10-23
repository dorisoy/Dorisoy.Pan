using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class DeleteVirtualFolderCommand : IRequest<ServiceResponse<VirtualFolderDto>>
    {
        public Guid Id { get; set; }
    }
}
