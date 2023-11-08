using Avalonia;
using Microsoft.AspNetCore.SignalR.Client;

namespace Dorisoy.PanClient.Services;
public interface IWhiteBoardService
{
    HubConnection Connection { get; }
    IObservable<IChangeSet<Rooms, Guid>> Connect();
    Task CreateRoomAsync();
    Task<bool> CreateRoomAsync(Guid uid, string nickname, string room, string password, CancellationToken calToken = default);
    Task<bool> DeleteRoomAsync(string roomid, CancellationToken calToken = default);
    Task DrawLineAsync(List<Point> points);
    Task DrawPointAsync(double x, double y);
    Task<List<Connections>> GetParticipantsAsync(string room, CancellationToken calToken = default);
    Task GetRoomsAsync();
    Task<List<Rooms>> GetRoomsAsync(Guid uid, CancellationToken calToken = default);
    Task JoinRandomRoomAsync();
    Task JoinRoomAsync();
    Task LeaveRoomAsync(Guid uid);
    Task<JoinRoomValidationResult> ValidateJoinRoomAsync(string room, string nickname, CancellationToken calToken = default);
}
