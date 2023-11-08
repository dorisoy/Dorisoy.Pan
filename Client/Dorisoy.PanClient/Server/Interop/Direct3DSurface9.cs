using System.Runtime.InteropServices;

namespace Sinol.CaptureManager.Server.Interop;

internal sealed class Direct3DSurface9 : IDisposable
{
    private ComInterface.IDirect3DSurface9 comObject;
    private nint native;
    private nint m_sharedhandle;

    public nint SharedHandle { get { return m_sharedhandle; } }

    public ComInterface.IDirect3DSurface9 texture { get { return comObject; } }

    internal Direct3DSurface9(ComInterface.IDirect3DSurface9 obj, nint sharedhandle)
    {
        comObject = obj;
        m_sharedhandle = sharedhandle;
        native = Marshal.GetIUnknownForObject(comObject);
    }

    ~Direct3DSurface9()
    {
        Release();
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
