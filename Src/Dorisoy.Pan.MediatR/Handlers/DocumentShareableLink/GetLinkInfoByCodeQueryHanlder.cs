using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetLinkInfoByCodeQueryHanlder : IRequestHandler<GetLinkInfoByCodeQuery, DocumentShareableLinkDto>
    {
        private readonly IDocumentShareableLinkRepository _documentShareableLinkRepository;

        public GetLinkInfoByCodeQueryHanlder(IDocumentShareableLinkRepository documentShareableLinkRepository)
        {
            _documentShareableLinkRepository = documentShareableLinkRepository;
        }
        public async Task<DocumentShareableLinkDto> Handle(GetLinkInfoByCodeQuery request, CancellationToken cancellationToken)
        {
            var link = await _documentShareableLinkRepository.All.FirstOrDefaultAsync(c => c.LinkCode == request.Code);
            if (link == null)
            {
                return new DocumentShareableLinkDto
                {
                    IsLinkExpired = true
                };
            }

            var linkInfo = new DocumentShareableLinkDto
            {
                Id = link.Id,
                HasPassword = !string.IsNullOrWhiteSpace(link.Password)
            };
            if (link.LinkExpiryTime.HasValue)
            {
                linkInfo.IsLinkExpired = DateTime.Now > link.LinkExpiryTime;
            }
            return linkInfo;
        }
    }
}
