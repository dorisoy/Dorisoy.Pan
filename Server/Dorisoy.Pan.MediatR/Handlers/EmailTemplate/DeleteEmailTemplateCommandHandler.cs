using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class DeleteEmailTemplateCommandHandler : IRequestHandler<DeleteEmailTemplateCommand, ServiceResponse<bool>>
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly ILogger<DeleteEmailTemplateCommandHandler> _logger;
        public DeleteEmailTemplateCommandHandler(
           IEmailTemplateRepository emailTemplateRepository,
            IUnitOfWork<DocumentContext> uow,
            ILogger<DeleteEmailTemplateCommandHandler> logger
            )
        {
            _emailTemplateRepository = emailTemplateRepository;
            _uow = uow;
            _logger = logger;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteEmailTemplateCommand request, CancellationToken cancellationToken)
        {
            var entityExist = await _emailTemplateRepository.FindAsync(request.Id);
            if (entityExist == null)
            {
                _logger.LogError("Email Template Not Found.");
                return ServiceResponse<bool>.Return404();
            }
            entityExist.IsDeleted = true;
            _emailTemplateRepository.Update(entityExist);
            if (await _uow.SaveAsync() <= 0)
            {
                return ServiceResponse<bool>.Return500();
            }
            return ServiceResponse<bool>.ReturnResultWith204();
        }
    }
}
