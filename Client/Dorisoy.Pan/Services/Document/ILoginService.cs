namespace Dorisoy.Pan.Services
{
    public interface ILoginService
    {
        Task<ServiceResult<UserInformationModel>> LoginAsync(ClientMode mode, string username, string password);
    }
}
