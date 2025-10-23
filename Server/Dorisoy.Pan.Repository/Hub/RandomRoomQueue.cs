using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dorisoy.Pan.Repository;

public interface IRandomRoomQueue
{
    void Enqueue(string connectionId, string nickname, string? ipAddress);
    UserInQueue? Dequeue();
    void Remove(string connectionId);
}

public class RandomRoomQueue : IRandomRoomQueue
{
    private readonly ConcurrentDictionary<string, UserInQueue> _queue = new();

    public void Enqueue(string connectionId, string nickname, string? ipAddress)
    {
        _queue.TryAdd(connectionId, new UserInQueue(connectionId, nickname, ipAddress));
    }

    public UserInQueue? Dequeue()
    {
        if (_queue.IsEmpty)
        {
            return null;
        }

        _queue.Remove(_queue.First().Key, out UserInQueue? userInQueue);

        return userInQueue;
    }

    public void Remove(string connectionId)
    {
        _queue.TryRemove(connectionId, out UserInQueue? _);
    }
}

public record UserInQueue(string ConnectionId, string Nickname, string? IpAddress);




public static class RoomNameGenerator
{
    private static readonly char[] chars = "abcdefghkmnprstuvwxyz123456789".ToCharArray();
    private const int Length = 7;

    public static string Generate()
    {
        var data = new byte[4 * Length];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }

        var result = new StringBuilder(Length);
        for (int i = 0; i < Length; i++)
        {
            var random = BitConverter.ToUInt32(data, i * 4);
            var index = random % chars.Length;

            result.Append(chars[index]);
        }

        return result.ToString();
    }
}