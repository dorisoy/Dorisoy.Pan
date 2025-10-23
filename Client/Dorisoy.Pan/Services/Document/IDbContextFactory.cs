using Microsoft.EntityFrameworkCore;

namespace Dorisoy.Pan.Services
{
    public interface IDbContextFactory<T> where T : DbContext
    {
        T Create();
    }
}
