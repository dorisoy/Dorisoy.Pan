namespace Dorisoy.Pan.Services;
public interface IOnlineUserService
{
    Task<List<OnlinUserUserModel>> GetOnlineUsers(CancellationToken calToken = default);
}
