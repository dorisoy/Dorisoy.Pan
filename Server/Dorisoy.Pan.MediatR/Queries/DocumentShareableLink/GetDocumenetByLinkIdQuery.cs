using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetDocumenetByLinkIdQuery : IRequest<ServiceResponse<DocumentShareableLinkDto>>
    {
        public Guid Id { get; set; }
    }
}
