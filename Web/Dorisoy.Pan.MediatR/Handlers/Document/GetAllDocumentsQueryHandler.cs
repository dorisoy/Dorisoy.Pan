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
using System.Linq.Expressions;
using System;
using Dorisoy.Pan.Common;

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
            Expression<Func<Document, bool>> where = c => c.PhysicalFolderId == folder.PhysicalFolderId && !c.DocumentDeleteds.Any(cs => cs.UserId == _userInfoToken.Id);
            if (folder.ParentId == null)
            {
                where = where.And(c => c.CreatedBy == _userInfoToken.Id);
            }
            var entitiesDto = await _documentRepository.All
                    .IgnoreQueryFilters()
                    .Where(where)
                    .Select(c => new DocumentDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Extension = c.Extension,
                        PhysicalFolderId = c.PhysicalFolderId,
                        ThumbnailPath = ThumbnailHelper.GetThumbnailFile(_pathHelper.DocumentPath, c.ThumbnailPath),
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
                entity.Users = entity.Users.Where(c => !EF.Constant(entity.DeletedUserIds).Contains(c.Id)).ToList();
            });
            return entitiesDto;
        }
    }
}
