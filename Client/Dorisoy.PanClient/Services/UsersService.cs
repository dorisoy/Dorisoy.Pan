using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dorisoy.PanClient.Commands;
using Dorisoy.PanClient.Common;
using Dorisoy.PanClient.Data.Contexts;

namespace Dorisoy.PanClient.Services;

public class UsersService : IUsersService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;
    private readonly MakeRequest _makeRequest;
    private static string _url;
    private readonly AppSettings _appSettings;

    private readonly SourceCache<UserModel, Guid> _employees;
    private readonly SourceCache<RoleModel, Guid> _roles;
    private readonly SourceCache<UserRoleModel, string> _userRoles;

    public IObservable<IChangeSet<UserModel, Guid>> Connect() => _employees.Connect();
    public IObservable<IChangeSet<RoleModel, Guid>> RoleConnect() => _roles.Connect();
    public IObservable<IChangeSet<UserRoleModel, string>> UserRoleConnect() => _userRoles.Connect();

    public UsersService(IDbContextFactory<CaptureManagerContext> contextFactory,
        MakeRequest makeRequest, 
        IMapper mapper, 
        AppSettings appSettings)
    {
        _makeRequest = makeRequest;
        _mapper = mapper;
        _contextFactory = contextFactory;
        _appSettings = appSettings;
        _employees = new SourceCache<UserModel, Guid>(e => e.Id);
        _roles = new SourceCache<RoleModel, Guid>(e => e.Id);
        _userRoles = new SourceCache<UserRoleModel, string>(e => e.RoleName);
        _url = _appSettings.HostUrl.EndsWith("/") ? (_appSettings.HostUrl + "api") : (_appSettings.HostUrl + "/api");

        using var context = _contextFactory.Create();
        var users = _mapper.ProjectTo<UserModel>(context.Users);
        _employees.AddOrUpdate(users);

        var roles = _mapper.ProjectTo<RoleModel>(context.Roles);
        _roles.AddOrUpdate(roles);
    }

    /// <summary>
    /// 获取用户
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserModel>> GetUsers()
    {
        return await Task.Run(() =>
        {
            var users = new List<UserModel>();
            using (var context = _contextFactory.Create())
            {
                var result = _mapper.ProjectTo<UserModel>(context.Users);
                users = result.ToList();
                users.ForEach(u =>
                {
                    var dept = context.Departments.Where(s => s.Id == u.DepartmentId).FirstOrDefault();
                    u.Department = _mapper.Map<DepartmentModel>(dept);
                });
            }

            return users;
        });
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ServiceResult<UserModel>> AddAsync(UserModel model)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = new User();
                _mapper.Map(model, entity);

                context.Users.Add(entity);
                context.SaveChanges();

                _mapper.Map(entity, model);

                _employees.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ServiceResult<UserModel>> UpdateAsync(UserModel model)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Users.First(e => e.Id == model.Id);
                _mapper.Map(model, entity);

                context.SaveChanges();

                _mapper.Map(entity, model);

                _employees.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task DeleteAsync(UserModel model)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                //var entity = context.Users.First(e => e.Id == model.Id);
                var entity = context.Set<User>().Find(model.Id);
                if (entity != null)
                {
                    context.Entry(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }

    /// <summary>
    /// 判断用户名称是否存在
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public bool UsernameIsFree(Guid id, string username)
    {
        using (var context = _contextFactory.Create())
        {
            var count = context.Users.Count(e => e.UserName == username) > 0;
            return count;
        }
    }

    /// <summary>
    /// 根据用户ID获取用户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<User> FindByIdAsync(Guid id)
    {
        using (var context = _contextFactory.Create())
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Id == id);
            return user;
        }
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
                 .Where(s => s.UserId == user.Id && s.Role.NormalizedName == role)
                 .CountAsync();
            return exits > 0;
        }
    }

    /// <summary>
    /// 根据用户ID获取用户角色
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IResult<UserRolesResponse>> GetRolesAsync(Guid userId)
    {
        using (var context = _contextFactory.Create())
        {
            var viewModel = new List<UserRoleModel>();
            var user = await FindByIdAsync(userId);
            var roles = await context.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRoleModel
                {
                    Name = role.Name,
                    RoleName = role.NormalizedName,
                    RoleDescription = role.Description
                };

                if (await IsInRoleAsync(user, role.NormalizedName))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }

                viewModel.Add(userRolesViewModel);
            }

            var result = new UserRolesResponse { UserRoles = viewModel };

            return await Result<UserRolesResponse>.SuccessAsync(result);
        }
    }


    public async Task<IList<Role>> GetRolesAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        using (var context = _contextFactory.Create())
        {
            var roles = await context.UserRoles
                .Where(s => s.UserId == user.Id)
                .Select(s => s.Role)
                .ToListAsync();

            return roles;
        }
    }

    public async Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<Role> roles)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (roles == null)
            throw new ArgumentNullException(nameof(roles));

        foreach (var role in roles)
        {
            if (!await IsInRoleAsync(user, role.Name).ConfigureAwait(false))
            {
                return IdentityResult.Failed();
            }
            await RemoveFromRoleAsync(user, role.Name).ConfigureAwait(false);
        }

        using (var context = _contextFactory.Create())
        {
            context.SaveChanges();
        }

        return IdentityResult.Success;
    }
    public async Task RemoveFromRoleAsync(User user, string roleName)
    {
        using (var context = _contextFactory.Create())
        {
            var entity = await context.UserRoles
                 .Include(s => s.Role)
                 .Where(s => s.UserId == user.Id && s.Role.Name == roleName)
                 .SingleOrDefaultAsync();

            if (entity != null)
            {
                context.Entry(entity).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
    }


    public async Task<IdentityResult> AddToRolesAsync(User user, IEnumerable<string> roles)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (roles == null)
        {
            throw new ArgumentNullException(nameof(roles));
        }

        foreach (var role in roles.Distinct())
        {
            if (await IsInRoleAsync(user, role).ConfigureAwait(false))
            {
                return IdentityResult.Failed();
            }

            await AddToRoleAsync(user, role).ConfigureAwait(false);
        }

        //using (var context = _contextFactory.Create())
        //{
        //    context.SaveChanges();
        //}
        return IdentityResult.Success;
    }

    public async Task AddToRoleAsync(User user, string roleName)
    {
        using (var context = _contextFactory.Create())
        {
            var role = await context.Roles
                 .Where(s => s.NormalizedName == roleName)
                 .SingleOrDefaultAsync();

            if (role != null)
            {
                //SqliteException: SQLite Error 19: 'UNIQUE constraint failed: Users.Id'.
                var userRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                };
                context.UserRoles.Add(userRole);
                context.SaveChanges();
            }
        }
    }

    /// <summary>
    /// 获取全部用户
    /// </summary>
    /// <param name="calToken"></param>
    /// <returns></returns>
    public async Task<List<UserModel>> GetAllUsers(CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IUserApi>(_url, true);
            var result = await _makeRequest.Start(api.GetAllUsers(calToken), calToken);
            var users = _mapper.ProjectTo<UserModel>(result.AsQueryable());
            return users.ToList();
        }
        catch (Exception e)
        {
            return null;
        }
    }


    /// <summary>
    /// 更新用户角色
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
    {
        var userId = Globals.CurrentUser.Id;

        var user = await FindByIdAsync(request.UserId);

        if (user.Email == "admin@sinol.com")
            return await Result.FailAsync("不允许操作.");

        //获取当前选择用户的角色
        var roles = await GetRolesAsync(user);

        //选择的角色
        var selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

        //当前登录用户
        var currentUser = await FindByIdAsync(userId);

        //用户是否存在角色
        if (!await IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
        {
            var tryToAddAdministratorRole = selectedRoles
                .Any(x => x.RoleName == RoleConstants.AdministratorRole);

            var userHasAdministratorRole = roles.Any(x => x.NormalizedName == RoleConstants.AdministratorRole);
            if (tryToAddAdministratorRole && !userHasAdministratorRole || !tryToAddAdministratorRole && userHasAdministratorRole)
            {
                return await Result.FailAsync("如果您没有管理员角色，则不允许添加或删除该角色.");
            }
        }
        var result = await RemoveFromRolesAsync(user, roles);

        result = await AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));

        return await Result.SuccessAsync("角色已经更新");
    }


    /// <summary>
    /// 远程添加用户
    /// </summary>
    /// <param name="addUserCommand"></param>
    /// <param name="calToken"></param>
    /// <returns></returns>
    public async Task<UserDto> AddUser(AddUserCommand addUserCommand, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IUserApi>(_url, true);
            var result = await _makeRequest.Start(api.AddUser(addUserCommand, calToken), calToken);
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// 远程更新用户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateUserCommand"></param>
    /// <param name="calToken"></param>
    /// <returns></returns>
    public async Task<UserDto> UpdateUser(Guid id, UpdateUserCommand updateUserCommand, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IUserApi>(_url, true);
            var result = await _makeRequest.Start(api.UpdateUser(id, updateUserCommand, calToken), calToken);
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }

}
