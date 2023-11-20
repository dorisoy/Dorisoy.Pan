using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetDocumentShareableLinkQuery : IRequest<DocumentShareableLinkDto>
    {
        public Guid Id { get; set; }
    }
}
