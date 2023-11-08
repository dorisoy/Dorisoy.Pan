using System.Runtime.InteropServices;

namespace Sinol.CaptureManager.Server.Interop;

internal static class NativeStructs
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class D3DDISPLAYMODEEX
    {
        public int Size;
        public int Width;
        public int Height;
        public int RefreshRate;
        public int Format;
        public int ScanLineOrdering;
    }

    [StructLayout(LayoutKind.Sequential)]
    public sealed class D3DDISPLAYMODE
    {
        public int Width;
        public int Height;
        public int RefreshRate;
        public int Format;
    }

    [StructLayout(LayoutKind.Sequential)]
    public sealed class D3DPRESENT_PARAMETERS
    {
        public int BackBufferWidth;
        public int BackBufferHeight;
        public int BackBufferFormat;
        public int BackBufferCount;
        public int MultiSampleType;
        public int MultiSampleQuality;
        public int SwapEffect;
        public nint hDeviceWindow;
        public int Windowed;
        public int EnableAutoDepthStencil;
        public int AutoDepthStencilFormat;
        public int Flags;
        public int FullScreen_RefreshRateInHz;
        public int PresentationInterval;
    }
}
