using System.Globalization;
using Dorisoy.Pan.Localization;

namespace Dorisoy.Pan.Properties;

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
