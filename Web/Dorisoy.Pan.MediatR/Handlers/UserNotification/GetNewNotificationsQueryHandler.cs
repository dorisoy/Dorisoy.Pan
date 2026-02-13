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
    public class GetNewNotificationsQueryHandler : IRequestHandler<GetNewNotificationsQuery, List<UserNotificationDto>>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        public GetNewNotificationsQueryHandler(IUserNotificationRepository userNotificationRepository,
            IUserRepository userRepository,
            UserInfoToken userInfoToken,
            PathHelper pathHelper)
        {
            _userNotificationRepository = userNotificationRepository;
            _userRepository = userRepository;
            _userInfoToken = userInfoToken;
            _pathHelper = pathHelper;
        }

        public async Task<List<UserNotificationDto>> Handle(GetNewNotificationsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _userNotificationRepository.All
                          .Include(c => c.VirtualFolder)
                          .Include(c => c.Document)
                          .OrderByDescending(c => c.CreatedDate)
                          .Where(c => c.ToUserId == _userInfoToken.Id && !c.IsRead)
                          .Take(10)
                          .Select(c => new UserNotificationDto
                          {
                              Id = c.Id,
                              CreatedDate = c.CreatedDate,
                              DocumentId = c.DocumentId,
                              DocumentName = c.DocumentId.HasValue ? c.Document.Name : "",
                              DocumentThumbnail = c.DocumentId.HasValue ? Path.Combine(_pathHelper.DocumentPath, c.Document.ThumbnailPath) : "",
                              FolderId = c.FolderId,
                              FolderName = c.FolderId.HasValue ? c.VirtualFolder.Name : "",
                              FromUserId = c.FromUserId,
                              Extension = c.DocumentId.HasValue ? c.Document.Extension : "",
                          }).ToListAsync();

            var allUsersIds = entities.Where(x => x.FromUserId != Guid.Empty).Select(x => x.FromUserId).Distinct().ToList();
            var allUsers = _userRepository.All.Where(c => EF.Constant(allUsersIds).Contains(c.Id)).Select(cs => new UserInfoDto
            {
                Id = cs.Id,
                FirstName = cs.FirstName,
                LastName = cs.LastName
            }).ToList();

            entities.ForEach(entity =>
            {
                var user = allUsers.FirstOrDefault(c => c.Id == entity.FromUserId);
                if (user != null)
                {
                    entity.FromUserName = $"{user.FirstName} {user.LastName}";
                }
            });
            return entities;
        }
    }
}
