using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class DeleteDeletedFolderCommandHandler : IRequestHandler<DeleteDeletedFolderCommand, ServiceResponse<bool>>
    {
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IPhysicalFolderUserRepository _physicalFolderUserRepository;
        private readonly IRecentActivityRepository _recentActivityRepository;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<DeleteDeletedFolderCommandHandler> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;

        public DeleteDeletedFolderCommandHandler(IVirtualFolderUserRepository virtualFolderUserRepository,
            IVirtualFolderRepository virtualFolderRepository,
            IPhysicalFolderRepository physicalFolderRepository,
            IPhysicalFolderUserRepository physicalFolderUserRepository,
            IRecentActivityRepository recentActivityRepository,
            IUserNotificationRepository userNotificationRepository,
            UserInfoToken userInfoToken,
            IUnitOfWork<DocumentContext> uow,
            ILogger<DeleteDeletedFolderCommandHandler> logger,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper)
        {
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _virtualFolderRepository = virtualFolderRepository;
            _physicalFolderRepository = physicalFolderRepository;
            _physicalFolderUserRepository = physicalFolderUserRepository;
            _recentActivityRepository = recentActivityRepository;
            _userNotificationRepository = userNotificationRepository;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteDeletedFolderCommand request, CancellationToken cancellationToken)
        {
            var virtualFolder = await _virtualFolderRepository.All
                .IgnoreQueryFilters()
                .Include(cs => cs.PhysicalFolder)
                .Where(c => c.Id == request.Id)
               .FirstOrDefaultAsync();

            if (virtualFolder == null)
            {
                return ServiceResponse<bool>.Return404();
            }

            var isOwner = virtualFolder.CreatedBy == _userInfoToken.Id;

            var allPhysicalFolderIdsToDelete = new List<Guid>() { virtualFolder.PhysicalFolderId };
            var physicalChildFolders = await _physicalFolderRepository
                .GetChildsHierarchyById(virtualFolder.PhysicalFolderId);

            allPhysicalFolderIdsToDelete.AddRange(physicalChildFolders.Select(c => c.Id).ToList());

            if (isOwner)
            {
                foreach (var physicalFolderId in allPhysicalFolderIdsToDelete)
                {
                    var vitrualFolders = _virtualFolderRepository.All
                        .IgnoreQueryFilters()
                        .Where(c => c.PhysicalFolderId == physicalFolderId).ToList();

                    _virtualFolderRepository.RemoveRange(vitrualFolders);

                    var physicalFolders = await _physicalFolderRepository.All
                        .IgnoreQueryFilters()
                        .Where(c => c.Id == physicalFolderId)
                        .ToListAsync();

                    _physicalFolderRepository.RemoveRange(physicalFolders);

                    // Delete recent Activity
                    var recentActivities = await _recentActivityRepository.All
                        .Where(c => c.FolderId.HasValue && c.VirtualFolder.PhysicalFolderId == physicalFolderId)
                        .ToListAsync();
                    _recentActivityRepository.RemoveRange(recentActivities);

                    // Delete User Notification
                    var notifications = await _userNotificationRepository.All
                        .Where(c => c.VirtualFolder.PhysicalFolderId == physicalFolderId)
                        .ToListAsync();
                    _userNotificationRepository.RemoveRange(notifications);
                }
            }
            else
            {
                foreach (var physicalFolderId in allPhysicalFolderIdsToDelete)
                {
                    var virtualFolderUser = await _virtualFolderUserRepository.All
                          .IgnoreQueryFilters()
                          .Where(c => c.VirtualFolder.PhysicalFolderId == physicalFolderId
                            && c.UserId == _userInfoToken.Id)
                          .ToListAsync();
                    _virtualFolderUserRepository.RemoveRange(virtualFolderUser);

                    var physicalFolderUsers = _physicalFolderUserRepository.All
                        .IgnoreQueryFilters()
                        .Where(c => c.UserId == _userInfoToken.Id && c.FolderId == physicalFolderId)
                        .ToList();
                    _physicalFolderUserRepository.RemoveRange(physicalFolderUsers);

                    // Delete recent Activity
                    var recentActivities = _recentActivityRepository.All
                        .Where(c => c.FolderId.HasValue && c.UserId == _userInfoToken.Id && c.VirtualFolder.PhysicalFolderId == physicalFolderId)
                        .ToList();
                    _recentActivityRepository.RemoveRange(recentActivities);

                    // Delete User Notification
                    var notifications = _userNotificationRepository.All.Where(c => c.FolderId.HasValue
                        && c.ToUserId == _userInfoToken.Id
                        && c.VirtualFolder.PhysicalFolderId == physicalFolderId).ToList();
                    _userNotificationRepository.RemoveRange(notifications);
                }

                // IsShared flag
                var sharedCount = _physicalFolderUserRepository.All.Count(c => c.FolderId == virtualFolder.PhysicalFolderId && c.UserId != _userInfoToken.Id);
                if (sharedCount <= 1)
                {
                    var virtualFolderToUpdate = _virtualFolderRepository.All
                        .Where(c => c.PhysicalFolderId == virtualFolder.PhysicalFolderId)
                        .ToList();
                    virtualFolderToUpdate.ForEach(c => c.IsShared = false);
                    _virtualFolderRepository.UpdateRange(virtualFolderToUpdate);
                }
            }

            var physicalFolderPath = await _physicalFolderRepository.GetParentFolderPath(virtualFolder.PhysicalFolder.Id);
            if (await _uow.SaveAsync() <= 0)
            {
                _logger.LogError("Error while Deleting Folder.");
                return ServiceResponse<bool>.Return500();
            }

            // Delete Folder from Disk
            if (isOwner)
            {
                var fullcontainerDocumentPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, physicalFolderPath);
                if (Directory.Exists(fullcontainerDocumentPath))
                {
                    try
                    {
                        Directory.Delete(fullcontainerDocumentPath, true);
                    }
                    catch (Exception e)
                    {
                        return ServiceResponse<bool>.ReturnException(e);
                    }
                }
            }
            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}
