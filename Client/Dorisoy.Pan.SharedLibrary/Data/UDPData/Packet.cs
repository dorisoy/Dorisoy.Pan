using MessagePack;

namespace Dorisoy.Pan.SharedLibrary.Data;

/// <summary>
/// Packet类表示一个单独的UDP数据包
/// </summary>
[MessagePackObject]
public class Packet
{
   [SerializationConstructor]
    public Packet(string userId)
    {
        UserId = userId;
    }

    [Key(0)]
    public string UserId { get; }

    [Key(1)]
    public byte[] Data { get; set; }
}
