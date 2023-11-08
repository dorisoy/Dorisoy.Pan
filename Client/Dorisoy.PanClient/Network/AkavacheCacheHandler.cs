using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Akavache;

namespace Sinol.CaptureManager;

public class AkavacheCacheHandler : ICacheHandler
{
    public AkavacheCacheHandler()
    {
        Registrations.Start($"Sinol.CaptureManager.{nameof(AkavacheCacheHandler)}");
    }

    public Task Set(string key, object value, TimeSpan? lifeSpan = null,
        CancellationToken cancellationToken = default)
    {
        return lifeSpan.HasValue
            ? BlobCache.LocalMachine.InsertObject(key, value, lifeSpan.Value).ToTask(cancellationToken)
            : BlobCache.LocalMachine.InsertObject(key, value).ToTask(cancellationToken);
    }

    public Task<T> Get<T>(string key, CancellationToken cancellationToken = default)
    {
        return BlobCache.LocalMachine.GetObject<T>(key)
            .Catch(Observable.Return(default(T)))
            .ToTask(cancellationToken);
    }

    public Task<bool> Remove(string key, CancellationToken cancellationToken = default)
    {
        return BlobCache.LocalMachine.Invalidate(key)
            .SelectMany(_ => Observable.Return(true))
            .Catch(Observable.Return(false))
            .ToTask(cancellationToken);
    }

    public Task Clear(CancellationToken cancellationToken = default)
    {
        return BlobCache.LocalMachine.InvalidateAll().ToTask(cancellationToken);
    }
}
