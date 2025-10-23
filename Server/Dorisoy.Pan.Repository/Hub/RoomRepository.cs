using Dapper;
using Microsoft.Extensions.Options;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository;

public interface IRoomRepository
{
    Task<bool> DeleteRoomAsync(Guid roomid);
    Task<bool> RoomExistsAsync(string room);
    Task CreateRoomAsync(Guid uid, string room, bool isPublic, string password);
    Task<List<Connections>> GetActiveParticipantsInRoomAsync(string room);
    Task CreateRoomAsync(Guid uid, string room, bool isPublic, string nickname, string password, string signalRConnectionId, string? ipAddress);
    Task JoinRoomAsync(Guid uid, string room, string nickname, string signalRConnectionId, string? ipAddress);
    Task<ConnectionRoom?> DisconnectAsync(string signalRConnectionId);
    Task DisconnectAllAsync();
    Task<IList<Rooms>> GetRooms(Guid createBy);
    Task<List<Connections>> GetRoomUsersAsync(Guid roomid);
}


public class RoomRepository : IRoomRepository
{
    private readonly string? _connectionString;

    public RoomRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        _connectionString = databaseSettings.Value.ConnectionString;
    }

    public async Task<bool> RoomExistsAsync(string room)
    {
        using IDbConnection conn = OpenConnection();
        return await conn.ExecuteScalarAsync<bool>(@"SELECT COUNT(*) FROM rooms WHERE name = @room", new { room });
    }

    public async Task<bool> DeleteRoomAsync(Guid roomid)
    {
        try
        {
            using IDbConnection conn = OpenConnection();
            using (var transaction = conn.BeginTransaction())
            {
                var ret = await conn.ExecuteScalarAsync<bool>(@"DELETE FROM `vcms`.`rooms` WHERE  Id = @roomid", new { roomid }, transaction: transaction);
                var ret2 = await conn.ExecuteScalarAsync<bool>(@"DELETE FROM `vcms`.`connections` WHERE  RoomId = @roomid", new { roomid }, transaction: transaction);
                transaction.Commit();
                return ret;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    /// <summary>
    /// 获取参与人
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public async Task<List<Connections>> GetActiveParticipantsInRoomAsync(string room)
    {
        using IDbConnection conn = OpenConnection();
        return (await conn.QueryAsync<Connections>(@"SELECT c.*
            FROM rooms AS r
            INNER JOIN connections AS c ON r.id = c.roomid AND c.isconnected
            WHERE r.name = @room", new { room })).ToList();
    }

    public async Task CreateRoomAsync(Guid uid, string room, bool isPublic, string nickname, string password, string signalRConnectionId, string? ipAddress)
    {
        try
        {
            var now = DateTime.Now;

            using IDbConnection conn = OpenConnection();
            using var transaction = conn.BeginTransaction();

            var rid = Guid.NewGuid();

            var roomId = await conn.ExecuteScalarAsync<int>("INSERT INTO rooms (name, ispublic, created,CreateBy) VALUES (@id,@name, @isPublic, @created, @createBy,@password) ;SELECT LAST_INSERT_ID();",
                new { id = rid, name = room, isPublic, created = now, createBy = uid, password = password }, transaction);

            var connectionId = await conn.ExecuteScalarAsync<int>(@"INSERT INTO connections (roomid, signalrconnectionid, ipaddress, user, isconnected, created, modified)
            VALUES (@roomId, @signalRConnectionId, @ipAddress, @nickname, TRUE, @created, @modified) ;SELECT LAST_INSERT_ID();",
                new { roomId, signalRConnectionId, ipAddress, nickname, created = now, modified = now }, transaction);

            await conn.ExecuteAsync("INSERT INTO events (roomid, connectionid, type, occurred) VALUES (@roomId, @connectionId, @type, @occurred)",
                new { roomId, connectionId, type = EventType.Joined, occurred = now }, transaction);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.Message);
        }
    }


    public async Task CreateRoomAsync(Guid uid, string room, bool isPublic, string password)
    {
        try
        {
            var now = DateTime.Now;

            using IDbConnection conn = OpenConnection();
            using var transaction = conn.BeginTransaction();
            var rid = Guid.NewGuid();

            var roomId = await conn.ExecuteScalarAsync<string>("INSERT INTO rooms (id,name, ispublic, created,CreateBy,password) VALUES (@id,@name, @isPublic, @created, @createBy,@password) ;SELECT LAST_INSERT_ID();",
                new { id = rid, name = room, isPublic, created = now, createBy = uid, password = password }, transaction);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.Message);
        }
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="room"></param>
    /// <param name="nickname"></param>
    /// <param name="signalRConnectionId"></param>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public async Task JoinRoomAsync(Guid uid, string room, string nickname, string signalRConnectionId, string? ipAddress)
    {
        try
        {
            var now = DateTime.Now;
            using IDbConnection conn = OpenConnection();
            using (var transaction = conn.BeginTransaction())
            {
                var roomId = await conn.QueryFirstAsync<Guid>("SELECT id FROM rooms WHERE name = @name", new { name = room }, transaction: transaction);
                var connectionId = await conn.QueryFirstOrDefaultAsync<int?>(@"SELECT id FROM connections WHERE signalrconnectionid = @signalRConnectionId", new { signalRConnectionId }, transaction: transaction);
                if (connectionId.HasValue)
                {
                    await conn.ExecuteScalarAsync<int>(@"UPDATE connections SET isconnected = TRUE, modified = @modified WHERE id = @connectionId", new { connectionId, modified = now }, transaction);
                }
                else
                {
                    connectionId = await conn.ExecuteScalarAsync<int>(@"INSERT INTO connections (roomid, signalrconnectionid, ipaddress, user, isconnected, created, modified,UserId)
                VALUES (@roomId, @signalRConnectionId, @ipAddress, @nickname, TRUE, @created, @modified,@userId) ;SELECT LAST_INSERT_ID();",
                        new
                        {
                            roomId,
                            signalRConnectionId,
                            ipAddress,
                            nickname,
                            created = now,
                            modified = now,
                            userId = uid
                        }, transaction);
                }

                await conn.ExecuteAsync("INSERT INTO events (roomid, connectionid, type, occurred) VALUES (@roomId, @connectionId, @type, @occurred)",
                    new { roomId, connectionId, type = EventType.Joined, occurred = now }, transaction);

                transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.Message);
        }
    }


    /// <summary>
    /// 获取房间
    /// </summary>
    /// <param name="createBy"></param>
    /// <returns></returns>
    public async Task<IList<Rooms>> GetRooms(Guid createBy)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@createBy", createBy, DbType.Guid);
            using IDbConnection conn = OpenConnection();
            //var rooms = await conn.QueryAsync<Rooms>(@"SELECT * from rooms where CreateBy = @createBy", parameters);
            var rooms = await conn.QueryAsync<Rooms>(@"SELECT * from rooms", parameters);
            if (rooms != null && rooms.Any())
            {
                foreach (var r in rooms)
                {
                    r.Users = await GetRoomUsersAsync(r.Id);
                }
                return rooms.ToList();
            }
            return new List<Rooms>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.Message);
            return new List<Rooms>();
        }
    }

    public async Task<List<Connections>> GetRoomUsersAsync(Guid roomid)
    {
        using IDbConnection conn = OpenConnection();
        return (await conn.QueryAsync<Connections>(@"SELECT c.*
            FROM rooms AS r
            INNER JOIN connections AS c ON r.id = c.roomid AND c.isconnected
            WHERE r.id = @roomid", new { roomid })).ToList();
    }

    public async Task<ConnectionRoom?> DisconnectAsync(string signalRConnectionId)
    {
        try
        {
            var now = DateTime.UtcNow;

            using IDbConnection conn = OpenConnection();
            using var transaction = conn.BeginTransaction();

            //await conn.ExecuteAsync(@"SET CONSTRAINTS ""FK_events_rooms_room_id"", ""FK_events_connections_connection_id"" DEFERRED", null, transaction);

            var connectionRoom = await conn.QueryFirstOrDefaultAsync<ConnectionRoom>(@"SELECT c.id AS ConnectionId, c.user  AS Nickname, r.id AS RoomId, r.name AS Room
            FROM connections AS c
            INNER JOIN rooms AS r ON c.roomid = r.id
            WHERE signalrconnectionid = @signalRConnectionId", new { signalRConnectionId }, transaction: transaction);

            if (connectionRoom != null)
            {
                await conn.ExecuteScalarAsync<int>(@"UPDATE connections SET isconnected = FALSE, modified = @modified WHERE id = @connectionId", new { connectionId = connectionRoom.ConnectionId, modified = now }, transaction);

                await conn.ExecuteAsync("INSERT INTO events (roomid, connectionid, type, occurred) VALUES (@roomId, @connectionId, @type, @occurred)",
                    new { roomId = connectionRoom.RoomId, connectionId = connectionRoom.ConnectionId, type = EventType.Disconnected, occurred = now }, transaction);
            }

            transaction.Commit();

            return connectionRoom;


        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.Message);
            return null;
        }
    }

    public async Task DisconnectAllAsync()
    {
        try
        {
            using IDbConnection conn = OpenConnection();
            var ret = await conn.ExecuteScalarAsync<int>(@"UPDATE connections SET isconnected = FALSE, modified = @modified", new { modified = DateTime.UtcNow });
            if (ret > 0)
            {

            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.Message);
        }
    }

    private IDbConnection OpenConnection()
    {
        var conn = new MySqlConnection(_connectionString);
        conn.Open();
        return conn;
    }
}
