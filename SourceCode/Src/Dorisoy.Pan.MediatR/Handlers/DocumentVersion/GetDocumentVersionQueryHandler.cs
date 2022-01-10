using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetDocumentVersionQueryHandler : IRequestHandler<GetDocumentVersionQuery, List<DocumentVersionInfoDto>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentVersionRepository _documentVersionRepository;

        public GetDocumentVersionQueryHandler(IDocumentRepository documentRepository,
            IDocumentVersionRepository documentVersionRepository)
        {
            _documentRepository = documentRepository;
            _documentVersionRepository = documentVersionRepository;
        }
        public async Task<List<DocumentVersionInfoDto>> Handle(GetDocumentVersionQuery request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.All
                .Include(c => c.ModifiedByUser)
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (document == null)
            {
                return new List<DocumentVersionInfoDto>();
            }

            var documnetVersions = await _documentVersionRepository.All
                .Include(c => c.ModifiedByUser)
                .Where(c => c.DocumentId == request.Id)
                .Select(documnetVersion => new DocumentVersionInfoDto
                {
                    Id = documnetVersion.Id,
                    ModifiedDate = documnetVersion.ModifiedDate,
                    Size = documnetVersion.Size,
                    UserName = $"{documnetVersion.ModifiedByUser.FirstName} {documnetVersion.ModifiedByUser.LastName}"
                }).ToListAsync();

            documnetVersions.Add(new DocumentVersionInfoDto
            {
                Id = document.Id,
                IsCurrentVersion = true,
                UserName = $"{document.ModifiedByUser.FirstName} {document.ModifiedByUser.LastName}",
                ModifiedDate = document.ModifiedDate,
                Size = document.Size
            });

            return documnetVersions.OrderByDescending(c => c.ModifiedDate).ToList();
        }
    }
}
