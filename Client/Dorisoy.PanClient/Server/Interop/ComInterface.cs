using System.Runtime.InteropServices;

namespace Sinol.CaptureManager.Server.Interop;

internal static class ComInterface
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int CreateDeviceEx(IDirect3D9Ex d3D9, uint Adapter, int DeviceType, nint hFocusWindow, int BehaviorFlags, NativeStructs.D3DPRESENT_PARAMETERS pPresentationParameters, NativeStructs.D3DDISPLAYMODEEX pFullscreenDisplayMode, out IDirect3DDevice9Ex ppReturnedDeviceInterface);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int CreateDevice_Ex(IDirect3D9Ex d3D9, uint Adapter, int DeviceType, nint hFocusWindow, int BehaviorFlags, NativeStructs.D3DPRESENT_PARAMETERS pPresentationParameters, out IDirect3DDevice9 ppReturnedDeviceInterface);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int CreateDevice(IDirect3D9 d3D9, uint Adapter, int DeviceType, nint hFocusWindow, int BehaviorFlags, NativeStructs.D3DPRESENT_PARAMETERS pPresentationParameters, out IDirect3DDevice9 ppReturnedDeviceInterface);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int CreateRenderTargetEx(IDirect3DDevice9Ex device, uint Width, uint Height, int Format, int MultiSample, uint MultisampleQuality, int Lockable, out IDirect3DSurface9 ppSurface, ref nint pSharedHandle);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int CreateRenderTarget(IDirect3DDevice9 device, uint Width, uint Height, int Format, int MultiSample, uint MultisampleQuality, int Lockable, out IDirect3DSurface9 ppSurface, ref nint pSharedHandle);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int CreateTextureEx(IDirect3DDevice9Ex device, uint Width, uint Height, uint Levels, uint Usage, int Format, int Pool, out IDirect3DTexture9 ppTexture, ref nint pSharedHandle);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int CreateTexture(IDirect3DDevice9 device, uint Width, uint Height, uint Levels, uint Usage, int Format, int Pool, out IDirect3DTexture9 ppTexture, ref nint pSharedHandle);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int CreateOffscreenPlainSurface(IDirect3DDevice9 device, uint Width, uint Height, int Format, int Pool, out IDirect3DTexture9 ppTexture, ref nint pSharedHandle);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate uint GetLevelCount(IDirect3DTexture9 texture);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate int GetSurfaceLevel(IDirect3DTexture9 texture, uint level, out IDirect3DSurface9 ppSurface);

    [ComImport, Guid("02177241-69FC-400C-8FF1-93A44DF6861D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3D9Ex { }

    [ComImport, Guid("81BDCBCA-64D4-426d-AE8D-AD0147F4275C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3D9 { }

    [ComImport, Guid("B18B10CE-2649-405a-870F-95F777D4313A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3DDevice9Ex { }

    [ComImport, Guid("D0223B96-BF7A-43fd-92BD-A43B0D82B9EB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3DDevice9
    {
    }

    [ComImport, Guid("0CFBAF3A-9FF6-429a-99B3-A2796AF8B89B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3DSurface9
    {
    }

    [ComImport, Guid("85C31227-3DE5-4f00-9B3A-F11AC38C18B5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3DTexture9
    {
    }




    // This is a helper method that accesses the COM objects v-table and
    // turns it into a delegate.
    public static bool GetComMethod<T, U>(T comObj, int slot, out U method) where U : class
    {
        var objectAddress = Marshal.GetComInterfaceForObject(comObj, typeof(T));
        if (objectAddress == nint.Zero)
        {
            method = null;
            return false;
        }

        try
        {
            var vTable = Marshal.ReadIntPtr(objectAddress, 0);
            var methodAddress = Marshal.ReadIntPtr(vTable, slot * nint.Size);

            // We can't have a Delegate constraint, so we have to cast to
            // object then to our desired delegate
            method = (U)(object)Marshal.GetDelegateForFunctionPointer(methodAddress, typeof(U));
            return true;
        }
        finally
        {
            Marshal.Release(objectAddress); // Prevent memory leak
        }
    }
}
