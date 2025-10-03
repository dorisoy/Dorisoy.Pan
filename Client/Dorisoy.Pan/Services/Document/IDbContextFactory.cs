using Microsoft.EntityFrameworkCore;

namespace Dorisoy.PanClient.Services
{
    public interface IDbContextFactory<T> where T : DbContext
    {
        T Create();
    }
}
