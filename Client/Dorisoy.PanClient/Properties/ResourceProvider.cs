using System.Globalization;
using Sinol.CaptureManager.Localization;

namespace Sinol.CaptureManager.Properties;

public class ResourceProvider : IResourceProvider
{
    public ResourceProvider()
    {

    }
    public void ChangeResources()
    {
        Language.Culture = new CultureInfo("en");
    }
}
