using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, List<DocumentDto>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly UserManager<User> _userManager;

        public GetAllDocumentsQueryHandler(IDocumentRepository documentRepository,
            PathHelper pathHelper,
            IVirtualFolderRepository virtualFolderRepository,
            UserInfoToken userInfoToken,
            UserManager<User> userManager)
        {
            _documentRepository = documentRepository;
            _pathHelper = pathHelper;
            _virtualFolderRepository = virtualFolderRepository;
            _userInfoToken = userInfoToken;
            _userManager = userManager;
        }
        public async Task<List<DocumentDto>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_userInfoToken.Id.ToString());
            var folder = await _virtualFolderRepository.FindAsync(request.FolderId);

            if (folder.ParentId == null)
            {
                var entitiesDto = await _documentRepository.All
                    .IgnoreQueryFilters()
                    .Where(c => c.PhysicalFolderId == folder.PhysicalFolderId
                        && c.CreatedBy == _userInfoToken.Id
                        && !c.DocumentDeleteds.Any(cs => cs.UserId == _userInfoToken.Id))
                    .Select(c => new DocumentDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Extension = c.Extension,
                        PhysicalFolderId = c.PhysicalFolderId,
                        ThumbnailPath = Path.Combine(_pathHelper.DocumentPath, c.ThumbnailPath),
                        ModifiedDate = c.ModifiedDate,
                        Size = c.Size,
                        IsStarred = c.DocumentStarreds.Any(cs => cs.UserId == _userInfoToken.Id),
                        DeletedUserIds = c.DocumentDeleteds.Where(cs => cs.IsDeleted).Select(c => c.UserId).ToList(),
                        Users = c.SharedDocumentUsers.Select(cs => new UserInfoDto
                        {
                            Id = cs.UserId,
                            Email = cs.User.Email,
                            FirstName = cs.User.FirstName,
                            LastName = cs.User.LastName,
                            IsOwner = c.CreatedBy == cs.UserId,
                            ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath, cs.User.ProfilePhoto)
                        }).ToList()
                    })
                    .OrderByDescending(c => c.ModifiedDate)
                    .ToListAsync();

                entitiesDto.ForEach(entity =>
                {
                    entity.Users.Add(new UserInfoDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsOwner = true,
                        ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath, user.ProfilePhoto)
                    });
                    entity.Users = entity.Users.Where(c => !entity.DeletedUserIds.Contains(c.Id)).ToList();
                });
                return entitiesDto;
            }
            else
            {
                var entitiesDto = await _documentRepository.All
                    .IgnoreQueryFilters()
                    .Where(c => c.PhysicalFolderId == folder.PhysicalFolderId
                           && !c.DocumentDeleteds.Any(cs => cs.UserId == _userInfoToken.Id))
                    .Select(c => new DocumentDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Extension = c.Extension,
                        PhysicalFolderId = c.PhysicalFolderId,
                        ThumbnailPath = Path.Combine(_pathHelper.DocumentPath, c.ThumbnailPath),
                        ModifiedDate = c.ModifiedDate,
                        Size = c.Size,
                        IsStarred = c.DocumentStarreds.Any(cs => cs.UserId == _userInfoToken.Id),
                        DeletedUserIds = c.DocumentDeleteds.Where(cs => cs.IsDeleted).Select(c => c.UserId).ToList(),
                        Users = c.SharedDocumentUsers.Select(cs => new UserInfoDto
                        {
                            Id = cs.UserId,
                            Email = cs.User.Email,
                            FirstName = cs.User.FirstName,
                            LastName = cs.User.LastName,
                            IsOwner = c.CreatedBy == cs.UserId,
                            ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath, cs.User.ProfilePhoto)
                        }).ToList(),
                        PhysicalUsers = c.Folder.PhysicalFolderUsers.Select(cs => new UserInfoDto
                        {
                            Id = cs.UserId,
                            Email = cs.User.Email,
                            FirstName = cs.User.FirstName,
                            LastName = cs.User.LastName,
                            IsOwner = c.CreatedBy == cs.UserId,
                            ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath, cs.User.ProfilePhoto)
                        }).ToList()
                    })
                     .OrderByDescending(c => c.ModifiedDate)
                    .ToListAsync();

                entitiesDto.ForEach(entity =>
                {
                    var userIds = entity.Users.Select(c => c.Id);
                    var usersToAdd = entity.PhysicalUsers.Where(c => !userIds.Contains(c.Id)).ToList();
                    entity.Users.AddRange(usersToAdd);
                    entity.PhysicalUsers = null;
                    entity.Users = entity.Users.Where(c => !entity.DeletedUserIds.Contains(c.Id)).ToList();
                });

                return entitiesDto;
            }
        }
    }
}
