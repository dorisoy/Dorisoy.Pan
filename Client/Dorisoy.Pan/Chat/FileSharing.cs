using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Chat;

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
            FileStream fs = null;
            long fileSize = 0;
            long receivedSize = 0;
            var fileName = "";

            tcpService.Received = (client, e) =>
            {
                receiveing = true;
                try
                {
                    //从客户端收到信息,注意：数据长度是byteBlock.Len
                    var message = Encoding.UTF8.GetString(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
                    if (message.StartsWith("FILESHARING"))
                    {
                        // 发送确认消息给客户端
                        client.Send(Encoding.UTF8.GetBytes("OK"));
                    }
                    else if (message.StartsWith("CONFIRMFILE"))
                    {
                        var files = message.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        if (files.Length > 1)
                        {
                            fileName = files[1];
                            fileSize = long.Parse(files[2]);

                            // 创建文件
                            string filePath = System.IO.Path.Combine(saveFilePath, fileName);

                            if (!Directory.Exists(saveFilePath))
                                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

                            fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                        }
                        // 发送文件信息接收确认回应
                        client.Send(Encoding.UTF8.GetBytes("READY_FOR_FILE"));
                    }
                    else
                    {
                        // 处理文件数据
                        if (fs != null)
                        {
                            fs.Write(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
                            receivedSize += e.ByteBlock.Len;

                            // 文件接收完毕
                            if (receivedSize >= fileSize)
                            {
                                fs.Close();
                                Console.WriteLine($"文件接收完成:{fileName}");

                                // 重置状态
                                fileName = null;
                                fileSize = 0;
                                receivedSize = 0;
                                fs = null;
                            }
                        }
                    }
                }
                catch (Exception ex) { }

                return EasyTask.CompletedTask;
            };
        }
    }


    private TorchSocket.Sockets.TcpClient _tcpClient;

    /// <summary>
    /// 用于等待服务器确认
    /// </summary>
    private ManualResetEventSlim serverReadyEvent = new ManualResetEventSlim(false);

    /// <summary>
    /// 发送文件
    /// </summary>
    /// <param name="main"></param>
    /// <param name="uploads"></param>
    /// <param name="progress"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task StartSend(MainViewViewModel main, List<UploadFile> uploads, IProgress<double> progress, CancellationToken token)
    {
        //自动创建文件夹
        saveFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\FileSharing\{BdtpClient.RemoteIP}\{DateTime.Now:yyyyMMdd}";

        if (_tcpClient == null)
            _tcpClient = BdtpClient.GetTcpClient;

        if (_tcpClient != null)
        {
            _tcpClient.Received = (c, e) =>
            {
                _tcpClient = c;

                string response = Encoding.UTF8.GetString(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
                if (response == "OK")
                {
                    serverReadyEvent.Set();
                }
                else if (response == "READY_FOR_FILE")
                {
                    serverReadyEvent.Set();
                }
                return EasyTask.CompletedTask;
            };
        }

        var task = Task.Run(() =>
        {
            var findex = 0;
            var pending = uploads.Count;
            foreach (var fp in uploads)
            {
                if (!token.IsCancellationRequested)
                {
                    findex++;
                    try
                    {
                        var fileInfo = new FileInfo(fp.Path);
                        long totalBytes = fileInfo.Length;

                        // 首先发送“CONFIRM”消息
                        _tcpClient.Send(Encoding.UTF8.GetBytes("FILESHARING"));

                        // 重设事件等待服务器响应文件信息接收
                        serverReadyEvent.Reset();

                        // 发送文件名和大小
                        byte[] fileInfoBytes = Encoding.UTF8.GetBytes($"CONFIRMFILE|{fileInfo.Name}|{totalBytes}");
                        _tcpClient.Send(fileInfoBytes);

                        // 重设事件等待服务器响应文件信息接收
                        serverReadyEvent.Reset();

                        // 等待5秒钟,等待服务器确认，超时则取消 
                        if (!serverReadyEvent.Wait(5000))
                        {
                            return;
                        }

                        // 分块发送文件
                        using var fs = new FileStream(fp.Path, FileMode.Open, FileAccess.Read);
                        int bytesRead;
                        long totalReadBytes = 0;
                        // 以4KB为单位分块
                        byte[] buffer = new byte[chunkSize];
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalReadBytes += bytesRead;

                            //报告进度
                            double percentage = (double)totalReadBytes / totalBytes * 100;
                            progress.Report(percentage);

                            //发送数据
                            _tcpClient.Send(buffer, 0, bytesRead);
                        }

                        //写入本地消息
                        var savefp = System.IO.Path.Combine(saveFilePath, fileInfo.Name);
                        main.WriteMessage("http://" + savefp + "");
                    }
                    catch (Exception) { }
                }
            }
        }, token);
        return task;
    }
}
