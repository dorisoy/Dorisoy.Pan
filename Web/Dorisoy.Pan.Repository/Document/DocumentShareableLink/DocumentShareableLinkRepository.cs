using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;

namespace Dorisoy.Pan.Repository
{
    public class DocumentShareableLinkRepository : GenericRepository<DocumentShareableLink, DocumentContext>, IDocumentShareableLinkRepository
    {
        public DocumentShareableLinkRepository(IUnitOfWork<DocumentContext> uow) : base(uow)
        {
        }
    }
}
