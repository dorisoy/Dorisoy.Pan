using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;

namespace Dorisoy.Pan.Repository
{
    public class DocumentDeletedRepository : GenericRepository<DocumentDeleted, DocumentContext>,
             IDocumentDeletedRepository
    {
        public DocumentDeletedRepository(
            IUnitOfWork<DocumentContext> uow
            ) : base(uow)
        {
        }

    }
}
