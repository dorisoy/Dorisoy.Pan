using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Role = Dorisoy.Pan.Data.Role;

namespace Dorisoy.Pan.Services;

public interface IRoleService
{
    Task<IdentityResult> AddClaimAsync(Role role, Claim claim, string group = "", string rmark = "");
    Task<IdentityResult> AddPermissionClaim(Role role, string permission, string group, string rmark);
    Task<ServiceResult<RoleModel>> AddRoleAsync(RoleModel model);
    IObservable<IChangeSet<RoleModel, Guid>> Connect();
    Task<Result<int>> DeleteAsync(Guid id);
    Task DeleteAsync(Role role);
    Task DeleteRoleAsync(RoleModel model);
    Task<Role> FindByIdAsync(Guid id);
    Task<bool> FindByNameAsync(string name);
    Task<Result<List<RoleModel>>> GetAllAsync();
    Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(Guid roleId);
    Task<Result<PermissionResponse>> GetAllPermissionsAsync(Guid roleId);
    Task<Result<RoleModel>> GetByIdAsync(Guid id);
    Task<IList<Claim>> GetClaimsAsync(Role role);
    Task<int> GetCountAsync();
    Task<List<string>> GetRoleClaims(User appUser);
    Task<List<RoleModel>> GetRoles();
    Task<bool> IsInRoleAsync(User user, string role);
    Task RemoveClaimAsync(Role role, Claim claim);
    bool RoleNameIsFree(Guid id, string name);
    Task<Result<int>> SaveAsync(RoleModel model);
    Task<Result<int>> UpdatePermissionsAsync(PermissionRequest request);
    Task<ServiceResult<RoleModel>> UpdateRoleAsync(RoleModel model);
}
