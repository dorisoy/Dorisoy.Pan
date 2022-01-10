using System.Threading.Tasks;
using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Resources;

namespace Dorisoy.Pan.Repository
{
    public interface INLogRespository : IGenericRepository<NLog>
    {
        Task<NLogList> GetNLogsAsync(NLogResource nLogResource);
    }
}
