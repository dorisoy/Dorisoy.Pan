using System.Collections.Concurrent;
using System.Globalization;
using System.Security.Claims;
using Dorisoy.PanClient.Common;

namespace Dorisoy.PanClient;


/// <summary>
/// 程序配置
/// </summary>
public class AppSettings
{
    //192.168.0.100
    public const string ServerIP = "192.168.0.100";

    /// <summary>
    /// 远程主机配置
    /// </summary>
    public string HostUrl { get; set; } = $"http://{ServerIP}:5000/";

    /// <summary>
    /// 远程服务器地址
    /// </summary>
    public string ServerHost { get; set; } = $"{ServerIP}";

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
    public string RSTPServer { get; set; } = $"rtmp://{ServerIP}/live/livestream";

    /// <summary>
    /// 远程数据库连接字符串
    /// </summary>
    public string DB_Conn { get; set; } = $"data source={ServerIP};Port=3306;Initial Catalog=vcms;user id=root;password=123";

    /// <summary>
    /// 应用程序标识
    /// </summary>
    public Guid ApplicationId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 日志存储目录
    /// </summary>
    public string LogsFolder { get; set; } = "%APPDATA%\\Dorisoy.PanClient\\Logs";

    /// <summary>
    /// 本地数据库目录
    /// </summary>
    public string DbFilename { get; set; } = "%APPDATA%\\Dorisoy.PanClient\\data.db";

    /// <summary>
    /// 本地临时存储目录
    /// </summary>
    public string TempFolder { get; set; } = "%APPDATA%\\Dorisoy.PanClient\\Temps";

    /// <summary>
    /// 客户端模式
    /// </summary>
    public ClientMode ClientMode { get; set; }

    /// <summary>
    /// 加密密钥
    /// </summary>
    public string EncryptionKey { get; set; } = "ewrqewrdsd432432cvxv##24sfdsfsdf";
    public string UserProfilePath { get; set; } = "Users";


    /// <summary>
    /// 本地用户物理存储目录
    /// </summary>
    public string DocumentPath { get; set; } = "Documents";
    public string ExecutableFileTypes { get; set; } = ".bat,.bin,.cmd,.com,.cpl,.exe,.gadget,.inf1,.ins,.inx,.isu,.job,.jse,.lnk,.msc,.msi,.msp,.action,.apk,.app,.command,.csh,.ipa,.ksh,.mst,.osx,.out,.paf,.pif,.run";

    public string ContentRootPath { get; set; } = "";
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
    public static UserAuthDto CurrentUser { get; set; } = new();

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
