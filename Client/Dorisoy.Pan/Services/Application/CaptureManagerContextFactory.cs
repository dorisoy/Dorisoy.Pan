using Dorisoy.Pan.Data.Contexts;

namespace Dorisoy.Pan.Services;

public class CaptureManagerDbContextFactory : IDbContextFactory<CaptureManagerContext>
{
    public CaptureManagerContext Create()
    {
        return Locator.Current.GetService<CaptureManagerContext>();
    }
}
