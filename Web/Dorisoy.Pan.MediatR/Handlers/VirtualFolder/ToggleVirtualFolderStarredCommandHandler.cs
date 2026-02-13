using Dorisoy.Pan.Common.UnitOfWork;
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
    public class ToggleVirtualFolderStarredCommandHandler : IRequestHandler<ToggleVirtualFolderStarredCommand, ServiceResponse<bool>>
    {
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly ILogger<ToggleVirtualFolderStarredCommandHandler> _logger;
        private readonly UserInfoToken _userInfoToken;

        public ToggleVirtualFolderStarredCommandHandler(IVirtualFolderUserRepository virtualFolderUserRepository,
            IUnitOfWork<DocumentContext> uow,
            ILogger<ToggleVirtualFolderStarredCommandHandler> logger,
            UserInfoToken userInfoToken)
        {
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _uow = uow;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }
        public async Task<ServiceResponse<bool>> Handle(ToggleVirtualFolderStarredCommand request, CancellationToken cancellationToken)
        {
            var virtualFolderUser = await _virtualFolderUserRepository.All
                        .FirstOrDefaultAsync(c => c.UserId == _userInfoToken.Id && c.FolderId == request.Id);
            virtualFolderUser.IsStarred = !virtualFolderUser.IsStarred;
            _virtualFolderUserRepository.Update(virtualFolderUser);

            if (await _uow.SaveAsync() <= 0)
            {
                _logger.LogError("Error while updating Starred.");
                return ServiceResponse<bool>.Return500();
            }
            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}
