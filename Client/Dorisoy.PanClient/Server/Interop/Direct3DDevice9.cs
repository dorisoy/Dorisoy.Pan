using System.Runtime.InteropServices;

namespace Sinol.CaptureManager.Server.Interop;

internal sealed class Direct3DDevice9 : IDisposable
{
    private ComInterface.IDirect3DDevice9 comObject;
    private ComInterface.CreateRenderTarget createRenderTarget;
    private ComInterface.CreateTexture createTexture;
    private ComInterface.CreateOffscreenPlainSurface createOffscreenPlainSurface;

    internal Direct3DDevice9(ComInterface.IDirect3DDevice9 obj)
    {
        comObject = obj;
        ComInterface.GetComMethod(comObject, 23, out createTexture);
        ComInterface.GetComMethod(comObject, 28, out createRenderTarget);
        ComInterface.GetComMethod(comObject, 36, out createOffscreenPlainSurface);
    }

    ~Direct3DDevice9()
    {
        Release();
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }

    public Direct3DSurface9 CreateRenderTarget(uint Width, uint Height, int Format, int MultiSample, uint MultisampleQuality, int Lockable, ref nint pSharedHandle)
    {
        ComInterface.IDirect3DSurface9 obj = null;
        var result = createRenderTarget(comObject, Width, Height, Format, MultiSample, MultisampleQuality, Lockable, out obj, ref pSharedHandle);
        Marshal.ThrowExceptionForHR(result);

        return new Direct3DSurface9(obj, nint.Zero);
    }

    public Direct3DTexture9 CreateTexture(uint Width, uint Height, uint Levels, uint Usage, int Format, int Pool)
    {
        var handle = new nint(0);
        ComInterface.IDirect3DTexture9 obj = null;
        var result = createTexture(comObject, Width, Height, Levels, Usage, Format, Pool, out obj, ref handle);
        Marshal.ThrowExceptionForHR(result);

        return new Direct3DTexture9(obj, handle);
    }

    public Direct3DTexture9 CreateOffscreenPlainSurface(uint Width, uint Height, int Format, int Pool)
    {
        var handle = new nint(0);
        ComInterface.IDirect3DTexture9 obj = null;
        var result = createOffscreenPlainSurface(comObject, Width, Height, Format, Pool, out obj, ref handle);
        Marshal.ThrowExceptionForHR(result);

        return new Direct3DTexture9(obj, handle);
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
