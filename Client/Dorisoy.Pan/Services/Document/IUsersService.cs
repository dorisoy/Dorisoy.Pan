using Microsoft.AspNetCore.Identity;
using Role = Dorisoy.PanClient.Data.Role;

namespace Dorisoy.PanClient.Services;

public interface IUsersService
{
    Task<ServiceResult<UserModel>> AddAsync(UserModel model);
    Task AddToRoleAsync(User user, string roleName);
    Task<IdentityResult> AddToRolesAsync(User user, IEnumerable<string> roles);
    IObservable<IChangeSet<UserModel, Guid>> Connect();
    Task DeleteAsync(UserModel model);
    Task<User> FindByIdAsync(Guid id);
    Task<IResult<UserRolesResponse>> GetRolesAsync(Guid userId);
    Task<IList<Role>> GetRolesAsync(User user);
    Task<List<UserModel>> GetUsers();
    Task<bool> IsInRoleAsync(User user, string role);
    Task RemoveFromRoleAsync(User user, string roleName);
    Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<Role> roles);
    IObservable<IChangeSet<RoleModel, Guid>> RoleConnect();
    Task<ServiceResult<UserModel>> UpdateAsync(UserModel model);
    Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);
    bool UsernameIsFree(Guid id, string username);
    IObservable<IChangeSet<UserRoleModel, string>> UserRoleConnect();
    Task<List<UserModel>> GetAllUsers(CancellationToken calToken = default);
    Task<UserDto> AddUser(AddUserCommand addUserCommand, CancellationToken calToken = default);
    Task<UserDto> UpdateUser(Guid id, UpdateUserCommand updateUserCommand, CancellationToken calToken = default);
}
