using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;

namespace Dorisoy.Pan.Repository
{
    public interface IConnectionMappingRepository
    {
        bool AddUpdate(UserInfoToken tempUserInfo, string connectionId);
        void Remove(UserInfoToken tempUserInfo);
        IEnumerable<UserInfoToken> GetAllUsersExceptThis(UserInfoToken tempUserInfo);
        UserInfoToken GetUserInfo(UserInfoToken tempUserInfo);
        UserInfoToken GetUserInfoByName(Guid id);
        UserInfoToken GetUserInfoByConnectionId(string connectionId);
        List<UserInfoToken> GetOnlineUserByList(List<Guid> users);
        Task SendFolderNotification(List<Guid> users, Guid folderId);
        Task RemovedFolderNotification(List<Guid> users, Guid folderId);

    }
}
