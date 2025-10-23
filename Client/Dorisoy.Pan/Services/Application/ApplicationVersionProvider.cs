namespace Dorisoy.Pan.Services
{
    public class ApplicationVersionProvider : IApplicationVersionProvider
    {
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}