using System.Reflection;

namespace Dorisoy.PanClient.Services
{
    public class ApplicationVersionProvider : IApplicationVersionProvider
    {
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}