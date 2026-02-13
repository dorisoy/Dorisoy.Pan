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
    public class GetAllDocumentCommentQueryHandler : IRequestHandler<GetAllDocumentCommentQuery, List<DocumentCommentDto>>
    {
        private readonly IDocumentCommentRepository _documentCommentRepository;

        public GetAllDocumentCommentQueryHandler(IDocumentCommentRepository documentCommentRepository)
        {
            _documentCommentRepository = documentCommentRepository;
        }
        public async Task<List<DocumentCommentDto>> Handle(GetAllDocumentCommentQuery request, CancellationToken cancellationToken)
        {
            return await _documentCommentRepository
                 .All
                 .Where(c => c.DocumentId == request.Id)
                 .OrderBy(c => c.CreatedDate)
                 .Select(c => new DocumentCommentDto
                 {
                     Comment = c.Comment,
                     CreatedDate = c.CreatedDate,
                     UserName = $"{c.CreatedByUser.FirstName} {c.CreatedByUser.LastName}"
                 }).ToListAsync();
        }
    }
}
