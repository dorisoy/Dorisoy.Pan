using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.Repository
{

    public class DatabaseSettings
    {
        public string? ConnectionString { get; set; }
    }

    public enum EventType
    {
        Joined,
        Disconnected
    }

    public class ConnectionRoom
    {
        public int ConnectionId { get; set; }
        public string Nickname { get; set; }
        public Guid RoomId { get; set; }
        public string Room { get; set; }
    }

    public class Connections
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

    public class Events
    {
        public int Id { get; set; }
        public Guid RoomId { get; set; }
        public int ConnectionId { get; set; }
        public int Type { get; set; }
        public DateTime Occurred { get; set; }
    }

    public class Rooms
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsPublic { get; set; }
        public DateTime Created { get; set; }
        public Guid CreateBy { get; set; }
        public List<Connections> Users { get; set; }
    }

}
