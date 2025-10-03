using DirectShowLib;
using Dorisoy.PanClient.Models;

namespace Dorisoy.PanClient.Webcam;

/// <summary>
/// 以与OpenCv相同的顺序枚举摄影机。使用OpenCv，您无法通过名称连接到相机，必须使用索引。
/// OpenCv中的索引基于与计算机的连接顺序。WMI都没有给你这些信息，
/// 所以我不得不引用 DirectShowLib，幸运的是，它的作用与OpenCv相同。（据我所知！
/// </summary>
public static class CameraDevicesEnumerator
{
    /// <summary>
    /// 获取摄像头驱动设备
    /// </summary>
    /// <returns></returns>
    public static List<CameraDevice> EnumerateDevices()
    {
        var cameras = new List<CameraDevice>();
        var videoInputDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

        int openCvId = 0;
        return videoInputDevices.Select(v => new CameraDevice()
        {
            DeviceId = v.DevicePath,
            Name = v.Name,
            OpenCvId = openCvId++
        }).ToList();
    }
}
