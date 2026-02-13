using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class SearchDocumentQueryHandler : IRequestHandler<SearchDocumentQuery, List<DocumentDto>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly UserManager<User> _userManager;

        public SearchDocumentQueryHandler(IDocumentRepository documentRepository,
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
        public async Task<List<DocumentDto>> Handle(SearchDocumentQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_userInfoToken.Id.ToString());

            var documents = await _documentRepository.All
                .OrderByDescending(c => c.CreatedDate)
                .Where(c =>
                    (c.CreatedBy == _userInfoToken.Id
                    || c.Folder.PhysicalFolderUsers.Any(pf => pf.UserId == _userInfoToken.Id)
                    || c.SharedDocumentUsers.Any(sd => sd.UserId == _userInfoToken.Id))
                    && EF.Functions.Like(c.Name, $"%{request.SearchString}%")
                    && !c.DocumentDeleteds.Any(cs => cs.UserId == _userInfoToken.Id))
                .Select(c => new DocumentDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Extension = c.Extension,
                    PhysicalFolderId = c.PhysicalFolderId,
                    ThumbnailPath = $"{_pathHelper.DocumentPath}{c.ThumbnailPath}",
                    ModifiedDate = c.ModifiedDate,
                    Size = c.Size,
                    IsStarred = c.DocumentStarreds.Any(cs => cs.UserId == _userInfoToken.Id),
                    DeletedUserIds = c.DocumentDeleteds.Where(cs => cs.IsDeleted).Select(c => c.UserId).ToList(),
                    CreatedByUserInfo = new UserInfoDto
                    {
                        Id = c.CreatedByUser.Id,
                        Email = c.CreatedByUser.Email,
                        FirstName = c.CreatedByUser.FirstName,
                        LastName = c.CreatedByUser.LastName,
                        IsOwner = true
                    },
                    Users = c.SharedDocumentUsers.Select(cs => new UserInfoDto
                    {
                        Id = cs.UserId,
                        Email = cs.User.Email,
                        FirstName = cs.User.FirstName,
                        LastName = cs.User.LastName,
                        IsOwner = c.CreatedBy == cs.UserId
                    }).ToList(),
                    PhysicalUsers = c.Folder.PhysicalFolderUsers.Select(cs => new UserInfoDto
                    {
                        Id = cs.UserId,
                        Email = cs.User.Email,
                        FirstName = cs.User.FirstName,
                        LastName = cs.User.LastName,
                        IsOwner = c.CreatedBy == cs.UserId
                    }).ToList()
                }).Take(30).ToListAsync();

            documents.ForEach(entity =>
            {
                if (!entity.PhysicalUsers.Any(c => c.Id == entity.CreatedByUserInfo.Id))
                {
                    entity.PhysicalUsers.Add(entity.CreatedByUserInfo);
                }
                entity.CreatedByUserInfo = null;
                var userIds = entity.Users.Select(c => c.Id);
                var usersToAdd = entity.PhysicalUsers.Where(c => !EF.Constant(userIds).Contains(c.Id)).ToList();
                entity.Users.AddRange(usersToAdd);
                entity.PhysicalUsers = null;
                entity.Users = entity.Users.Where(c => !EF.Constant(entity.DeletedUserIds).Contains(c.Id)).ToList();
            });
            return documents;
        }
    }
}
