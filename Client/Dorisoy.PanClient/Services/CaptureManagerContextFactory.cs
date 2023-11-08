using Dorisoy.PanClient.Data.Contexts;

namespace Dorisoy.PanClient.Services;

public class CaptureManagerDbContextFactory : IDbContextFactory<CaptureManagerContext>
{
    public CaptureManagerContext Create()
    {
        return Locator.Current.GetService<CaptureManagerContext>();
    }
}
