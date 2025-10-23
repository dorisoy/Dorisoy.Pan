using System.Threading.Tasks;
using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Data.Resources;

namespace Dorisoy.Pan.Repository
{
    public interface ILoginAuditRepository : IGenericRepository<LoginAudit>
    {
        Task<LoginAuditList> GetDocumentAuditTrails(LoginAuditResource loginAuditResrouce);
        Task LoginAudit(LoginAuditDto loginAudit);
    }
}
