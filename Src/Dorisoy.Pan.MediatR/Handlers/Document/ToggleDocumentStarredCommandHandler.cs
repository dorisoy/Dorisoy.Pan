using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class ToggleDocumentStarredCommandHandler : IRequestHandler<ToggleDocumentStarredCommand, ServiceResponse<bool>>
    {

        private readonly IDocumentStarredRepository _documentStarredRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly ILogger<ToggleDocumentStarredCommandHandler> _logger;
        private readonly UserInfoToken _userInfoToken;

        public ToggleDocumentStarredCommandHandler(IDocumentStarredRepository documentStarredRepository,
            IUnitOfWork<DocumentContext> uow,
            ILogger<ToggleDocumentStarredCommandHandler> logger,
            UserInfoToken userInfoToken)
        {
            _documentStarredRepository = documentStarredRepository;
            _uow = uow;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }
        public async Task<ServiceResponse<bool>> Handle(ToggleDocumentStarredCommand request, CancellationToken cancellationToken)
        {
            var documentStarred = await _documentStarredRepository.All
            .FirstOrDefaultAsync(c => c.UserId == _userInfoToken.Id && c.DocumentId == request.Id);
            if (documentStarred != null)
            {
                _documentStarredRepository.Remove(documentStarred);
            }
            else
            {
                _documentStarredRepository.Add(new DocumentStarred
                {
                    DocumentId = request.Id,
                    UserId = _userInfoToken.Id
                });
            }
            if (await _uow.SaveAsync() <= 0)
            {
                _logger.LogError("Error while updating Starred.");
                return ServiceResponse<bool>.Return500();
            }
            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}
