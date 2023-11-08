using Refit;

namespace Dorisoy.PanClient.Services;

public interface IRoomApi
{
    [Get("/rooms/alls/{uid}")]
    Task<IList<RoomsDto>> GetOnlineUsers(Guid uid, CancellationToken calToken = default);

    [Get("/rooms/delete/{roomid}")]
    Task<bool> DeleteRoom(string roomid, CancellationToken calToken = default);

    [Post("/rooms/createroom/{uid}/{nickname}/{room}/{password}")]
    Task<bool> CreateRoom(Guid uid, string nickname, string room, string password, CancellationToken calToken = default);

    [Get("/rooms/{room}/validate-join/{nickname}")]
    Task<JoinRoomValidationResult> ValidateJoin(string room, string nickname, CancellationToken calToken = default);

    [Get("/rooms/{room}/participants")]
    Task<List<ConnectionsDto>> GetParticipantsInRoom(string room, CancellationToken calToken = default);
}
