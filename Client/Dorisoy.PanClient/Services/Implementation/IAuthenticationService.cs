namespace Dorisoy.PanClient.Services;
public interface IAuthenticationService
{
    Task<UserAuthDto> LoginAsync(string userName, string password, CancellationToken calToken = default);
    Task<bool> LogOutAsync(CancellationToken calToken = default);
    Task<UserAuthDto> RefreshTokenAsync(CancellationToken calToken = default);
}
