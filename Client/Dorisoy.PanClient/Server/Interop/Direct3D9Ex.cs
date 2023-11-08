using System.Runtime.InteropServices;

namespace Sinol.CaptureManager.Server.Interop;

internal sealed class Direct3D9Ex : IDisposable
{
    private ComInterface.IDirect3D9Ex comObject;
    private ComInterface.CreateDeviceEx createDeviceEx;
    private ComInterface.CreateDevice_Ex createDevice;

    private Direct3D9Ex(ComInterface.IDirect3D9Ex obj)
    {
        comObject = obj;
        ComInterface.GetComMethod(comObject, 16, out createDevice);
        ComInterface.GetComMethod(comObject, 20, out createDeviceEx);
    }

    ~Direct3D9Ex()
    {
        Release();
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }

    public static Direct3D9Ex Create(int SDKVersion)
    {
        ComInterface.IDirect3D9Ex obj;
        Marshal.ThrowExceptionForHR(NativeMethods.Direct3DCreate9Ex(SDKVersion, out obj));

        return new Direct3D9Ex(obj);
    }

    public Direct3DDevice9Ex CreateDeviceEx(uint Adapter, int DeviceType, nint hFocusWindow, int BehaviorFlags,
                                            NativeStructs.D3DPRESENT_PARAMETERS pPresentationParameters, NativeStructs.D3DDISPLAYMODEEX pFullscreenDisplayMode)
    {
        ComInterface.IDirect3DDevice9Ex obj = null;
        var result = createDeviceEx(comObject, Adapter, DeviceType, hFocusWindow, BehaviorFlags, pPresentationParameters, pFullscreenDisplayMode, out obj);
        Marshal.ThrowExceptionForHR(result);

        return new Direct3DDevice9Ex(obj);
    }

    public Direct3DDevice9 CreateDevice(uint Adapter, int DeviceType, nint hFocusWindow, int BehaviorFlags,
                                            NativeStructs.D3DPRESENT_PARAMETERS pPresentationParameters, NativeStructs.D3DDISPLAYMODEEX pFullscreenDisplayMode)
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
            createDeviceEx = null;
        }
    }
}
