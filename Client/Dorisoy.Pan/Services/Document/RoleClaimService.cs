using Microsoft.EntityFrameworkCore;
using Dorisoy.Pan.Data.Contexts;

namespace Dorisoy.Pan.Services;

public class RoleClaimService : IRoleClaimService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;

    public RoleClaimService(
        IDbContextFactory<CaptureManagerContext> contextFactory,
        IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;
    }

    public async Task<Result<List<RoleClaimResponse>>> GetAllAsync()
    {
        using (var context = _contextFactory.Create())
        {
            var roleClaims = await context.RoleClaims.ToListAsync();
            var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }
    }

    public async Task<int> GetCountAsync()
    {
        using (var context = _contextFactory.Create())
        {
            var count = await context.RoleClaims.CountAsync();
            return count;
        }
    }

    public async Task<Result<RoleClaimResponse>> GetByIdAsync(int id)
    {
        using (var context = _contextFactory.Create())
        {
            var roleClaim = await context.RoleClaims
            .SingleOrDefaultAsync(x => x.Id == id);
            var roleClaimResponse = _mapper.Map<RoleClaimResponse>(roleClaim);
            return await Result<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
        }
    }

    /// <summary>
    /// 获取指定角色的角色声明
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(Guid roleId)
    {
        using (var context = _contextFactory.Create())
        {
            var roleClaims = await context.RoleClaims
            .Include(x => x.Role)
            .Where(x => x.RoleId == roleId)
            .ToListAsync();

            var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);

            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }
    }

    /// <summary>
    /// 保存角色声明
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Result<string>> SaveAsync(RoleClaimResponse request)
    {
        using (var context = _contextFactory.Create())
        {


            if (string.IsNullOrWhiteSpace(request.RoleId.ToString()))
                return await Result<string>.FailAsync("角色不能为空.");

            if (request.Id == 0)
            {
                var existingRoleClaim =
                    await context.RoleClaims
                        .SingleOrDefaultAsync(x =>
                            x.RoleId == request.RoleId && x.ClaimType == request.Type && x.ClaimValue == request.Value);

                if (existingRoleClaim != null)
                {
                    return await Result<string>.FailAsync("类似的角色声明已存在.");
                }

                var roleClaim = _mapper.Map<RoleClaim>(request);

                roleClaim.CreatedBy = Globals.CurrentUser.Id;
                roleClaim.CreatedDate = DateTime.Now;
                roleClaim.ModifiedBy = Globals.CurrentUser.Id;
                roleClaim.ModifiedDate = DateTime.Now;

                await context.RoleClaims.AddAsync(roleClaim);

                await context.SaveChangesAsync();

                return await Result<string>.SuccessAsync(string.Format("已创建角色声明｛0｝.", request.Value));
            }
            else
            {
                var existingRoleClaim =
                    await context.RoleClaims
                        .Include(x => x.Role)
                        .SingleOrDefaultAsync(x => x.Id == request.Id);
                if (existingRoleClaim == null)
                {
                    return await Result<string>.SuccessAsync("Role Claim does not exist.");
                }
                else
                {
                    existingRoleClaim.ClaimType = request.Type;
                    existingRoleClaim.ClaimValue = request.Value;
                    existingRoleClaim.Group = request.Group;
                    existingRoleClaim.Description = request.Description;
                    existingRoleClaim.RoleId = request.RoleId;
                    existingRoleClaim.ModifiedBy = Globals.CurrentUser.Id;
                    existingRoleClaim.ModifiedDate = DateTime.Now;

                    context.RoleClaims.Update(existingRoleClaim);
                    await context.SaveChangesAsync();

                    return await Result<string>.SuccessAsync(string.Format("Role Claim {0} for Role {1} updated.", request.Value, existingRoleClaim.Role.Name));
                }
            }
        }
    }

    public async Task<Result<string>> DeleteAsync(int id)
    {
        using (var context = _contextFactory.Create())
        {


            var existingRoleClaim = await context.RoleClaims
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingRoleClaim != null)
            {
                context.RoleClaims.Remove(existingRoleClaim);
                await context.SaveChangesAsync();
                return await Result<string>.SuccessAsync(string.Format("Role Claim {0} for {1} Role deleted.", existingRoleClaim.ClaimValue, existingRoleClaim.Role.Name));
            }
            else
            {
                return await Result<string>.FailAsync("Role Claim does not exist.");
            }
        }
    }
}
