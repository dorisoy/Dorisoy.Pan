namespace Dorisoy.PanClient;

public abstract class DisposableBase : IDisposable
{
    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            if (disposing)
            {
                DisposeManaged();
            }

            DisposeUnmanaged();

            _disposed = true;
        }
    }

    protected abstract void DisposeManaged();
    protected abstract void DisposeUnmanaged();

    ~DisposableBase()
    {
        Dispose(false);
    }
}

