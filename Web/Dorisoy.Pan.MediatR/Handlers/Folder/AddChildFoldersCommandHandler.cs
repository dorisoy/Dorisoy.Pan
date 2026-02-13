using Dorisoy.Pan.Common;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
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
    public class AddChildFoldersCommandHandler : IRequestHandler<AddChildFoldersCommand, ServiceResponse<List<VirtualFolderInfoDto>>>
    {
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IPhysicalFolderUserRepository _physicalFolderUserRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddChildFoldersCommandHandler(IPhysicalFolderRepository physicalFolderRepository,
            IPhysicalFolderUserRepository physicalFolderUserRepository,
            IVirtualFolderRepository virtualFolderRepository,
            IVirtualFolderUserRepository virtualFolderUserRepository,
            IUnitOfWork<DocumentContext> uow,
            UserInfoToken userInfoToken)
        {
            _physicalFolderRepository = physicalFolderRepository;
            _physicalFolderUserRepository = physicalFolderUserRepository;
            _virtualFolderRepository = virtualFolderRepository;
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }
        public async Task<ServiceResponse<List<VirtualFolderInfoDto>>> Handle(AddChildFoldersCommand request, CancellationToken cancellationToken)
        {
            var folderPaths = request.Paths.Select(c => string.Join("/", c.Split("/").SkipLast(1))).ToList();
            var allFoldersPath = folderPaths.Distinct().Select(c => c.Split("/").ToList()).ToList();
            var physicalFolders = new List<PhysicalFolder>();
            var virtualFolders = new List<VirtualFolder>();
            var virtualFolderUserPemissions = new List<VirtualFolderUser>();
            var topMostFolderName = allFoldersPath.FirstOrDefault().FirstOrDefault();

            foreach (var names in allFoldersPath)
            {
                var physicalFolderId = request.PhysicalFolderId;
                foreach (var name in names)
                {
                    // Physical Folder
                    var physicalFolder = physicalFolders.FirstOrDefault(c => c.ParentId == physicalFolderId && c.Name == name);
                    // Physical folder already initialize.
                    if (physicalFolder != null)
                    {
                        physicalFolderId = physicalFolder.Id;
                    }
                    else
                    {
                        // Physical
                        var existingPhysicalFolder = _physicalFolderRepository
                            .All
                            .Include(c => c.PhysicalFolderUsers)
                            .FirstOrDefault(c => c.ParentId == physicalFolderId
                                && c.Name == name
                                && c.PhysicalFolderUsers.Any(c => c.UserId == _userInfoToken.Id));
                        if (existingPhysicalFolder != null)
                        {
                            // Restore from trace.
                            var virtualFolderPemission = await _virtualFolderUserRepository
                                .All
                                .IgnoreQueryFilters()
                                .Include(c => c.VirtualFolder)
                                .Where(c => c.UserId == _userInfoToken.Id
                                    && c.VirtualFolder.PhysicalFolderId == physicalFolderId
                                    && c.IsDeleted).FirstOrDefaultAsync();
                            if (virtualFolderPemission != null)
                            {
                                var isAlreadyAdded = virtualFolderUserPemissions
                                    .Any(c => c.UserId == virtualFolderPemission.UserId
                                            && c.FolderId == virtualFolderPemission.FolderId);
                                if (!isAlreadyAdded)
                                {
                                    virtualFolderUserPemissions.Add(virtualFolderPemission);
                                }
                            }
                            physicalFolderId = existingPhysicalFolder.Id;
                        }
                        else
                        {
                            var physicalFolderIdToCreate = Guid.NewGuid();
                            physicalFolder = new PhysicalFolder
                            {
                                Id = physicalFolderIdToCreate,
                                Name = name,
                                ParentId = physicalFolderId,
                            };
                            physicalFolders.Add(physicalFolder);

                            // Virtual Folder
                            var virtualFoldersToCreate = _virtualFolderRepository.All.Where(c => c.PhysicalFolderId == physicalFolderId).ToList();
                            var localVirtualFoldersToCreate = virtualFolders.Where(c => c.PhysicalFolderId == physicalFolderId).ToList();
                            virtualFoldersToCreate.ForEach(virtualFolder =>
                            {
                                virtualFolders.Add(new VirtualFolder
                                {
                                    Id = Guid.NewGuid(),
                                    Name = name,
                                    ParentId = virtualFolder.Id,
                                    PhysicalFolderId = physicalFolderIdToCreate
                                });
                            });
                            localVirtualFoldersToCreate.ForEach(folder =>
                            {
                                virtualFolders.Add(new VirtualFolder
                                {
                                    Id = Guid.NewGuid(),
                                    Name = name,
                                    ParentId = folder.Id,
                                    PhysicalFolderId = physicalFolderIdToCreate
                                });
                            });

                            physicalFolderId = physicalFolderIdToCreate;
                        }
                    }
                }
            }

            if (virtualFolders.Any() || physicalFolders.Any() || virtualFolderUserPemissions.Any())
            {
                _virtualFolderRepository.AddRange(virtualFolders);
                _physicalFolderRepository.AddRange(physicalFolders);

                var userIds = await GetFolderUserIds(topMostFolderName, request.PhysicalFolderId, request.VirtualFolderId);

                // Add Current User if it's root or not shared yet.
                userIds = userIds.Any() ? userIds : new List<Guid> { _userInfoToken.Id };

                // Create Physical Folders.
                if (physicalFolders.Any())
                {
                    var physicalFolderUsers = physicalFolders
                           .Join(userIds, f => true, u => true, (f, u) => new PhysicalFolderUser
                           {
                               FolderId = f.Id,
                               UserId = u
                           }).ToList();
                    _physicalFolderUserRepository.AddRange(physicalFolderUsers);
                }

                // Create Virtual Folders.
                if (virtualFolders.Any())
                {
                    var virtualFolderUsers = virtualFolders
                        .Join(userIds, f => true, u => true, (f, u) => new VirtualFolderUser
                        {
                            FolderId = f.Id,
                            UserId = u
                        }).ToList();
                    _virtualFolderUserRepository.AddRange(virtualFolderUsers);
                }

                // Restore the permission if Re-Uploaded the folder
                if (virtualFolderUserPemissions.Any())
                {
                    virtualFolderUserPemissions.ForEach(permission => permission.IsDeleted = false);
                    _virtualFolderUserRepository.UpdateRange(virtualFolderUserPemissions);
                }

                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<List<VirtualFolderInfoDto>>.Return500();
                }
            }

            var folderIdsToReturn = virtualFolders.Where(c => c.ParentId == request.VirtualFolderId).Select(c => c.Id).ToList();

            var virtualFolderInfo = await _virtualFolderRepository.All
                  .Include(c => c.PhysicalFolder)
                  .ThenInclude(c => c.PhysicalFolderUsers)
                  .ThenInclude(c => c.User)
                  .WhereContains(c => c.Id, folderIdsToReturn)
                  .Select(c => new VirtualFolderInfoDto
                  {
                      Id = c.Id,
                      Name = c.Name,
                      ParentId = c.ParentId,
                      PhysicalFolderId = c.PhysicalFolderId,
                      IsShared = c.IsShared,
                      Users = c.PhysicalFolder.PhysicalFolderUsers.Select(cs => new UserInfoDto
                      {
                          Email = cs.User.Email,
                          FirstName = cs.User.FirstName,
                          Id = cs.UserId,
                          LastName = cs.User.LastName
                      }).ToList()
                  }).ToListAsync();
            return ServiceResponse<List<VirtualFolderInfoDto>>.ReturnResultWith201(virtualFolderInfo);
        }

        private async Task<List<Guid>> GetFolderUserIds(string folderName, Guid parentPhysicalFolderId, Guid parentVirtualFolderId)
        {
            var virtualParentFolder = await _virtualFolderRepository.FindAsync(parentVirtualFolderId);
            if (virtualParentFolder.ParentId == null) // Root Folder
            {
                var existingEntityPermission = await _virtualFolderRepository
                    .All
                    .IgnoreQueryFilters()
                    .Include(c => c.VirtualFolderUsers)
                    .Where(c => c.Name == folderName
                    && c.ParentId == parentVirtualFolderId
                    && c.VirtualFolderUsers.Any(c => c.UserId == _userInfoToken.Id))
                    .FirstOrDefaultAsync();
                if (existingEntityPermission != null)
                {
                    return existingEntityPermission.VirtualFolderUsers.Select(c => c.UserId).ToList();
                }
                return new List<Guid>();
            }

            return await _virtualFolderRepository.All
                .Where(c => c.PhysicalFolderId == parentPhysicalFolderId)
                .IgnoreQueryFilters()
                .SelectMany(c => c.VirtualFolderUsers)
                .Select(c => c.UserId).ToListAsync();
        }
    }
}
