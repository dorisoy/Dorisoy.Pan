using System.Drawing;
using Size = System.Drawing.Size;

namespace Dorisoy.PanClient.Services.ScreenShare;

[SupportedOSPlatform("windows")]
public class ScreenInformations
{
    public static uint RawDpi { get; private set; }
    public static uint RawDpiY { get; private set; }


    static ScreenInformations()
    {
        uint dpiX;
        uint dpiY;
        GetDpi(DpiType.EFFECTIVE, out dpiX, out dpiY);
        RawDpi = dpiX;
        RawDpiY = dpiY;
    }

    /// <summary>
    /// Returns the scaling of the given screen.
    /// </summary>
    /// <param name="dpiType">The type of dpi that should be given back..</param>
    /// <param name="dpiX">Gives the horizontal scaling back (in dpi).</param>
    /// <param name="dpiY">Gives the vertical scaling back (in dpi).</param>
    private static void GetDpi(DpiType dpiType, out uint dpiX, out uint dpiY)
    {
        var point = new System.Drawing.Point(1, 1);
        var hmonitor = MonitorFromPoint(point, _MONITOR_DEFAULTTONEAREST);

        switch (GetDpiForMonitor(hmonitor, dpiType, out dpiX, out dpiY).ToInt32())
        {
            case _S_OK: return;
            case _E_INVALIDARG:
                throw new ArgumentException("Unknown error. See https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx for more information.");
            default:
                throw new COMException("Unknown error. See https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx for more information.");
        }
    }

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062.aspx
    [DllImport("User32.dll")]
    private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx
    [DllImport("Shcore.dll")]
    private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

    const int _S_OK = 0;
    const int _MONITOR_DEFAULTTONEAREST = 2;
    const int _E_INVALIDARG = -2147024809;
    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

    public enum DeviceCap
    {
        VERTRES = 10,
        DESKTOPVERTRES = 117
    }


    public static double GetWindowsScreenScalingFactor(bool percentage = true)
    {
        //Create Graphics object from the current windows handle
        Graphics GraphicsObject = Graphics.FromHwnd(IntPtr.Zero);
        //Get Handle to the device context associated with this Graphics object
        IntPtr DeviceContextHandle = GraphicsObject.GetHdc();
        //Call GetDeviceCaps with the Handle to retrieve the Screen Height
        int LogicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.VERTRES);
        int PhysicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.DESKTOPVERTRES);
        //Divide the Screen Heights to get the scaling factor and round it to two decimals
        double ScreenScalingFactor = Math.Round(PhysicalScreenHeight / (double)LogicalScreenHeight, 2);
        //If requested as percentage - convert it
        if (percentage)
        {
            ScreenScalingFactor *= 100.0;
        }
        //Release the Handle and Dispose of the GraphicsObject object
        GraphicsObject.ReleaseHdc(DeviceContextHandle);
        GraphicsObject.Dispose();
        //Return the Scaling Factor
        return ScreenScalingFactor;
    }


    /// <summary>
    /// 获取显示分辨率
    /// </summary>
    /// <returns></returns>
    public static Size GetDisplayResolution()
    {
        var sf = GetWindowsScreenScalingFactor(false);

        // 获取主屏幕的边界
        var bounds = App.MainWindow.PrimaryScreenBounds;

        // 你可以获取屏幕的宽度和高度信息
        var screenWidth = bounds.Width * sf;
        var screenHeight = bounds.Height * sf;

        return new Size((int)screenWidth, (int)screenHeight);
    }

}

/// <summary>
/// Represents the different types of scaling.
/// </summary>
/// <seealso cref="https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511.aspx"/>
public enum DpiType
{
    EFFECTIVE = 0,
    ANGULAR = 1,
    RAW = 2,
}
