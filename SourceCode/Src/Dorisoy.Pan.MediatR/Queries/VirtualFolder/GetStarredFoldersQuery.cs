using Dorisoy.Pan.Data.Dto;
using MediatR;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetStarredFoldersQuery : IRequest<List<VirtualFolderInfoDto>>
    {
    }
}
