﻿using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Resources;
using System.Collections.Generic;
using System;
using System.Security.Claims;

namespace Dorisoy.Pan.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<UserList> GetUsers(UserResource userResource);
        Task<UserAuthDto> BuildUserAuthObject(User appUser, IList<Claim> claims);
        Task<UserList> GetSharedUsers(UserResource userResource, List<Guid> folderUsers, List<Guid> documentUsers);
        Task<bool> ResetPasswordAsync(User user, string password);
    }
}
