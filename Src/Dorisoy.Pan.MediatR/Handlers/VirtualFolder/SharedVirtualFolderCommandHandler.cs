using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class SharedVirtualFolderCommandHandler : IRequestHandler<SharedVirtualFolderCommand, bool>
    {
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly IPhysicalFolderUserRepository _physicalFolderUserRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IConnectionMappingRepository _connectionMappingRepository;
        public SharedVirtualFolderCommandHandler(
            IVirtualFolderRepository virtualFolderRepository,
            IVirtualFolderUserRepository virtualFolderUserRepository,
            IPhysicalFolderUserRepository physicalFolderUserRepository,
            IUnitOfWork<DocumentContext> uow,
            IUserNotificationRepository userNotificationRepository,
            IConnectionMappingRepository connectionMappingRepository
            )
        {
            _virtualFolderRepository = virtualFolderRepository;
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _physicalFolderUserRepository = physicalFolderUserRepository;
            _uow = uow;
            _userNotificationRepository = userNotificationRepository;
            _connectionMappingRepository = connectionMappingRepository;
        }
        public async Task<bool> Handle(SharedVirtualFolderCommand request, CancellationToken cancellationToken)
        {
            var virtualFolder = await _virtualFolderRepository.GetRootFolder();
            var orignalVirtualFolder = await _virtualFolderRepository.All.Where(c => c.Id == request.Id).FirstOrDefaultAsync();
            var hierarchyFolder = new HierarchyFolder { Id = orignalVirtualFolder.Id, ParentId = orignalVirtualFolder.ParentId, Level = 0, Name = orignalVirtualFolder.Name, PhysicalFolderId = orignalVirtualFolder.PhysicalFolderId };
            Guid Id = Guid.NewGuid();

            await _physicalFolderUserRepository.AddPhysicalFolderUsersChildsById(hierarchyFolder.PhysicalFolderId, request.Users);
            var folderExist = await _virtualFolderRepository.SharedFolderExist(hierarchyFolder.PhysicalFolderId, virtualFolder.Id);
            if (folderExist != null)
            {
                if (!orignalVirtualFolder.IsShared)
                {
                    orignalVirtualFolder.IsShared = true;
                    _virtualFolderRepository.Update(orignalVirtualFolder);
                }
                await _virtualFolderUserRepository.AddVirtualFolderUsersChildsById(folderExist.Id, request.Users);
                if (orignalVirtualFolder.Id != folderExist.Id && !folderExist.IsShared)
                {
                    folderExist.IsShared = true;
                    _virtualFolderRepository.Update(orignalVirtualFolder);
                }
                Id = folderExist.Id;
            }
            else
            {
                orignalVirtualFolder.IsShared = true;
                _virtualFolderRepository.Update(orignalVirtualFolder);
                var childUserFolders = await _virtualFolderRepository.GetChildsHierarchyById(request.Id);
                var newId = Guid.NewGuid();
                folderExist = new VirtualFolder
                {
                    Id = newId,
                    Name = hierarchyFolder.Name,
                    ParentId = virtualFolder.Id,
                    PhysicalFolderId = hierarchyFolder.PhysicalFolderId,
                    IsShared = true
                };
                var virtualFolderUsers = request.Users.Select(c => new VirtualFolderUser
                {
                    FolderId = newId,
                    UserId = c
                }).ToList();

                folderExist.VirtualFolderUsers.AddRange(virtualFolderUsers);
                var populatedOrgano = PopulateChildren(hierarchyFolder, childUserFolders, folderExist, request.Users);
                _virtualFolderRepository.Add(folderExist);
                Id = newId;

            }

            _userNotificationRepository.SaveUserNotification(Id, null, request.Users, ActionEnum.Shared);

            if (await _uow.SaveAsync() <= 0)
            {
                return false;
            }
            await _connectionMappingRepository.SendFolderNotification(request.Users, Id);
            return true;
        }
        public HierarchyFolder PopulateChildren(HierarchyFolder sourceOrgano, ICollection<HierarchyFolder> organos, VirtualFolder root, List<Guid> users)
        {
            var children = organos.Where(x => x.ParentId == sourceOrgano.Id);
            foreach (var child in children)
            {
                //child
                HierarchyFolder organoChild_ = new HierarchyFolder();
                VirtualFolder virtualFolder_ = new VirtualFolder();
                virtualFolder_.Id = Guid.NewGuid();
                virtualFolder_.Name = child.Name;
                virtualFolder_.ParentId = root.Id;
                virtualFolder_.PhysicalFolderId = child.PhysicalFolderId;
                var virtualFolderUsers = users.Select(c => new VirtualFolderUser
                {
                    FolderId = virtualFolder_.Id,
                    UserId = c
                }).ToList();
                virtualFolder_.VirtualFolderUsers.AddRange(virtualFolderUsers);
                organoChild_.Id = child.Id;
                organoChild_.Name = child.Name;
                organoChild_.ParentId = sourceOrgano.Id;
                sourceOrgano.Children.Add(organoChild_);
                root.Children.Add(virtualFolder_);
                PopulateChildren(child, organos, virtualFolder_, users);
            }
            return sourceOrgano;
        }

    }


}
