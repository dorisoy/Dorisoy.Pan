using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;

namespace Dorisoy.Pan.Repository
{
    public class DocumentSharedUserRepository : GenericRepository<SharedDocumentUser, DocumentContext>,
              IDocumentSharedUserRepository
    {
        public DocumentSharedUserRepository(
          IUnitOfWork<DocumentContext> uow
          ) : base(uow)
        {
        }

    }

}
