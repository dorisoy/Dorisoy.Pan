using ProtoBuf;
using Protobuff;

namespace Dorisoy.PanClient.Models;

/// <summary>
/// 表示Chat序列化数据模型
/// </summary>
[ProtoContract]
class ChatSerializationData : IProtoMessage
{
    [ProtoContract]
    public enum MsgType
    {
        /// <summary>
        /// 本地
        /// </summary>
        Local,
        /// <summary>
        /// 远程
        /// </summary>
        Remote,
        /// <summary>
        /// 系统信息
        /// </summary>
        Info
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    [ProtoMember(1)]
    public MsgType MessageType { get; set; }

    /// <summary>
    /// 发送者
    /// </summary>
    [ProtoMember(2)]
    public string Sender { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    [ProtoMember(3)]
    public string Message { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    [ProtoMember(4)]
    public DateTime TimeStamp { get; set; }
}


/// <summary>
/// 表示一个Chat 序列化器
/// </summary>
internal class ChatSerializer
{
    /// <summary>
    /// 并发程序序列化器
    /// </summary>
    ConcurrentProtoSerialiser serializer = new ConcurrentProtoSerialiser();

    /// <summary>
    /// 最后偏移量
    /// </summary>
    private int lastOffset = 0;

    /// <summary>
    /// 是否全部载入消息
    /// </summary>
    private bool allMessagesLoaded;

    /// <summary>
    /// 路径索引
    /// </summary>
    public string PathIndex { get; private set; }

    /// <summary>
    /// 路径数据
    /// </summary>
    public string PathData { get; private set; }

    /// <summary>
    /// 数据量锁
    /// </summary>
    private readonly object StreamLocker = new object();

    /// <summary>
    /// 存储数据路径
    /// </summary>
    /// <param name="path"></param>
    public ChatSerializer(string path)
    {
        //文件名称
        PathData = path + @"\data.bin";
        new FileStream(PathData, FileMode.OpenOrCreate).Dispose();
    }

    /// <summary>
    /// 序列化远程输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    /// <param name="timestamp"></param>
    public void SerializeRemoteEntry(string sender, string message, DateTime timestamp)
    {
        //表示Chat序列化数据模型
        var data = new ChatSerializationData
        {
            MessageType = ChatSerializationData.MsgType.Remote,
            Sender = sender,
            Message = message,
            TimeStamp = timestamp
        };

        //序列到流
        SerializeIntoStream(data);
    }

    /// <summary>
    /// 序列化本地输入
    /// </summary>
    /// <param name="message"></param>
    /// <param name="timestamp"></param>
    /// <param name="sender"></param>
    public void SerializeLocalEntry(string message, DateTime timestamp, string sender)
    {
        var data = new ChatSerializationData
        {
            MessageType = ChatSerializationData.MsgType.Local,
            Sender = sender,
            Message = message,
            TimeStamp = timestamp
        };
        //序列到流
        SerializeIntoStream(data);
    }

    /// <summary>
    /// 序列化信息输入
    /// </summary>
    /// <param name="message"></param>
    /// <param name="timestamp"></param>
    public void SerializeInfoEntry(string message, DateTime timestamp)
    {
        var data = new ChatSerializationData
        {
            MessageType = ChatSerializationData.MsgType.Info,
            Message = message,
            TimeStamp = timestamp
        };
        //序列到流
        SerializeIntoStream(data);
    }

    /// <summary>
    /// 序列到流
    /// </summary>
    /// <param name="data"></param>
    private void SerializeIntoStream(ChatSerializationData data)
    {
        lock (StreamLocker)
        {
            using (var streamData = new FileStream(PathData, FileMode.Append))
            {
                // prefix + postfix 以防结尾损坏
                var bytes = serializer.Serialize(data);
                //var bytes2 = SerializeConvert.BinarySerialize(data);
                int lenght = bytes.Length;
                var msgByteLength = BitConverter.GetBytes(lenght);

                //将一个字节块写入文件流
                //数组中从零开始将字节复制到的字节偏移量
                //要写入的最大字节数
                streamData.Write(msgByteLength, 0, 4);
                streamData.Write(bytes, 0, bytes.Length);
                streamData.Write(msgByteLength, 0, 4);

                streamData.Flush();
                lastOffset += bytes.Length + 8;
            };
        }
    }

    /// <summary>
    /// 读取到结尾
    /// </summary>
    /// <param name="maxAmount"></param>
    /// <param name="messages"></param>
    /// <returns></returns>
    public bool LoadFromEnd(int maxAmount, out List<ChatSerializationData> messages)
    {
        lock (StreamLocker)
        {
            messages = null;
            if (allMessagesLoaded)
            {
                return false;
            }

           //打开文件流
            using var streamData = new FileStream(PathData, FileMode.Open);
            // seek start + 4,                      -> get len
            // seek start + len + 4,                -> get msg
            // seek start + len + 12,               -> get len2
            // seek start + len + len2 + 12,        ...
            // seek start + len + len2 + 20
            // seek start + len + len2 + len3 + 20
            bool retval = false;
            try
            {
                int offset = lastOffset;
                byte[] suffix = new byte[4];
                messages = new List<ChatSerializationData>();
                int numMessages = 0;
                while (numMessages < maxAmount && (streamData.Length >= offset + 8))
                {
                    var pos = streamData.Seek(-(offset + 4), SeekOrigin.End);
                    streamData.Read(suffix, 0, 4);
                    var lenght = BitConverter.ToInt32(suffix, 0);

                    streamData.Seek(-(lenght + offset + 4), SeekOrigin.End);
                    byte[] message = new byte[lenght];
                    streamData.Read(message, 0, message.Length);

                    var msg = serializer.Deserialize<ChatSerializationData>(message, 0, message.Length);
                    messages.Add(msg);

                    numMessages++;
                    offset += lenght + 8;
                    lastOffset = offset;
                    retval = true;
                }

                if ((streamData.Length < offset + 8))
                    allMessagesLoaded = true;

                return retval;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 清除历史消息
    /// </summary>
    public void ClearAllHistory()
    {
        lock (StreamLocker)
        {
            allMessagesLoaded = true;
            File.WriteAllText(PathData, string.Empty);
        }
    }
}
