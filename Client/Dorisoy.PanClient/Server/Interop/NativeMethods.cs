using System.Runtime.InteropServices;

namespace Sinol.CaptureManager.Server.Interop;

internal static class NativeMethods
{
    [DllImport("d3d9.dll")]
    public static extern int Direct3DCreate9Ex(int SDKVersion, out ComInterface.IDirect3D9Ex directX);

    [DllImport("d3d9.dll")]
    public static extern ComInterface.IDirect3D9 Direct3DCreate9(int SDKVersion);

    [DllImport("user32.dll", SetLastError = false)]
    public static extern nint GetDesktopWindow();
}
