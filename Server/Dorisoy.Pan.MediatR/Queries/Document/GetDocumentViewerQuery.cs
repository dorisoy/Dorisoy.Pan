using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetDocumentViewerQuery: IRequest<DocumentSource>
    {
        public Guid DocumentId { get; set; }
    }
}
