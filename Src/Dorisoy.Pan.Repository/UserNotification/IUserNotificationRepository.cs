using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository
{
    public interface IUserNotificationRepository : IGenericRepository<UserNotification>
    {
        Task SaveUserNotification(Guid? folderId, Guid? documentId, List<Guid> users, ActionEnum action);
        Task<UserNotificationList> GetUserNotifications(NotificationSource notificationSource);
    }
}
