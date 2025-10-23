using System.Security.Claims;
using AvaloniaEdit.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dorisoy.Pan.Data.Contexts;
using Role = Dorisoy.Pan.Data.Role;

namespace Dorisoy.Pan.Services;

public class RoleService : IRoleService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;
    private readonly SourceCache<RoleModel, Guid> _roles;
    private readonly IRoleClaimService _roleClaimService;

    public IObservable<IChangeSet<RoleModel, Guid>> Connect() => _roles.Connect();

    public RoleService(IDbContextFactory<CaptureManagerContext> contextFactory,
        IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;
        _roleClaimService = Locator.Current.GetService<IRoleClaimService>();
        _roles = new SourceCache<RoleModel, Guid>(e => e.Id);

        using (var context = _contextFactory.Create())
        {
            var users = _mapper.ProjectTo<RoleModel>(context.Roles);
            _roles.AddOrUpdate(users);
        }
    }

    /// <summary>
    /// 获取全部角色
    /// </summary>
    /// <returns></returns>
    public async Task<Result<List<RoleModel>>> GetAllAsync()
    {
        var roles = new List<RoleModel>();
        using (var context = _contextFactory.Create())
        {
            var result = _mapper.ProjectTo<RoleModel>(context.Roles);
            roles = result.ToList();
        }
        return await Result<List<RoleModel>>.SuccessAsync(roles);
    }

    /// <summary>
    /// 获取全部角色
    /// </summary>
    /// <returns></returns>
    public async Task<List<RoleModel>> GetRoles()
    {
        return await Task.Run(() =>
        {
            var roles = new List<RoleModel>();
            using (var context = _contextFactory.Create())
            {
                var result = _mapper.ProjectTo<RoleModel>(context.Roles);
                roles = result.ToList();
            }
            return roles;
        });
    }

    /// <summary>
    /// 角色名是否存在
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool RoleNameIsFree(Guid id, string name)
    {
        using (var context = _contextFactory.Create())
        {
            var count = context.Roles.Count(e => e.Id == id && e.Name == name) > 1;
            return !count;
        }
    }


    /// <summary>
    /// 根据角色ID获取角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Result<RoleModel>> GetByIdAsync(Guid id)
    {
        using (var context = _contextFactory.Create())
        {
            var roles = await context.Roles.SingleOrDefaultAsync(x => x.Id == id);
            var rolesResponse = _mapper.Map<RoleModel>(roles);
            return await Result<RoleModel>.SuccessAsync(rolesResponse);
        }
    }

    /// <summary>
    /// 根据角色ID获取角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Role> FindByIdAsync(Guid id)
    {
        using (var context = _contextFactory.Create())
        {
            var roles = await context.Roles.SingleOrDefaultAsync(x => x.Id == id);
            return roles;
        }
    }


    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Result<int>> DeleteAsync(Guid id)
    {
        using (var context = _contextFactory.Create())
        {
            var existingRole = await context.Roles.Where(s => s.Id == id).FirstAsync();
            if (existingRole.Name != RoleConstants.AdministratorRole && existingRole.Name != RoleConstants.BasicRole)
            {
                bool roleIsNotUsed = true;
                var allUsers = await context.Users.ToListAsync();
                foreach (var user in allUsers)
                {
                    if (await IsInRoleAsync(user, existingRole.Name))
                    {
                        roleIsNotUsed = false;
                    }
                }
                if (roleIsNotUsed)
                {
                    await DeleteAsync(existingRole);

                    return await Result<int>.SuccessAsync($"{existingRole.Name} 删除成功");
                }
                else
                {
                    return await Result<int>.SuccessAsync(string.Format("Not allowed to delete {0} Role as it is being used.", existingRole.Name));
                }
            }
            else
            {
                return await Result<int>.SuccessAsync(string.Format("Not allowed to delete {0} Role.", existingRole.Name));
            }
        }
    }


    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task DeleteAsync(Role role)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                if (role != null)
                {
                    context.Entry(role).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }

    /// <summary>
    /// 用户是否存在角色
    /// </summary>
    /// <param name="user"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public async Task<bool> IsInRoleAsync(User user, string role)
    {
        using (var context = _contextFactory.Create())
        {
            var exits = await context.UserRoles
                 .Include(x => x.Role)
                 .Where(s => s.UserId == user.Id && s.Role.Name == role)
                 .CountAsync();
            return exits > 0;
        }
    }


    /// <summary>
    /// 获取当前角色的全部权限
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<Result<PermissionResponse>> GetAllPermissionsAsync(Guid roleId)
    {
        return await Task.Run(async () =>
        {
            var model = new PermissionResponse();

            //获取全部权限
            var alls = GetAllPermissions().AsQueryable();

            var allPermissions = _mapper.ProjectTo<RoleClaimResponseModel>(alls).ToList();

            using (var context = _contextFactory.Create())
            {
                var role = await context.Roles.Where(s => s.Id == roleId).FirstOrDefaultAsync();
                if (role != null)
                {
                    model.RoleId = role.Id;
                    model.RoleName = role.Name;

                    //根据角色获取角色权限
                    var roleClaimsResult = await GetAllByRoleIdAsync(role.Id);
                    if (roleClaimsResult.Succeeded)
                    {
                        //角色权限
                        var roleClaims = roleClaimsResult.Data;

                        //全部角色权限值
                        var allClaimValues = allPermissions.Select(a => a.Value).ToList();

                        //角色权限值
                        var roleClaimValues = roleClaims.Select(a => a.Value).ToList();

                        //授权Claim(交集)
                        var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();

                        //遍历权限
                        foreach (var permission in allPermissions)
                        {
                            //如果授权Claim包含在角色权限中
                            if (authorizedClaims.Any(a => a == permission.Value))
                            {
                                //标记为选择状态
                                permission.Selected = true;

                                //描述信息
                                var roleClaim = roleClaims.SingleOrDefault(a => a.Value == permission.Value);
                                if (roleClaim?.Description != null)
                                {
                                    permission.Description = roleClaim.Description;
                                }
                                //分组
                                if (roleClaim?.Group != null)
                                {
                                    permission.Group = roleClaim.Group;
                                }
                            }
                        }
                    }
                    else
                    {
                        model.RoleClaims = new List<RoleClaimResponseModel>();

                        return await Result<PermissionResponse>.FailAsync(roleClaimsResult.Messages);
                    }
                }
            }

            model.RoleClaims = allPermissions;

            return await Result<PermissionResponse>.SuccessAsync(model);
        });
    }

    /// <summary>
    /// 根据角色获取角色权限
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(Guid roleId)
    {
        var roleClaimsResponse = new List<RoleClaimResponse>();
        using (var context = _contextFactory.Create())
        {
            var roleClaims = await context.RoleClaims
            .Include(x => x.Role)
            .Where(x => x.RoleId == roleId)
            .ToListAsync();
            roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
        }
        return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
    }

    /// <summary>
    /// 获取全部权限
    /// </summary>
    /// <returns></returns>
    private List<RoleClaimResponse> GetAllPermissions()
    {
        var allPermissions = new List<RoleClaimResponse>();
        allPermissions.GetAllPermissions();
        return allPermissions;
    }

    /// <summary>
    /// 角色名是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<bool> FindByNameAsync(string name)
    {
        return await Task.Run(async () =>
         {
             var exits = false;
             using (var context = _contextFactory.Create())
             {
                 var count = await context.Roles.CountAsync(e => e.Name == name);
                 exits = count > 0;
             }
             return exits;
         });
    }

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ServiceResult<RoleModel>> AddRoleAsync(RoleModel model)
    {
        return await Task.Run(() =>
        {
            var user = Globals.CurrentUser;
            using (var context = _contextFactory.Create())
            {
                var entity = new Role();
                entity.CreatedBy = user.Id;
                entity.CreatedDate = DateTime.Now;

                _mapper.Map(model, entity);

                context.Roles.Add(entity);
                context.SaveChanges();

                _mapper.Map(entity, model);

                _roles.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ServiceResult<RoleModel>> UpdateRoleAsync(RoleModel model)
    {
        return await Task.Run(() =>
        {
            var user = Globals.CurrentUser;
            using (var context = _contextFactory.Create())
            {
                var entity = context.Roles.First(e => e.Id == model.Id);

                entity.ModifiedBy = user.Id;
                entity.ModifiedDate = DateTime.Now;

                _mapper.Map(model, entity);

                context.SaveChanges();

                _mapper.Map(entity, model);

                _roles.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }


    /// <summary>
    /// 保存角色
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<Result<int>> SaveAsync(RoleModel model)
    {
        if (model.Id == Guid.Empty)
        {
            var existingRole = await FindByNameAsync(model.Name);
            if (existingRole)
                return await Result<int>.FailAsync("Similar Role already exists.");

            var response = await AddRoleAsync(model);
            if (response.Succeeded)
            {
                return await Result<int>.SuccessAsync(string.Format("Role {0} Created.", model.Name));
            }
            else
            {
                return await Result<int>.FailAsync(response.Errors.Select(e => e.Message).ToList());
            }
        }
        else
        {
            var existingRole = await FindByIdAsync(model.Id);
            if (existingRole.Name == RoleConstants.AdministratorRole || existingRole.Name == RoleConstants.BasicRole)
            {
                return await Result<int>.FailAsync(string.Format("Not allowed to modify {0} Role.", existingRole.Name));
            }

            existingRole.Name = model.Name;
            existingRole.NormalizedName = model.Name.ToUpper();
            existingRole.Description = model.Description;

            using (var context = _contextFactory.Create())
            {
                _mapper.Map(model, existingRole);
                context.SaveChanges();

                _mapper.Map(existingRole, model);
                _roles.AddOrUpdate(model);
            }
            return await Result<int>.SuccessAsync(string.Format("Role {0} Updated.", existingRole.Name));
        }
    }


    /// <summary>
    /// 获取角色数量
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetCountAsync()
    {
        using (var context = _contextFactory.Create())
        {
            var count = await context.Roles.CountAsync();
            return count;
        }
    }

    /// <summary>
    /// 获取指定角色的Claim
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public async Task<IList<Claim>> GetClaimsAsync(Role role)
    {
        using (var context = _contextFactory.Create())
        {
            var claims = await context
                .RoleClaims
                .Where(s => s.RoleId == role.Id)
                .Select(s => s.ToClaim())
                .ToListAsync();
            return claims;
        }
    }

    /// <summary>
    /// 移除当前角色的权限
    /// </summary>
    /// <param name="role"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task RemoveClaimAsync(Role role, Claim claim)
    {
        using (var context = _contextFactory.Create())
        {
            var entity = await context.RoleClaims
                .Where(s => s.ClaimValue == claim.Value && s.ClaimType == claim.Type && s.RoleId == role.Id)
                .FirstAsync();

            if (entity != null)
            {
                context.Entry(entity).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
    }

    /// <summary>
    /// 给角色添加权限
    /// </summary>
    /// <param name="role"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult> AddClaimAsync(Role role, Claim claim, string group = "", string rmark = "")
    {
        return await Task.Run(() =>
        {
            try
            {
                var user = Globals.CurrentUser;
                using (var context = _contextFactory.Create())
                {
                    var entity = new RoleClaim();

                    entity.Group = group;
                    entity.Description = rmark;

                    entity.RoleId = role.Id;

                    entity.ClaimType = claim.Type;
                    entity.ClaimValue = claim.Value;

                    entity.CreatedBy = user.Id;
                    entity.CreatedDate = DateTime.Now;

                    context.RoleClaims.Add(entity);
                    context.SaveChanges();
                }
                return IdentityResult.Success;
            }
            catch (Exception)
            {
                return IdentityResult.Failed();
            }
        });
    }

    public async Task<IdentityResult> AddPermissionClaim(Role role, string permission, string group, string rmark)
    {
        var allClaims = await GetClaimsAsync(role);
        if (!allClaims.Any(a => a.Type == ApplicationClaimTypes.Permission && a.Value == permission))
        {
            //添加权限
            return await AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission), group, rmark);
        }
        return IdentityResult.Failed();
    }

    /// <summary>
    /// 更新权限
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Result<int>> UpdatePermissionsAsync(PermissionRequest request)
    {
        using (var context = _contextFactory.Create())
        {
            try
            {
                var user = Globals.CurrentUser;
                var userId = user.Id;

                var errors = new List<string>();

                //选择绑定的角色
                var role = await FindByIdAsync(request.RoleId);
                //判断是否管理员
                if (role.Name == RoleConstants.AdministratorRole)
                {
                    var currentUser = await context.Users.SingleAsync(x => x.Id == userId);
                    if (await IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
                    {
                        //稍后汉化：Not allowed to modify Permissions for this Role
                        return await Result<int>.FailAsync("不允许修改此角色的权限.");
                    }
                }

                //选择的角色权限
                var selectedClaims = request.RoleClaims.Where(a => a.Selected).ToList();
                if (role.Name == RoleConstants.AdministratorRole)
                {
                    if (!selectedClaims.Any(x => x.Value == Permissions.Roles.View)
                       || !selectedClaims.Any(x => x.Value == Permissions.RoleClaims.View)
                       || !selectedClaims.Any(x => x.Value == Permissions.RoleClaims.Edit))
                    {
                        return await Result<int>.FailAsync(string.Format(
                            "不允许取消选择此角色的｛0｝、｛1｝或｛2｝.",
                            Permissions.Roles.View,
                            Permissions.RoleClaims.View,
                            Permissions.RoleClaims.Edit));
                    }
                }

                //获取当前角色的权限
                var claims = await GetClaimsAsync(role);
                //先移除角色权限
                foreach (var claim in claims)
                {
                    await RemoveClaimAsync(role, claim);
                }

                //添加已选择权限
                foreach (var claim in selectedClaims)
                {
                    var addResult = await AddPermissionClaim(role, claim.Value, claim.Group, claim.Rmark);
                    if (!addResult.Succeeded)
                    {
                        errors.AddRange(addResult.Errors.Select(e => e.Description));
                    }
                }

                //重新获取当前选择的角色的角色权限
                var addedClaims = await _roleClaimService.GetAllByRoleIdAsync(role.Id);
                if (addedClaims.Succeeded)
                {
                    foreach (var claim in selectedClaims)
                    {
                        var addedClaim = addedClaims.Data.SingleOrDefault(x => x.Type == claim.Type && x.Value == claim.Value);
                        if (addedClaim != null)
                        {
                            claim.Id = addedClaim.Id;
                            claim.RoleId = addedClaim.RoleId;

                            //SqliteException: SQLite Error 19: 'FOREIGN KEY constraint failed'.

                            var data = _mapper.Map<RoleClaimResponse>(claim);
                            var saveResult = await _roleClaimService.SaveAsync(data);
                            if (!saveResult.Succeeded)
                            {
                                errors.AddRange(saveResult.Messages);
                            }
                        }
                    }
                }
                else
                {
                    errors.AddRange(addedClaims.Messages);
                }

                if (errors.Any())
                {
                    return await Result<int>.FailAsync(errors);
                }

                //Permissions Updated
                return await Result<int>.SuccessAsync("已更新权限.");
            }
            catch (Exception ex)
            {
                return await Result<int>.FailAsync(ex.Message);
            }
        }
    }


    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task DeleteRoleAsync(RoleModel model)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Set<Role>().Find(model.Id);
                if (entity != null)
                {
                    context.Entry(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }

    /// <summary>
    /// 获取指定用户角色声明
    /// </summary>
    /// <param name="appUser"></param>
    /// <returns></returns>
    public async Task<List<string>> GetRoleClaims(User appUser)
    {
        return await Task.Run(async () =>
        {
            var roleClaims = new List<string>();

            using (var context = _contextFactory.Create())
            {
                var rolesIds = await context.UserRoles
                .Where(c => c.UserId == appUser.Id)
                .Select(c => c.RoleId)
                .ToListAsync();

                var lstRoleClaim = new List<RoleClaim>();

                roleClaims = await context.RoleClaims
                .Where(c => rolesIds.Contains(c.RoleId))
                .Select(c => c.ClaimType)
                .ToListAsync();
            }
            return roleClaims;
        });
    }
}
