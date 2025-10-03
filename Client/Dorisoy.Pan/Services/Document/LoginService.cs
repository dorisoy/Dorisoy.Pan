using Dorisoy.PanClient.Data.Contexts;

namespace Dorisoy.PanClient.Services;

public class LoginService : ILoginService
{
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;
    public LoginService(IDbContextFactory<CaptureManagerContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public Task<ServiceResult<UserInformationModel>> LoginAsync(ClientMode mode, string username, string password)
    {
        return Task.Run(() =>
        {
            if (mode == ClientMode.Hospital)
            {
                using (var context = _contextFactory.Create())
                {
                    var userInfo = context.Users.FirstOrDefault(e => e.UserName == username);
                    if (userInfo == null || userInfo?.PasswordHash != password)
                    {
                        return ServiceResult<UserInformationModel>.Fail("用户名/密码不正确");
                    }

                    //登记全局用户信息
                    //Globals.CurrentUser = userInfo;

                    return ServiceResult.Ok(new UserInformationModel()
                    {
                        ClientMode = mode,
                        FullName = $"{userInfo.RaleName}",
                        IsAdmin = userInfo.IsAdmin,
                        UserId = userInfo.Id
                    });
                }
            }

            return ServiceResult<UserInformationModel>.Fail("不支持的客户端模式登录方法");
        });
    }
}
