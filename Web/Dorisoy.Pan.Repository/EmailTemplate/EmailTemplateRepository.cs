using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;

namespace Dorisoy.Pan.Repository
{
    public class EmailTemplateRepository : GenericRepository<EmailTemplate, DocumentContext>,
          IEmailTemplateRepository
    {
        public EmailTemplateRepository(
            IUnitOfWork<DocumentContext> uow
            ) : base(uow)
        {

        }
    }
}

