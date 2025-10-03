namespace Dorisoy.PanClient.Common;

public interface ISettingsProvider<T>
{
    T Settings { get; }
    void Save();
}
