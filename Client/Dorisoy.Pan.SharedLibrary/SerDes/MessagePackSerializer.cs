using System;

namespace Dorisoy.Pan.SharedLibrary.SerDes;

public class MessagePackSerializer
{
    public static byte[] Serialize<T>(T obj)
    {
        try
        {
            var bytes = MessagePack.MessagePackSerializer.Serialize<T>(obj);
            return bytes;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static T Deserialize<T>(byte[] data)
    {
        try
        {
            var obj = MessagePack.MessagePackSerializer.Deserialize<T>(data);
            return obj;
        }
        catch (Exception ex)
        {
            return default;
        }
    }

}
