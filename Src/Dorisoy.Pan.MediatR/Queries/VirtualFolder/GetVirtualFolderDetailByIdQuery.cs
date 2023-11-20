using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetVirtualFolderDetailByIdQuery: IRequest<VirtualFolderInfoDto>
    {
        public Guid Id { get; set; }
    }
}
