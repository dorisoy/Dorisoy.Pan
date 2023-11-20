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
    public class DeleteVirtualFolderCommandHandler : IRequestHandler<DeleteVirtualFolderCommand, ServiceResponse<VirtualFolderDto>>
    {
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<DeleteVirtualFolderCommandHandler> _logger;

        public DeleteVirtualFolderCommandHandler(IVirtualFolderUserRepository virtualFolderUserRepository,
            IVirtualFolderRepository virtualFolderRepository,
            UserInfoToken userInfoToken,
            IUnitOfWork<DocumentContext> uow,
            ILogger<DeleteVirtualFolderCommandHandler> logger)
        {
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _virtualFolderRepository = virtualFolderRepository;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _logger = logger;
        }
        public async Task<ServiceResponse<VirtualFolderDto>> Handle(DeleteVirtualFolderCommand request, CancellationToken cancellationToken)
        {
            var virtualFolderUser = await _virtualFolderUserRepository
                .All
                .Where(c => (c.FolderId == request.Id || c.VirtualFolder.PhysicalFolderId== request.Id)
                    && c.UserId == _userInfoToken.Id)
                .FirstOrDefaultAsync();
            if (virtualFolderUser == null)
            {
                return ServiceResponse<VirtualFolderDto>.Return404();
            }

            var virtualFolderIdsToDelete = new List<Guid> { virtualFolderUser.FolderId };
            var virtualChildFolder = await _virtualFolderRepository.GetChildsHierarchyById(virtualFolderUser.FolderId);
            virtualFolderIdsToDelete.AddRange(virtualChildFolder.Select(c => c.Id));

            var virtualFolderUsersToDelete = _virtualFolderUserRepository.All
                .Where(c => c.UserId == _userInfoToken.Id && virtualFolderIdsToDelete.Contains(c.FolderId)).ToList();

            virtualFolderUsersToDelete.ForEach(user => user.IsDeleted = true);
            _virtualFolderUserRepository.UpdateRange(virtualFolderUsersToDelete);

            if (await _uow.SaveAsync() <= 0)
            {
                _logger.LogError("Error while deleting Folder.");
                return ServiceResponse<VirtualFolderDto>.Return500();
            }
            return ServiceResponse<VirtualFolderDto>.ReturnSuccess();
        }
    }
}
