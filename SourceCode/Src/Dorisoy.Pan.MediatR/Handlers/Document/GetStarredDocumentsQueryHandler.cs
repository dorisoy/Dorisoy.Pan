using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetStarredDocumentsQueryHandler : IRequestHandler<GetStarredDocumentsQuery, List<DocumentDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IDocumentStarredRepository _documentStarredRepository;
        private readonly PathHelper _pathHelper;

        public GetStarredDocumentsQueryHandler(UserInfoToken userInfoToken,
            IDocumentStarredRepository documentStarredRepository,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _documentStarredRepository = documentStarredRepository;
            _pathHelper = pathHelper;
        }
        public async Task<List<DocumentDto>> Handle(GetStarredDocumentsQuery request, CancellationToken cancellationToken)
        {
            var entitiesDto = await _documentStarredRepository.All
                .IgnoreQueryFilters()
                .Where(c => c.UserId == _userInfoToken.Id
                    && !c.Document.DocumentDeleteds.Any(cs => cs.UserId == _userInfoToken.Id))
                .Select(c => new DocumentDto
                {
                    Id = c.Document.Id,
                    Name = c.Document.Name,
                    Extension = c.Document.Extension,
                    PhysicalFolderId = c.Document.PhysicalFolderId,
                    ThumbnailPath = Path.Combine(_pathHelper.DocumentPath,c.Document.ThumbnailPath),
                    ModifiedDate = c.Document.ModifiedDate,
                    Size = c.Document.Size,
                    IsStarred = c.Document.DocumentStarreds.Any(cs => cs.UserId == _userInfoToken.Id),
                    DeletedUserIds = c.Document.DocumentDeleteds.Where(cd => cd.IsDeleted).Select(c => c.UserId).ToList(),
                    Users = c.Document.SharedDocumentUsers.Select(cs => new UserInfoDto
                    {
                        Id = cs.UserId,
                        Email = cs.User.Email,
                        FirstName = cs.User.FirstName,
                        LastName = cs.User.LastName,
                        IsOwner = false,
                        ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath,cs.User.ProfilePhoto)
                    }).ToList(),
                    PhysicalUsers = new List<UserInfoDto> {
                        new UserInfoDto
                        {
                            Id = c.Document.CreatedByUser.Id,
                            Email = c.Document.CreatedByUser.Email,
                            FirstName = c.Document.CreatedByUser.FirstName,
                            LastName = c.Document.CreatedByUser.LastName,
                            IsOwner = true,
                            ProfilePhoto =Path.Combine(_pathHelper.UserProfilePath,c.Document.CreatedByUser.ProfilePhoto)
                        }
                    }
                }).ToListAsync();

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
