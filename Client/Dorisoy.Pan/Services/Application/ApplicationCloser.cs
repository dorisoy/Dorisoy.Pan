namespace Dorisoy.PanClient.Services;

public class ApplicationCloser : IApplicationCloser
{
    public void Shutdown()
    {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;

        lifetime.Shutdown();
    }
}
