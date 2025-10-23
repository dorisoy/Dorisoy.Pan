using System.Text.RegularExpressions;

namespace Dorisoy.Pan.Models;

/// <summary>
/// 表示Chat消息数据模型
/// </summary>
public class ChatDataModel : ReactiveObject
{
    /// <summary>
    /// 是否自己
    /// </summary>
    public bool IsOwenMessage { get; set; }

    /// <summary>
    /// 发送人
    /// </summary>
    public string Sender { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public string Time { get; set; }

    /// <summary>
    /// 对齐方向
    /// </summary>
    public string Allignment { get; set; } = "Left";

    /// <summary>
    /// URL地址
    /// </summary>
    public string URI { get; set; } = "";

    /// <summary>
    ///  URL地址文本高度
    /// </summary>
    public double URIHeight { get => string.IsNullOrEmpty(URI) ? 0 : -1; }


    /// <summary>
    /// 背景颜色
    /// </summary>
    public Avalonia.Media.Color BackgroundColor { get; set; } = Avalonia.Media.Color.FromRgb(30, 30, 30);
   
    /// <summary>
    /// 补白间距
    /// </summary>
    public Thickness RectMargin 
    {
        get => string.IsNullOrEmpty(Sender) ? new Thickness(0, 0, 0, 0) : new Thickness(-3, 10, -3, 0);
    }

    /// <summary>
    /// 发送文本高度
    /// </summary>
    public double SenderTextHeight { get => string.IsNullOrEmpty(Sender) ? 0 : 20; }


    /// <summary>
    /// 创建远程Chat输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    public void CreateRemoteChatEntry(string sender, string message)
    {
        if (IsValidURL(message))
        {
            if (!message.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !message.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                message = "http://" + message;
            URI = message;
            Message = "请接收...";
        }
        else
        {
            Message = message;
        }

        IsOwenMessage = false;
        Sender = sender;
        Time = DateTime.Now.ToShortTimeString();
        Allignment = "Left";
        BackgroundColor = Avalonia.Media.Color.FromRgb(30, 30, 30);
    }

    /// <summary>
    /// 创建本地Chat输入
    /// </summary>
    /// <param name="message"></param>
    /// <param name="timestamp"></param>
    public void CreateLocalChatEntry(string message, DateTime timestamp)
    {
        if (IsValidURL(message))
        {
            if (!message.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !message.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                message = "http://" + message;
            URI = message;
            Message = "请接收...";
        }
        else
        {
            Message = message;
        }

        IsOwenMessage = true;
        Sender = "你";
        Time = timestamp.ToShortTimeString();
        Allignment = "Right";
        BackgroundColor = Avalonia.Media.Color.FromRgb(35, 35, 42);
    }

    /// <summary>
    /// 创建信息提示Chat输入
    /// </summary>
    /// <param name="info"></param>
    /// <param name="url"></param>
    public void CreateInfoChatEntry(string info, string url = null)
    {
        URI = url;
        Message = info;
        Time = DateTime.Now.ToShortTimeString();
        Allignment = "Center";
        BackgroundColor = Avalonia.Media.Color.FromRgb(50, 50, 50);
    }


    const string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
    readonly Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    bool IsValidURL(string URL)
    {
        if (string.IsNullOrEmpty(URL))
            return false;
        return Rgx.IsMatch(URL);
    }
}
