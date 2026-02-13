using Dorisoy.Pan.Data.Dto;
using MediatR;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetRootFolderQuery : IRequest<VirtualFolderDto>
    {
    }
}
