using System.IO;
using System.Text.Json;

namespace Dorisoy.PanClient.Common;

public class JsonSettingsProvider<T> : ISettingsProvider<T>
{
    public T Settings { get; private set; }

    private readonly string _jsonFilename;

    private readonly JsonSerializerOptions _serializerOptions = null;

    public JsonSettingsProvider(string jsonFilename, T defaultSettings, JsonSerializerOptions serializerOptions = null)
    {
        _jsonFilename = jsonFilename;
        _serializerOptions = serializerOptions;
        Settings = defaultSettings;
        if(File.Exists(_jsonFilename))
        {
            var content = File.ReadAllText(_jsonFilename);
            Settings = JsonSerializer.Deserialize<T>(content);
        }
        else
        {
            Save();
        }
    }

    public void Save()
    {
        var content = JsonSerializer.Serialize(Settings, _serializerOptions);
        if (File.Exists(_jsonFilename))
        {
            File.WriteAllText(_jsonFilename, content);
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_jsonFilename));
            File.WriteAllText(_jsonFilename, content);
        }
    }
}
