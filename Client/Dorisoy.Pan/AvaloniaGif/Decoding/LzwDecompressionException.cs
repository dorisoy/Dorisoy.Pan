using System.Runtime.Serialization;

namespace AvaloniaGif.Decoding;

[Serializable]
public class LzwDecompressionException : Exception
{
    public LzwDecompressionException()
    {
    }

    public LzwDecompressionException(string message) : base(message)
    {
    }

    public LzwDecompressionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected LzwDecompressionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
