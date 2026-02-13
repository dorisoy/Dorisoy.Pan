using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class RemoveFolderRightForUserCommandHandler : IRequestHandler<RemoveFolderRightForUserCommand, bool>
    {
        private readonly IPhysicalFolderUserRepository _physicalFolderUserRepository;
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IConnectionMappingRepository _connectionMappingRepository;
        private readonly IUserNotificationRepository _userNotificationRepository;
        public RemoveFolderRightForUserCommandHandler(
            IPhysicalFolderUserRepository physicalFolderUserRepository,
            IVirtualFolderUserRepository virtualFolderUserRepository,
            IUnitOfWork<DocumentContext> uow,
            IVirtualFolderRepository virtualFolderRepository,
            IPhysicalFolderRepository physicalFolderRepository,
            IConnectionMappingRepository connectionMappingRepository,
            IUserNotificationRepository userNotificationRepository)
        {
            _physicalFolderUserRepository = physicalFolderUserRepository;
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _uow = uow;
            _virtualFolderRepository = virtualFolderRepository;
            _physicalFolderRepository = physicalFolderRepository;
            _connectionMappingRepository = connectionMappingRepository;
            _userNotificationRepository = userNotificationRepository;
        }
        public async Task<bool> Handle(RemoveFolderRightForUserCommand request, CancellationToken cancellationToken)
        {
            // Remove Physical Folder Rights
            var physicaluser = await _physicalFolderUserRepository
                .FindBy(c => c.UserId == request.UserId && c.FolderId == request.PhysicalFolderId)
                .FirstOrDefaultAsync();
            if (physicaluser != null)
            {
                _physicalFolderUserRepository.Remove(physicaluser);
                var physicalFolderChilds = await _physicalFolderRepository.GetChildsHierarchyById(physicaluser.FolderId);
                await _physicalFolderUserRepository.RemovedFolderUsers(physicalFolderChilds, request.UserId);
            }

            // Remove Virtual Folder Rights
            var virtualFolderUser = await _virtualFolderUserRepository
              .FindBy(c => c.UserId == request.UserId && c.VirtualFolder.PhysicalFolderId == request.PhysicalFolderId)
              .FirstOrDefaultAsync();
            if (virtualFolderUser != null)
            {
                _virtualFolderUserRepository.Remove(virtualFolderUser);
                var virtualFolderChilds = await _virtualFolderRepository.GetChildsHierarchyById(virtualFolderUser.FolderId);
                await _virtualFolderUserRepository.RemovedFolderUsers(virtualFolderChilds, request.UserId);
            }

            // Remove User Notifications
            var notifications = await _userNotificationRepository.All
                .Where(c => c.FolderId == request.FolderId && c.ToUserId == request.UserId).ToListAsync();
            _userNotificationRepository.RemoveRange(notifications);


            var physicalUsersCount = await _physicalFolderUserRepository
                .FindBy(c => c.FolderId == request.PhysicalFolderId && c.UserId != request.UserId).CountAsync();
            if (physicalUsersCount <= 1)
            {
                var virtualFolders = await _virtualFolderRepository
             .FindBy(c => c.PhysicalFolderId == request.PhysicalFolderId)
             .ToListAsync();
                foreach (var vf in virtualFolders)
                {
                    vf.IsShared = false;
                    _virtualFolderRepository.Update(vf);
                }
            }
            if (await _uow.SaveAsync() <= 0)
            {
                return false;
            }
            await _connectionMappingRepository.RemovedFolderNotification(new List<Guid> { request.UserId }, request.FolderId);
            return true;
        }
    }
}
