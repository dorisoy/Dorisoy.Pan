using Dorisoy.Pan.Data.Dto;
using MediatR;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class SearchVirtualFolderQuery : IRequest<List<VirtualFolderInfoDto>>
    {
        public string SearchString { get; set; }
    }
}
