using System.Net.NetworkInformation;
using Path = System.IO.Path;
namespace Dorisoy.PanClient.Utils;

public class Utilities
{
    public static void SetWindowBounds(
        double pixelDensity,
        double containerW,
        double containerH,
        double originalW,
        double originalH,
        out double width,
        out double height,
        bool addIncrement = true)
    {
        var incrementX = 0.0;
        var incrementY = 0.0;

        // Trick of the pig I
        if (pixelDensity > 1.0)
        {
            pixelDensity = pixelDensity - 1.25 + 1.0;
        }

        if (addIncrement)
        {
            // Trick of the pig II
            incrementX = originalW * pixelDensity * 0.1;
            incrementY = originalH * pixelDensity * 0.1;

            if (originalW + incrementX > containerW)
            {
                incrementX = containerW - originalW - 100;
            }
            if (originalH + incrementY > containerH)
            {
                incrementY = containerH - originalH - 100;
            }
        }

        width = (originalW + incrementX) / pixelDensity;
        height = (originalH + incrementY) / pixelDensity;
    }


    public static string ApplicationFolder()
    {
        Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        string? assemblyPath = Path.GetDirectoryName(assembly.Location);
        return assemblyPath;
    }

    public static void OpenUri(string uri) => _ = Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });


    public static bool IsWin64()
    {
        //if (Environment.Is64BitOperatingSystem)
        if (IntPtr.Size == 4)
        {
            return false;
        }
        return true;
    }

    public static Hardware GetSystemInfo()
    {
        // 获取CPU使用率
        var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        var cpuUsage = (int)cpuCounter.NextValue();
        // 获取内存使用率
        var memCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        var memUsage = (int)memCounter.NextValue();
        return new Hardware(cpuUsage, memUsage);
    }

    public static async Task<double> GetCpuUsageForProcess()
    {
        var startTime = DateTime.UtcNow;
        var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        await Task.Delay(500);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;

        var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

        return cpuUsageTotal * 100;
    }

    /// <summary>
    /// 获取本地IP地址
    /// </summary>
    /// <returns></returns>
    public static string GetLocalIP()
    {
        //Grabs local ip of clients server
        IPHostEntry host;
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1";
    }

    public record Hardware(int CpuUsage, int MemUsage);
}

public class InternetCheck
{
    public static bool PingIpOrDomainName(string ip)
    {
        try
        {
            Ping objPingSender = new Ping();
            PingOptions objPinOptions = new PingOptions();
            objPinOptions.DontFragment = true;
            string data = "";
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            int intTimeout = 120;
            PingReply objPinReply = objPingSender.Send(ip, intTimeout, buffer, objPinOptions);
            string strInfo = objPinReply.Status.ToString();
            if (strInfo == "Success")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }


    [DllImport("wininet.dll")]
    private extern static bool InternetGetConnectedState(ref int Description, int ReservedValue);

    /// <summary>
    /// 用于检查网络是否可以连接互联网,true表示连接成功,false表示连接失败
    /// </summary>
    /// <returns></returns>
    public static bool IsConnectInternet()
    {
        int Description = 0;
        return InternetGetConnectedState(ref Description, 0);
    }

    /// <summary>
    /// 判断本地的连接状态
    /// </summary>
    private static bool IsConnectedInternet()
    {
        int dwFlag = new int();
        if (!InternetGetConnectedState(ref dwFlag, 0))
        {
            Console.WriteLine("当前没有联网，请您先联网后再进行操作！");
            if ((dwFlag & 0x14) == 0)
                return false;
            System.Diagnostics.Debug.WriteLine("本地系统处于脱机模式。");
            return false;
        }
        else
        {
            if ((dwFlag & 0x01) != 0)
            {
                Console.WriteLine("调制解调器上网。");
                return true;
            }
            else if ((dwFlag & 0x02) != 0)
            {
                Console.WriteLine("网卡上网。");
                return true;
            }
            else if ((dwFlag & 0x04) != 0)
            {
                Console.WriteLine("代理服务器上网。");
                return true;
            }
            else if ((dwFlag & 0x40) != 0)
            {
                Console.WriteLine("虽然可以联网，但可能链接也可能不连接。");
                return true;
            }
        }

        return false;
    }
}
