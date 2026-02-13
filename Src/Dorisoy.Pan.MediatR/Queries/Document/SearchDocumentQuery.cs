using Dorisoy.Pan.Data.Dto;
using MediatR;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class SearchDocumentQuery : IRequest<List<DocumentDto>>
    {
        public string SearchString { get; set; }
    }
}
