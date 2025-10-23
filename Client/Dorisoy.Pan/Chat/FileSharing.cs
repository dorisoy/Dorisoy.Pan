using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Chat;

/// <summary>
/// 用户表示文件传输共享
/// </summary>
public class FileSharing : DataSharing
{
    //保存路径
    private string saveFilePath;
    //包的大小限制  
    private const int chunkSize = 65000;
    private static readonly char[] separator = ['|'];
    public FileSharing(VoiceChatModel model) : base(model)
    {
        LineIndex = 2;
    }

    private bool receiveing;
    /// <summary>
    /// 接收文件
    /// </summary>
    protected override void Receive()
    {
        var tcpService = BdtpClient.GetTcpService;
        if (tcpService != null && !receiveing)
        {
            saveFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\FileSharing\{BdtpClient.RemoteIP}\{DateTime.Now:yyyyMMdd}";
        }
    }


    private TorchSocket.Sockets.TcpClient _tcpClient;

    /// <summary>
    /// 用于等待服务器确认
    /// </summary>
    private ManualResetEventSlim serverReadyEvent = new ManualResetEventSlim(false);


}
