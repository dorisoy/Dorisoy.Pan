using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;

namespace Dorisoy.Pan.Repository
{
    public class DocumentReminderRepository : GenericRepository<DocumentReminder, DocumentContext>,
            IDocumentReminderRepository
    {
        public DocumentReminderRepository(
            IUnitOfWork<DocumentContext> uow
            ) : base(uow)
        {
        }
    }
}
