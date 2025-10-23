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
    public class GetAllSharedDocumentsQueryHandler : IRequestHandler<GetAllSharedDocumentsQuery, List<DocumentDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IDocumentSharedUserRepository _documentSharedUserRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly PathHelper _pathHelper;

        public GetAllSharedDocumentsQueryHandler(UserInfoToken userInfoToken,
            IDocumentSharedUserRepository documentSharedUserRepository,
            IDocumentRepository documentRepository,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _documentSharedUserRepository = documentSharedUserRepository;
            _documentRepository = documentRepository;
            _pathHelper = pathHelper;
        }
        public async Task<List<DocumentDto>> Handle(GetAllSharedDocumentsQuery request, CancellationToken cancellationToken)
        {
            var entitiesDto = await _documentSharedUserRepository.All
                .IgnoreQueryFilters()
                .Where(c => (c.UserId == _userInfoToken.Id || c.Document.CreatedBy == _userInfoToken.Id)
                    && !c.Document.DocumentDeleteds.Any(cs => cs.UserId == _userInfoToken.Id))
                .Select(c => new DocumentDto
                {
                    Id = c.Document.Id,
                    Name = c.Document.Name,
                    Extension = c.Document.Extension,
                    PhysicalFolderId = c.Document.PhysicalFolderId,
                    ThumbnailPath = Path.Combine(_pathHelper.DocumentPath, c.Document.ThumbnailPath),
                    ModifiedDate = c.Document.ModifiedDate.Value,
                    Size = c.Document.Size,
                    IsStarred = c.Document.DocumentStarreds.Any(cs => cs.UserId == _userInfoToken.Id),
                    DeletedUserIds = c.Document.DocumentDeleteds.Where(cd => cd.IsDeleted).Select(c => c.UserId).ToList(),
                    Users = c.Document.SharedDocumentUsers.Select(cs => new UserInfoDto
                    {
                        Id = cs.UserId,
                        Email = cs.User.Email,
                        RaleName = cs.User.RaleName,
                        IsOwner = false,
                        ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath,cs.User.ProfilePhoto)
                    }).ToList(),
                    PhysicalUsers = new List<UserInfoDto> {
                        new UserInfoDto
                        {
                            Id = c.Document.CreatedByUser.Id,
                            Email = c.Document.CreatedByUser.Email,
                            RaleName = c.Document.CreatedByUser.RaleName,
                            IsOwner = true,
                            ProfilePhoto= Path.Combine(_pathHelper.UserProfilePath,c.Document.CreatedByUser.ProfilePhoto)
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
