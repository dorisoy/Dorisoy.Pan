using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data.Entities;

[Table("Connections")]
public class Connections
{
    [Key]
    public int Id { get; set; }
    public Guid RoomId { get; set; }
    public string SignalrConnectionId { get; set; }
    public string IPAddress { get; set; }
    public string User { get; set; }
    public bool IsConnected { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}

[Table("Events")]
//[PrimaryKey("Id")]
public class Events
{
    [Key]
    public int Id { get; set; }
    public Guid RoomId { get; set; }
    public int ConnectionId { get; set; }
    public int Type { get; set; }
    public DateTime Occurred { get; set; }
}

[Table("Rooms")]
//[PrimaryKey("Id")]
public class Rooms
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public bool IsPublic { get; set; }
    public DateTime Created { get; set; }
    public Guid CreateBy { get; set; }
}
