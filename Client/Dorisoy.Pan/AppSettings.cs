using System.Security.Claims;
using Dorisoy.PanClient.Models;
using Path = System.IO.Path;

namespace Dorisoy.PanClient;

/// <summary>
/// 程序配置
/// </summary>
public class AppSettings
{
    /// <summary>
    /// 远程服务器地址
    /// </summary>
    public string ServerIP { get; set; }

    /// <summary>
    /// 远程主机配置
    /// </summary>
    public string HostUrl { get; set; } = "http://{0}:5000/";

    /// <summary>
    /// 远程服务器UDP端口
    /// </summary>
    public int ServerUdpPort { get; set; } = 9933;

    /// <summary>
    /// TCP发送接收端口
    /// </summary>
    public int TCPSendReceiver { get; set; } = 8800;

    /// <summary>
    /// RTMP 服务器地址
    /// </summary>
    public string RSTPServer { get; set; } = "rtmp://{0}/live/livestream";

    /// <summary>
    /// 远程数据库连接字符串
    /// </summary>
    public string DB_Conn { get; set; } = "data source={0};Port=3306;Initial Catalog=vcms;user id=root;password=racing.1";

    /// <summary>
    /// 应用程序标识
    /// </summary>
    public Guid ApplicationId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 日志存储目录
    /// </summary>
    public string LogsFolder { get; set; } = "Logs";

    /// <summary>
    /// 本地数据库目录
    /// </summary>
    public string DbFilename { get; set; } = "data.db";

    /// <summary>
    /// 本地临时存储目录
    /// </summary>
    public string TempFolder { get; set; } = "Temps";

    /// <summary>
    /// 客户端模式
    /// </summary>
    public ClientMode ClientMode { get; set; }

    /// <summary>
    /// 加密密钥
    /// </summary>
    public string EncryptionKey { get; set; } = "sURur^bo+i%8)p9+#C-0T9?WxDfreHwz*?*gR^f(4/'e5M056_L8B3:?%8@PE@6";
    public string UserProfilePath { get; set; } = "Users";

    /// <summary>
    /// 默认摄像头驱动
    /// </summary>
    public CameraDevice CameraDefault { get; set; }


    /// <summary>
    /// 本地用户物理存储目录
    /// </summary>
    public string DocumentPath { get; set; } = "Documents";

    public string ExecutableFileTypes { get; set; } = ".bat,.bin,.cmd,.com,.cpl,.exe,.gadget,.inf1,.ins,.inx,.isu,.job,.jse,.lnk,.msc,.msi,.msp,.action,.apk,.app,.command,.csh,.ipa,.ksh,.mst,.osx,.out,.paf,.pif,.run";

    public string ContentRootPath { get; set; } = "";

    public string GetHost()
    {
        var _url = HostUrl.EndsWith("/") ? (HostUrl + "api") : (HostUrl + "/api");
        return string.Format(_url, ServerIP);
    }

    public string GetHub()
    {
        var _hub = HostUrl.EndsWith("/") ? (HostUrl + "userHub") : (HostUrl + "/userHub");
        return string.Format(_hub, ServerIP);
    }

    public string GetDBConn()
    {
        return string.Format(DB_Conn, ServerIP);
    }

    public string GetRSTPServer()
    {
        return string.Format(RSTPServer, ServerIP);
    }
}


/// <summary>
/// 全局存储设置
/// </summary>
public class Globals
{
    public static ConcurrentDictionary<Guid, SharedLibrary.Data.Models.UserModel> _onlineUsers { get; set; } = new();

    public static Guid ReferenceUserId { get; set; }

    public static CultureInfo CultureInfo { get; set; } = new CultureInfo("zh");

    /// <summary>
    /// Canvas画布最大宽度
    /// </summary>
    public static int CanvasWidth { get; set; } = 1024;
    /// <summary>
    /// Canvas画布最大高度
    /// </summary>
    public static int CanvasHeight { get; set; } = 768;
    /// <summary>
    /// Canvas画布最大宽度
    /// </summary>
    public const int ChatCanvasWidth = 1280;
    /// <summary>
    /// Canvas画布最大高度
    /// </summary>
    public const int ChatCanvasHeight = 720;

    public static bool IsTeam { get; set; }

    /// <summary>
    /// 当前用户
    /// </summary>
    public static UserAuthDto CurrentUser { get; set; } 

    /// <summary>
    /// 当前在线用户数
    /// </summary>
    public static int Onlines { get; set; }
    public static List<Guid> OnlineUsers { get; set; } = new();


    public static string AccessToken { get; set; }
    public static bool IsAuthenticated { get; set; }

    /// <summary>
    /// 身份验证状态用户
    /// </summary>
    public static ClaimsPrincipal AuthenticationStateUser { get; set; }

    /// <summary>
    /// 当前项目
    /// </summary>
    public static PatientModel CurrentPatient { get; set; } = new();

    /// <summary>
    /// 默认摄像头
    /// </summary>
    public static CameraDevice LastSelectCamera { get; set; }

    /// <summary>
    /// 渲染间隔
    /// </summary>
    private static short _renderingIntervalMs = 10;
    public static short RenderingIntervalMs
    {
        get
        {
            return _renderingIntervalMs;
        }
        set
        {
            if (value > 1000)
            {
                _renderingIntervalMs = 1000;
            }
            else if (value < 3)
            {
                _renderingIntervalMs = 3;
            }
            else
            {
                _renderingIntervalMs = value;
            }
        }
    }
}

public static class UserExt
{
    /// <summary>
    /// 检查角色权限声明
    /// </summary>
    /// <param name="user"></param>
    /// <param name="policyName"></param>
    /// <returns></returns>
    public static bool Authorize(this UserAuthDto user, string policyName)
    {
        return user.Claims.Select(s => s.ClaimValue).Contains(policyName);
    }

    public static string GetUserStorageFolder(this User user)
    {
        //创建用户目录
        if (user.Id != Guid.Empty)
        {
            var _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();

            var folder = _settingsProvider.Settings.DocumentPath;
            var documentPath = Path.Combine(folder, user.Id.ToString());

            if (!Directory.Exists($"{documentPath}"))
                Directory.CreateDirectory($"{documentPath}");

            return documentPath;
        }
        else
        {
            return "";
        }
    }
}
