
using Avalonia.Controls;

namespace Dorisoy.PanClient.Chat;


/// <summary>
/// 定义一个BDTP网络服务（双工传输协议）
/// </summary>
public class BdtpClient : IDisposable
{
    /// <summary>
    /// 表示确认的缓冲区大小
    /// </summary>
    public const int BUFFER_SIZE = 1024;
    private static readonly char[] separator = ['|'];

    /// <summary>
    /// 返回指定是否建立连接的值。
    /// </summary>
    public bool Connected
    {
        get
        {
            return RemoteIP != null;
        }
    }

    /// <summary>
    /// 返回用于传输和接收数据的线路数
    /// </summary>
    public int LineCount { get; }

    private IPAddress remoteIP;
    /// <summary>
    /// 返回已连接到的远程IP地址
    /// </summary>
    public virtual IPAddress RemoteIP
    {
        get
        {
            return remoteIP;
        }

        protected set
        {
            remoteIP = value;
            if (value != null)
            {
                tcpListener?.Stop();
                var thread = new Thread(new ThreadStart(WaitReceipt));
                thread.Start();
            }
        }
    }

    /// <summary>
    /// 返回链接的本地IP地址
    /// </summary>
    public IPAddress LocalIP { get; private set; }


    /// <summary>
    /// 定义一个TCP监听器
    /// </summary>
    private TcpListener tcpListener;

    /// <summary>
    /// 定义一个TCP控制器
    /// </summary>
    private System.Net.Sockets.TcpClient tcpController;

    /// <summary>
    /// 直接初始化TcpService，会使用默认的SocketClient。 
    /// 简单的处理逻辑可通过Connecting、Connected、Received等委托直接实现。
    /// </summary>
    private TcpService tcpService;
    /// <summary>
    /// 简单的处理逻辑可通过Connecting、Connected、Received等委托直接实现
    /// </summary>
    private TorchSocket.Sockets.TcpClient tcpClient;

    public TcpService GetTcpService => tcpService;
    public TorchSocket.Sockets.TcpClient GetTcpClient => tcpClient;

    /// <summary>
    /// UDP发送器
    /// </summary>
    private UdpSession[] udpSenders;

    /// <summary>
    /// UDP接收器
    /// </summary>
    private UdpSession[] udpReceivers;

    /// <summary>
    /// TCP端口号
    /// </summary>
    public int TcpPort { get; private set; } = 11100;

    /// <summary>
    /// UDP发送端口号
    /// </summary>
    public int SenderPort { get; private set; } = 11001;

    /// <summary>
    /// UDP接收端口号
    /// </summary>
    public int ReceiverPort { get; private set; } = 11011;


    /// <summary>
    /// 当远程主机接收确认时发生
    /// </summary>
    public event Action<byte[]> ReceiptReceived;


    public event Action<string> ReceiptInfoReceived;


    /// <summary>
    /// 初始化BDTPClient类的新实例，并将其与指定的本地IP地址和指定的接收和发送数据的线路数相关联.
    /// </summary>
    /// <param name="localIP">本地主机IPAddress对象</param>
    /// <param name="lineCount">数线路</param>
    public BdtpClient(IPAddress localIP, int lineCount)
    {
        //最多允许50路线路
        if (lineCount > 50)
            throw new OverflowException("线路太多");

        LineCount = lineCount;
        LocalIP = localIP;

        //初始化TCP客户端
        InitializeClient();
    }

    /// <summary>
    /// 初始化TCP客户端
    /// </summary>
    private void InitializeClient()
    {
        tcpListener = new TcpListener(LocalIP, TcpPort);
        tcpController = new System.Net.Sockets.TcpClient();

        //tcpService
        InitTcpService();
        InitTcpClient();

        udpSenders = new UdpSession[LineCount];
        udpReceivers = new UdpSession[LineCount];

        // 创建通道集合
        for (int i = 0; i < LineCount; i++)
        {
            //作为客户端使用
            udpSenders[i] = new UdpSession();
            udpSenders[i].Setup(new TorchSocketConfig()
                      .SetBindIPHost(new IPHost(LocalIP, SenderPort + i))
                      .UseBroadcast()
                      .SetNoDelay(true)
                      .SetUdpDataHandlingAdapter(() => new UdpPackageAdapter()));
            udpSenders[i].Start();

            //作为服务器使用
            udpReceivers[i] = new UdpSession();
            udpReceivers[i].Setup(new TorchSocketConfig()
                     .SetBindIPHost(new IPHost(LocalIP, ReceiverPort + i))
                     .UseBroadcast()
                     .SetNoDelay(true)
                     .SetUdpDataHandlingAdapter(() => new UdpPackageAdapter()));
            udpReceivers[i].Start();
        }
    }

    /// <summary>
    /// 初始文件传输TCP服务
    /// </summary>
    private void InitTcpService()
    {
        tcpService = new TcpService
        {
            //有客户端正在连接
            Connecting = (client, e) => { return EasyTask.CompletedTask; },
            //有客户端成功连接
            Connected = (client, e) => { return EasyTask.CompletedTask; },
            //有客户端正在断开连接，只有当主动断开时才有效。
            Disconnecting = (client, e) => { return EasyTask.CompletedTask; },
            //有客户端断开连接
            Disconnected = (client, e) => { return EasyTask.CompletedTask; }
        };

        //载入配置
        tcpService.Setup(new TorchSocketConfig()
            .SetListenIPHosts(new IPHost(LocalIP, TcpPort + 1))
            .ConfigureContainer(a =>//容器的配置顺序应该在最前面
            {
                //添加一个控制台日志注入（注意：在maui中控制台日志不可用）
                a.AddConsoleLogger();
            }));

        //启动
        tcpService.Start();
    }

    /// <summary>
    /// 连接文件传输客户端
    /// </summary>
    private void InitTcpClient()
    {
        tcpClient = new TorchSocket.Sockets.TcpClient();
        //即将连接到服务器，此时已经创建socket，但是还未建立tcp
        tcpClient.Connecting = (client, e) => { return EasyTask.CompletedTask; };
        //成功连接到服务器
        tcpClient.Connected = (client, e) => { return EasyTask.CompletedTask; };
        //即将从服务器断开连接。此处仅主动断开才有效。
        tcpClient.Disconnecting = (client, e) => { return EasyTask.CompletedTask; };
        //从服务器断开连接，当连接不成功时不会触发。
        tcpClient.Disconnected = (client, e) => { return EasyTask.CompletedTask; };
        //载入配置
        tcpClient.Setup(new TorchSocketConfig()
            .ConfigureContainer(a =>
            {
                //添加一个日志注入
                a.AddConsoleLogger();
            }));
    }

    /// <summary>
    /// 使用指定的IP地址将客户端连接到远程BDTP主机。
    /// </summary>
    /// <param name="remoteIP">要连接到的主机的IPAddress对象.</param>
    /// <returns>如果连接成功，则为true；否则，false.</returns>
    public virtual bool Connect(IPAddress remoteIP)
    {
        if (Connected)
            return false;

        try
        {
            //建立TCP连接
            tcpController = new System.Net.Sockets.TcpClient();
            tcpController.Connect(remoteIP, TcpPort);

            //连接文件传输客户端
            //InitTcpClient();
            tcpClient?.Connect(new IPHost(remoteIP, TcpPort + 1));
        }
        catch { return false; }

        //发送接收邀请
        SendReceipt(LocalIP.GetAddressBytes());

        RemoteIP = remoteIP;
        return true;
    }

    /// <summary>
    /// 接受挂起的连接请求
    /// </summary>
    /// <returns>如果您成功接受连接，则为true；否则，false.</returns>
    public virtual bool Accept()
    {
        while (Connected)
        {
           Debug.WriteLine("Connected...");
        }

        try
        {
            // 开始侦听TCP端口连接
            tcpListener.Start();
            // 接受挂起的连接请求(阻塞),直到连接
            tcpController = tcpListener.AcceptTcpClient();
        }
        catch
        {
            return false;
        }

        //等待接收,直到数据到来时(或超时)返回
        var buffer = ReceiveReceipt();
        if (buffer == Array.Empty<byte>())
            return false;

        RemoteIP = new IPAddress(buffer);


        return true;
    }

    /// <summary>
    /// 接受呼叫连接
    /// </summary>
    public void AcceptCallConnect()
    {
        //连接到呼叫者，作为呼叫者客户端
        tcpClient?.Connect(new IPHost(RemoteIP, TcpPort + 1));
    }


    /// <summary>
    /// 停止等待传入的连接请求
    /// </summary>
    public virtual void StopAccept()
    {
        tcpListener?.Stop();
    }

    /// <summary>
    /// 通过UDP协议发送数据字节，UDP是通过指定索引的线路连接到的节点.
    /// </summary>
    /// <param name="data">包含要发送的数据的Byte对象数组.</param>
    /// <param name="index">要发送数据的线路索引.</param>
    /// <returns>成功发送的字节数</returns>
    public virtual int Send(byte[] data, int index)
    {
        if (!Connected)
            return 0;

        var remoteEP = new IPEndPoint(RemoteIP, ReceiverPort + index);

        if (udpSenders[index] != null && !udpSenders[index].DisposedValue)
            udpSenders[index].Send(remoteEP, data);

        //{192.168.0.2:11011}
        //System.Diagnostics.Debug.WriteLine($" Send  {index} to --> {RemoteIP.ToString()}:{UDPReceiverPort + index} {data.Length}");

        return data.Length;
    }


    /// <summary>
    /// 获取指定索引的udpSession
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public UdpSession GetUdpSession(int index)
    {
        return udpReceivers[index];
    }

    /// <summary>
    /// 向连接到的主机发送TCP确认.
    /// </summary>
    /// <param name="data">包含要发送的数据的Byte对象数组.</param>
    /// <returns>如果成功发送数据，则为真；否则，false.</returns>
    public virtual bool SendReceipt(byte[] data)
    {
        if (data.Length > BUFFER_SIZE)
            return false;

        try
        {
            var stream = tcpController.GetStream();
            stream.Write(data, 0, data.Length);
        }
        catch { return false; }

        return true;
    }

    /// <summary>
    /// 接受接收数据(直等到数据到来时(或超时)才会返回)
    /// </summary>
    /// <returns></returns>
    private byte[] ReceiveReceipt()
    {
        var stream = tcpController.GetStream();
        var buffer = new byte[BUFFER_SIZE];
        int count;

        try
        {
            //NetworkStream.read 方法是阻塞的,它会一直等到数据到来时(或超时)才会返回
            //在调用ServerSocket.accept()方法时,也会一直阻塞到有客户端连接才会返回
            count = stream.Read(buffer, 0, BUFFER_SIZE);
        }
        catch
        {
            return Array.Empty<byte>();
        }

        var result = new byte[count];
        Array.Copy(buffer, result, count);

        return result;
    }


    /// <summary>
    /// 等待接收
    /// </summary>
    private void WaitReceipt()
    {
        do
        {
            //等待接收,直到数据到来时(或超时)返回
            var buffer = ReceiveReceipt();
            ReceiptReceived(buffer);
        }
        while (Connected);
    }

    /// <summary>
    /// 关闭与当前远程主机的连接并允许重新连接.
    /// </summary>
    public virtual void Disconnect()
    {
        try
        {
            if (!Connected)
                return;

            RemoteIP = null;

            tcpListener?.Stop();
            tcpController?.Close();
            
            for (var i = 0; i < LineCount; i++)
            {
                if (udpReceivers[i] != null)
                {
                    if (!udpReceivers[i].DisposedValue)
                    {
                        //udpReceivers[i].Stop();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// 释放BDTPClient使用的所有托管和非托管资源.
    /// </summary>
    public void Dispose()
    {
        try
        {
            RemoteIP = null;

            tcpController?.SafeDispose();
            tcpService?.Stop();
            tcpService?.SafeDispose();

            for (var i = 0; i < LineCount; i++)
            {
                udpReceivers[i].SafeDispose();
                udpSenders[i].SafeDispose();
            }

            tcpListener?.Stop();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
