using AutoMapper;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetAllRecentActivityQueryHandler : IRequestHandler<GetAllRecentActivityQuery, List<RecentActivityDto>>
    {

        private readonly IRecentActivityRepository _recentActivityRepository;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        public GetAllRecentActivityQueryHandler(
            IRecentActivityRepository recentActivityRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            PathHelper pathHelper
            )
        {
            _recentActivityRepository = recentActivityRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _pathHelper = pathHelper;
        }
        public async Task<List<RecentActivityDto>> Handle(GetAllRecentActivityQuery request, CancellationToken cancellationToken)
        {
            var entities = await _recentActivityRepository.All
                          .Include(c => c.VirtualFolder)
                            .ThenInclude(c => c.PhysicalFolder)
                                .ThenInclude(c => c.PhysicalFolderUsers)
                          .Include(c => c.Document)
                            .ThenInclude(c => c.SharedDocumentUsers)
                          .Include(c => c.Document)
                            .ThenInclude(c => c.DocumentDeleteds)
                          .Include(c => c.Document)
                            .ThenInclude(c => c.CreatedByUser)
                          .OrderByDescending(c => c.CreatedDate)
                          .Where(c => c.UserId == _userInfoToken.Id)
                          .Take(30)
                          .Select(c => new RecentActivityDto
                          {
                              Id = c.Id,
                              CreatedDate = c.CreatedDate,
                              Action = c.Action,
                              Name = c.VirtualFolder == null ? c.Document.Name : c.VirtualFolder.Name,
                              IsShared = c.VirtualFolder == null ? false : c.VirtualFolder.IsShared,
                              FolderId = c.FolderId,
                              DocumentId = c.DocumentId,
                              ThumbnailPath = c.DocumentId == null ? "" : Path.Combine(_pathHelper.DocumentPath, c.Document.ThumbnailPath),
                              Document = c.DocumentId == null ? null : _mapper.Map<DocumentDto>(c.Document),
                              CreatedByUser = c.DocumentId != null ? c.Document.CreatedByUser : null,
                              DeletedUserIds = c.DocumentId != null ? c.Document.DocumentDeleteds.Select(d => d.UserId).ToList() : null,
                              Users = c.VirtualFolder != null ? c.VirtualFolder.PhysicalFolder.PhysicalFolderUsers.Select(cs => new UserInfoDto
                              {
                                  Email = cs.User.Email,
                                  RaleName = cs.User.RaleName,
                                  Id = cs.UserId,
                                  IsOwner = cs.UserId == c.VirtualFolder.PhysicalFolder.CreatedBy
                              }).ToList()
                              :
                               c.Document.SharedDocumentUsers.Select(cs => new UserInfoDto
                               {
                                   Id = cs.UserId,
                                   Email = cs.User.Email,
                                   RaleName = cs.User.RaleName,
                                   IsOwner = c.Document.CreatedBy == cs.UserId
                               }).ToList()
                          })
                          .ToListAsync();

            entities.ForEach(c =>
            {
                if (c.DocumentId != null)
                {
                    c.Users = c.Users.Where(d => c.DeletedUserIds.Any(du => du == d.Id)).ToList();
                    if (!c.Users.Any(u => u.Id == c.CreatedByUser.Id))
                    {
                        c.Users.Add(
                            new UserInfoDto
                            {
                                Id = c.CreatedByUser.Id,
                                Email = c.CreatedByUser.Email,
                                RaleName = c.CreatedByUser.RaleName,
                                IsOwner = true
                            });
                    }
                }
            });
            return entities;
        }
    }
}
