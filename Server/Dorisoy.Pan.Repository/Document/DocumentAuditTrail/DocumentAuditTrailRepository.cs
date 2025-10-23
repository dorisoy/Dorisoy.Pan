using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;

namespace Dorisoy.Pan.Repository
{
    public class DocumentAuditTrailRepository : GenericRepository<DocumentAuditTrail, DocumentContext>,
          IDocumentAuditTrailRepository
    {
        public DocumentAuditTrailRepository(
            IUnitOfWork<DocumentContext> uow
            ) : base(uow)
        {
        }
    }
}
