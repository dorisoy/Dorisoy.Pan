using System.Runtime.InteropServices;

namespace Sinol.CaptureManager.Server.Interop;

internal sealed class Direct3DDevice9Ex : IDisposable
{
    private ComInterface.IDirect3DDevice9Ex comObject;
    private ComInterface.CreateRenderTargetEx createRenderTarget;
    private ComInterface.CreateTextureEx createTexture;


    internal Direct3DDevice9Ex(ComInterface.IDirect3DDevice9Ex obj)
    {
        comObject = obj;
        ComInterface.GetComMethod(comObject, 23, out createTexture);
        ComInterface.GetComMethod(comObject, 28, out createRenderTarget);
    }

    ~Direct3DDevice9Ex()
    {
        Release();
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }

    public Direct3DSurface9 CreateRenderTarget(uint Width, uint Height, int Format, int MultiSample, uint MultisampleQuality, int Lockable)
    {
        var lSharedHandle = nint.Zero;

        ComInterface.IDirect3DSurface9 obj = null;
        var result = createRenderTarget(comObject, Width, Height, Format, MultiSample, MultisampleQuality, Lockable, out obj, ref lSharedHandle);
        Marshal.ThrowExceptionForHR(result);

        return new Direct3DSurface9(obj, lSharedHandle);
    }

    public Direct3DSurface9 CreateTexture(uint Width, uint Height, uint Levels, uint Usage, int Format, int Pool, ref nint pSharedHandle)
    {
        ComInterface.IDirect3DTexture9 obj = null;
        var result = createTexture(comObject, Width, Height, Levels, Usage, Format, Pool, out obj, ref pSharedHandle);
        Marshal.ThrowExceptionForHR(result);

        return null;
    }

    private void Release()
    {
        if (comObject != null)
        {
            Marshal.ReleaseComObject(comObject);
            comObject = null;
            createRenderTarget = null;
        }
    }
}
