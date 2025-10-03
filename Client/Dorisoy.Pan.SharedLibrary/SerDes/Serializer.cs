using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dorisoy.Pan.SharedLibrary.SerDes;

public static class Serializer
{
    public static byte[] serialize(object obj)
    {
        return SerializeHelper.Serialize(obj);
    }
}
