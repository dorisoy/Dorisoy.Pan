using System.Runtime.InteropServices;

namespace Sinol.CaptureManager.Server.Interop;

class Direct3DTexture9 : IDisposable
{
    private ComInterface.IDirect3DTexture9 comObject;
    private nint native;
    private ComInterface.GetLevelCount getLevelCount;
    private ComInterface.GetSurfaceLevel getSurfaceLevel;
    private nint m_sharedhandle;

    public nint SharedHandle { get { return m_sharedhandle; } }

    internal Direct3DTexture9(ComInterface.IDirect3DTexture9 obj, nint sharedhandle)
    {
        comObject = obj;
        native = Marshal.GetIUnknownForObject(comObject);
        m_sharedhandle = sharedhandle;
        ComInterface.GetComMethod(comObject, 13, out getLevelCount);
        ComInterface.GetComMethod(comObject, 18, out getSurfaceLevel);
    }

    ~Direct3DTexture9()
    {
        Release();
    }

    public uint GetLevelCount()
    {
        ComInterface.IDirect3DTexture9 obj = null;
        var result = getLevelCount(comObject);
        return result;
    }

    public Direct3DSurface9 GetSurfaceLevel(uint level)
    {
        ComInterface.IDirect3DSurface9 obj = null;
        var result = getSurfaceLevel(comObject, level, out obj);
        Marshal.ThrowExceptionForHR(result);

        return new Direct3DSurface9(obj, nint.Zero);
    }

    public nint NativeInterface
    {
        get { return native; }
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }

    private void Release()
    {
        if (comObject != null)
        {
            Marshal.Release(native);
            native = nint.Zero;

            Marshal.ReleaseComObject(comObject);
            comObject = null;
        }
    }
}
