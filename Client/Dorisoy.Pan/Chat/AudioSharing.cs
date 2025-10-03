namespace Dorisoy.PanClient.Chat;

/// <summary>
/// 表示音频共享
/// </summary>
public class AudioSharing : DataSharing
{
    private WaveInEvent input;
    private WaveOutEvent output;
    private BufferedWaveProvider bufferStream;

    public AudioSharing(VoiceChatModel model) : base(model)
    {
        LineIndex = 0;

        // 创建一个流来记录我们的演讲 采样频率8000 Hz，采样宽度16位，单通道
        input = new WaveInEvent();
        input.WaveFormat = new WaveFormat(8000, 16, 1);

        // 创建流以收听传入声音
        bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));

        output = new WaveOutEvent();
        output.Init(bufferStream);
    }

    public override void BeginSend()
    {
        base.BeginSend();

        input.DataAvailable += Send;
        input.StartRecording();
    }

    public override void BeginReceive()
    {
        bufferStream.ClearBuffer();
        output.Play();

        base.BeginReceive();
    }

    public override void EndSend()
    {
        base.EndSend();

        input.StopRecording();
        input.DataAvailable -= Send;
    }

    public override void EndReceive()
    {
        base.EndReceive();
        output.Stop();
    }

    protected override void Send(object sender, EventArgs e)
    {
        base.Send(sender, e);
        BdtpClient.Send((e as WaveInEventArgs).Buffer, LineIndex);
    }


    private UdpSession udpSenders;
    protected override void Receive()
    {
        if (udpSenders == null)
            udpSenders = BdtpClient.GetUdpSession(LineIndex);
        

        if (!udpSenders.DisposedValue)
        {
            udpSenders.Received = (client, e) =>
            {
                try
                {
                    // 接收数据包
                    byte[] data = e.ByteBlock.ToArray();
                    bufferStream.AddSamples(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                }

                return Task.CompletedTask;
            };
        }
    }
}
