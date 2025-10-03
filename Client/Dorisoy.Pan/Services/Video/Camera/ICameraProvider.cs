namespace Dorisoy.PanClient.Services.Video.Camera;


/// <summary>
/// 用于表示一个摄像头提供器
/// </summary>
public interface ICameraProvider
{
    int FrameHeight { get; set; }
    int FrameWidth { get; set; }

    /// <summary>
    /// 释放非托管
    /// </summary>
    void Dispose();

    /// <summary>
    /// 从视频文件或捕获设备抓取下一帧
    /// </summary>
    /// <returns></returns>
    bool Grab();

    /// <summary>
    /// 是否已经打开
    /// </summary>
    /// <returns></returns>
    bool IsOpened();

    /// <summary>
    /// 打开摄像头
    /// </summary>
    /// <param name="camIdx"></param>
    void Open(int camIdx);

    /// <summary>
    /// 关闭视频文件或捕获设备
    /// </summary>
    void Release();

    /// <summary>
    /// 解码并返回抓取的视频帧
    /// </summary>
    /// <param name="im"></param>
    /// <returns></returns>
    bool Retrieve(out ImageReference im);
}

