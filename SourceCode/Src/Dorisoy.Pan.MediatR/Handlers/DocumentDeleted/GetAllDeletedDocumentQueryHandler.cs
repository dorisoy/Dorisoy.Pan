using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetAllDeletedDocumentQueryHandler : IRequestHandler<GetAllDeletedDocumentQuery, List<DeletedDocumentInfoDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IDocumentDeletedRepository _documentDeletedRepository;
        private readonly PathHelper _pathHelper;

        public GetAllDeletedDocumentQueryHandler(UserInfoToken userInfoToken,
            IDocumentDeletedRepository documentDeletedRepository,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _documentDeletedRepository = documentDeletedRepository;
            _pathHelper = pathHelper;
        }

        public async Task<List<DeletedDocumentInfoDto>> Handle(GetAllDeletedDocumentQuery request, CancellationToken cancellationToken)
        {
            var deletedDocuments = await _documentDeletedRepository
                .All
                .Where(c => c.UserId == _userInfoToken.Id)
                .Select(cs => new DeletedDocumentInfoDto
                {
                    Id = cs.Document.Id,
                    DeletedDate = cs.CreatedDate,
                    Name = cs.Document.Name,
                    ThumbnailPath = Path.Combine(_pathHelper.DocumentPath, cs.Document.ThumbnailPath),
                }).ToListAsync();

            return deletedDocuments;
        }
    }
}
