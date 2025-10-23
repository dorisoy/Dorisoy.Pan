using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Data.Resources;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository
{
    public class UserNotificationRepository : GenericRepository<UserNotification, DocumentContext>,
          IUserNotificationRepository
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly PathHelper _pathHelper;

        public UserNotificationRepository(
            IUnitOfWork<DocumentContext> uow,
            UserInfoToken userInfoToken,
            IPropertyMappingService propertyMappingService,
            PathHelper pathHelper
            ) : base(uow)
        {
            _userInfoToken = userInfoToken;
            _propertyMappingService = propertyMappingService;
            _pathHelper = pathHelper;
        }

        public void SaveUserNotification(Guid? folderId, Guid? documentId, List<Guid> users, ActionEnum action)
        {
            List<UserNotification> lstUserNotification = new List<UserNotification>();
            UserNotification UserNotification;
            foreach (var user in users)
            {
                UserNotification = new UserNotification
                {
                    FromUserId = _userInfoToken.Id,
                    ToUserId = user,
                    FolderId = folderId,
                    DocumentId = documentId,
                    Action = action,
                    IsRead = false,
                    Status = NotificationStatusEnum.InQueue,
                    CreatedDate = DateTime.Now
                };
                lstUserNotification.Add(UserNotification);
            }
            if (lstUserNotification.Count > 0)
            {
                AddRange(lstUserNotification);
            }
        }

        public async Task<UserNotificationList> GetUserNotifications(NotificationSource notificationSource)
        {
            var collectionBeforePaging = All
                   .Include(c => c.VirtualFolder)
                          .Include(c => c.Document)
                          .OrderByDescending(c => c.CreatedDate)
                          .Where(c => c.ToUserId == _userInfoToken.Id);
            collectionBeforePaging =
               collectionBeforePaging.ApplySort(notificationSource.OrderBy,
               _propertyMappingService.GetPropertyMapping<UserNotificationDto, UserNotification>());


            var userNotifications = new UserNotificationList(_pathHelper);
            return await userNotifications.Create(
                collectionBeforePaging,
                notificationSource.Skip,
                notificationSource.PageSize);
        }
    }
}
