using Refit;

namespace Dorisoy.PanClient.Services;

public interface IAuthenticationApi
{
    [Post("/user/login")]
    Task<UserAuthDto> LoginAsync(LoginModel model, CancellationToken calToken = default);

    [Post("/user/logout/{userId}")]
    Task<UserModel> LogOutAsync(RevokeTokenRequest model, Guid userId, CancellationToken calToken = default);

    [Post("/refresh-token")]
    Task<UserAuthDto> RefreshTokenAsync(string rtoken, CancellationToken calToken = default);

}
