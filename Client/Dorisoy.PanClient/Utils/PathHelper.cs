using Dorisoy.PanClient.Common;

namespace Dorisoy.PanClient.Utils;

public class PathHelper
{
    public ISettingsProvider<AppSettings> _configuration;
    public PathHelper()
    {
        this._configuration = Locator.Current.GetService<ISettingsProvider<AppSettings>>();
    }

    public string DocumentPath
    {
        get
        {
            return _configuration.Settings.DocumentPath;
        }
    }

    public string EncryptionKey
    {
        get
        {
            return _configuration.Settings.EncryptionKey;
        }
    }

    public string UserProfilePath
    {
        get
        {
            return _configuration.Settings.UserProfilePath;
        }
    }

    public List<string> ExecutableFileTypes
    {
        get
        {
            try
            {
                return _configuration.Settings.ExecutableFileTypes.Split(',').ToList();
            }
            catch
            {
                return new List<string>();
            }
        }
    }


    public string ContentRootPath
    {
        get
        {
            var contentRootPath = _configuration.Settings.ContentRootPath;
            return contentRootPath;
        }
    }

    public string AppRootPath
    {
        get
        {
            var contentRootPath = System.AppDomain.CurrentDomain.BaseDirectory;
            return contentRootPath;
        }
    }
}
