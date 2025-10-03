namespace Dorisoy.PanClient.Services
{
    public interface IRoleClaimService
    {
        Task<Result<string>> DeleteAsync(int id);
        Task<Result<List<RoleClaimResponse>>> GetAllAsync();
        Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(Guid roleId);
        Task<Result<RoleClaimResponse>> GetByIdAsync(int id);
        Task<int> GetCountAsync();
        Task<Result<string>> SaveAsync(RoleClaimResponse request);
    }
}