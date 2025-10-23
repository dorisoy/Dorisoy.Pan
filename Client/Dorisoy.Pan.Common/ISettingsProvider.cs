namespace Dorisoy.Pan.Common;

public interface ISettingsProvider<T>
{
    T Settings { get; }
    void Save();
}
