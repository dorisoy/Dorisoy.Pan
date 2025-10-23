
namespace Dorisoy.Pan.Services;

public interface IWhiteBoardService
{
    Task<bool> CreateRoomAsync(Guid uid, string nickname, string room, string password, CancellationToken calToken = default);
    Task<bool> DeleteRoomAsync(string roomid, CancellationToken calToken = default);
    Task<List<Connections>> GetParticipantsAsync(string room, CancellationToken calToken = default);
    Task<List<Rooms>> GetRoomsAsync(Guid uid, CancellationToken calToken = default);
    Task<JoinRoomValidationResult> ValidateJoinRoomAsync(string room, string nickname, CancellationToken calToken = default);
}