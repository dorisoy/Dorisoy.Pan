using System.Runtime.InteropServices;

namespace Sinol.CaptureManager.Server.Interop;

internal sealed class Direct3D9 : IDisposable
{
    private ComInterface.IDirect3D9 comObject;
    private ComInterface.CreateDevice createDevice;

    private Direct3D9(ComInterface.IDirect3D9 obj)
    {
        comObject = obj;
        ComInterface.GetComMethod(comObject, 16, out createDevice);
    }

    ~Direct3D9()
    {
        Release();
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }

    public static Direct3D9 Create(int SDKVersion)
    {
        ComInterface.IDirect3D9 obj;

        obj = NativeMethods.Direct3DCreate9(SDKVersion);

        return new Direct3D9(obj);
    }

    public Direct3DDevice9 CreateDevice(uint Adapter, int DeviceType, nint hFocusWindow, int BehaviorFlags,
                                            NativeStructs.D3DPRESENT_PARAMETERS pPresentationParameters, NativeStructs.D3DDISPLAYMODE pFullscreenDisplayMode)
    {
        ComInterface.IDirect3DDevice9 obj = null;
        var result = createDevice(comObject, Adapter, DeviceType, hFocusWindow, BehaviorFlags, pPresentationParameters, out obj);

        Marshal.ThrowExceptionForHR(result);

        return new Direct3DDevice9(obj);
    }

    private void Release()
    {
        if (comObject != null)
        {
            Marshal.ReleaseComObject(comObject);
            comObject = null;
            createDevice = null;
        }
    }
}
