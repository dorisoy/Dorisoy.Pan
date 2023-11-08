using Refit;
using Dorisoy.PanClient.Commands;

namespace Dorisoy.PanClient.Services;

[Headers("Authorization: Bearer")]
public interface IUserApi
{
    /// <summary>
    /// 获取在线用户
    /// </summary>
    /// <param name="calToken"></param>
    /// <returns></returns>
    [Get("/dashboard/getOnlineUsers")]
    Task<List<UserDto>> GetOnlineUsers(CancellationToken calToken = default);

    /// <summary>
    /// 获取全部用户
    /// </summary>
    /// <param name="calToken"></param>
    /// <returns></returns>
    [Get("/user/GetAllUsers")]
    Task<List<UserDto>> GetAllUsers(CancellationToken calToken = default);

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="addUserCommand"></param>
    /// <param name="calToken"></param>
    /// <returns></returns>
    [Post("/user")]
    Task<UserDto> AddUser(AddUserCommand addUserCommand, CancellationToken calToken = default);

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateUserCommand"></param>
    /// <param name="calToken"></param>
    /// <returns></returns>
    [Put("/user/{id}")]
    Task<UserDto> UpdateUser(Guid id, UpdateUserCommand updateUserCommand, CancellationToken calToken = default);
}
