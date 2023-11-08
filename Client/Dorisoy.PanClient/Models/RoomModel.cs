namespace Dorisoy.PanClient.Models;

public readonly record struct JoinRoomValidationResult(bool RoomExists = true, bool RoomIsFull = false, bool NicknameIsTaken = false);

public class Connections : ReactiveObject
{
    public int Id { get; set; }
    [Reactive] public Guid RoomId { get; set; }
    [Reactive] public string SignalrConnectionId { get; set; }
    [Reactive] public string IPAddress { get; set; }
    [Reactive] public string User { get; set; }
    public Guid UserId { get; set; }
    [Reactive] public bool IsConnected { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}

public class Events : ReactiveObject
{
    public int Id { get; set; }
    [Reactive] public Guid RoomId { get; set; }
    [Reactive] public int ConnectionId { get; set; }
    [Reactive] public int Type { get; set; }
    public DateTime Occurred { get; set; }
}

public class Rooms : ReactiveObject
{
    public Guid Id { get; set; }
    public string Password { get; set; }
    [Reactive] public string Name { get; set; }
    [Reactive] public bool IsPublic { get; set; }
    public DateTime Created { get; set; }
    public Guid CreateBy { get; set; }
    public List<Connections> Users { get; set; }
    public int UserCount
    {
        get { return Users?.Count ?? 0; }
    }
}


public class RoomsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public bool IsPublic { get; set; }
    public DateTime Created { get; set; }
    public Guid CreateBy { get; set; }
    public List<Connections> Users { get; set; }
}


public class ConnectionsDto
{
    public int Id { get; set; }
    public Guid RoomId { get; set; }
    public string SignalrConnectionId { get; set; }
    public string IPAddress { get; set; }
    public string User { get; set; }
    public Guid UserId { get; set; }
    public bool IsConnected { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}
