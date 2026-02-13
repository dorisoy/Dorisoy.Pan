using Dorisoy.Pan.Common;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class RestoreDeletedFolderCommandHandler : IRequestHandler<RestoreDeletedFolderCommand, ServiceResponse<bool>>
    {
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<RestoreDeletedFolderCommandHandler> _logger;

        public RestoreDeletedFolderCommandHandler(IVirtualFolderUserRepository virtualFolderUserRepository,
            IVirtualFolderRepository virtualFolderRepository,
            UserInfoToken userInfoToken,
            IUnitOfWork<DocumentContext> uow,
            ILogger<RestoreDeletedFolderCommandHandler> logger)
        {
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _virtualFolderRepository = virtualFolderRepository;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _logger = logger;
        }
        public async Task<ServiceResponse<bool>> Handle(RestoreDeletedFolderCommand request, CancellationToken cancellationToken)
        {
            var virtualFolderUser = await _virtualFolderUserRepository
               .All
               .IgnoreQueryFilters()
               .Where(c => c.FolderId == request.Id
                   && c.UserId == _userInfoToken.Id && c.IsDeleted)
               .FirstOrDefaultAsync();

            if (virtualFolderUser == null)
            {
                return ServiceResponse<bool>.Return404();
            }

            var virtualFolderIdsToRestore = new List<Guid> { virtualFolderUser.FolderId };
            var virtualChildFolder = await _virtualFolderRepository.GetChildsHierarchyById(virtualFolderUser.FolderId);
            virtualFolderIdsToRestore.AddRange(virtualChildFolder.Select(c => c.Id));

            var virtualFolderUsersToRestore = _virtualFolderUserRepository.All
                .IgnoreQueryFilters()
                .Where(c => c.UserId == _userInfoToken.Id && c.IsDeleted)
                .WhereContains(c => c.FolderId, virtualFolderIdsToRestore).ToList();

            virtualFolderUsersToRestore.ForEach(user =>
            {
                user.IsDeleted = false;
                user.DeletedDate = null;
                user.DeletedBy = null;
            });

            _virtualFolderUserRepository.UpdateRange(virtualFolderUsersToRestore);
            if (await _uow.SaveAsync() <= 0)
            {
                _logger.LogError("Error while Restoring Folder.");
                return ServiceResponse<bool>.Return500();
            }
            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}
