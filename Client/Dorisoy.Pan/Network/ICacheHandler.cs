namespace Dorisoy.Pan;

public interface ICacheHandler
{
    Task Set(string key, object value, TimeSpan? lifeSpan = null, CancellationToken cancellationToken = default);
    Task<T> Get<T>(string key, CancellationToken cancellationToken = default);
    Task<bool> Remove(string key, CancellationToken cancellationToken = default);
    Task Clear(CancellationToken cancellationToken = default);
}
