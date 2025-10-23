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
        void RemoveInId(Guid userId);

        /// <summary>
        /// 获取除此之外的所有用户
        /// </summary>
        /// <param name="tempUserInfo"></param>
        /// <returns></returns>
        IEnumerable<UserInfoToken> GetAllUsersExceptThis(UserInfoToken tempUserInfo);

        IEnumerable<UserInfoToken> GetAllUsers();
        UserInfoToken GetUserInfo(UserInfoToken tempUserInfo);
        UserInfoToken GetUserInfoByName(Guid id);
        UserInfoToken GetUserInfoByConnectionId(string connectionId);
        UserInfoToken GetUserInfoByUserId(Guid uid);
        List<UserInfoToken> GetOnlineUserByList(List<Guid> users);
        Task SendFolderNotification(List<Guid> users, Guid folderId);
        Task RemovedFolderNotification(List<Guid> users, Guid folderId);

    }
}
