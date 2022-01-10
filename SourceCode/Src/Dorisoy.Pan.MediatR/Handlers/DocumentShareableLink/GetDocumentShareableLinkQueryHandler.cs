using AutoMapper;
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
    public class GetDocumentShareableLinkQueryHandler
        : IRequestHandler<GetDocumentShareableLinkQuery, DocumentShareableLinkDto>
    {
        private readonly IDocumentShareableLinkRepository _documentShareableLinkRepository;
        private readonly IMapper _mapper;

        public GetDocumentShareableLinkQueryHandler(IDocumentShareableLinkRepository documentShareableLinkRepository,
            IMapper mapper)
        {
            _documentShareableLinkRepository = documentShareableLinkRepository;
            _mapper = mapper;
        }

        public async Task<DocumentShareableLinkDto> Handle(GetDocumentShareableLinkQuery request, CancellationToken cancellationToken)
        {
            var link = await _documentShareableLinkRepository.All.FirstOrDefaultAsync(c => c.DocumentId == request.Id);
            if (link == null)
            {
                return new DocumentShareableLinkDto { };
            }

            var result = _mapper.Map<DocumentShareableLinkDto>(link);
            if (!string.IsNullOrWhiteSpace(result.Password))
            {
                var base64EncodedBytes = Convert.FromBase64String(result.Password);
                result.Password = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            return result;
        }
    }
}
