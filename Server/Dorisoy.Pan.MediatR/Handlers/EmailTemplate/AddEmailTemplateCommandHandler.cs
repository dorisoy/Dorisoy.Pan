using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class AddEmailTemplateCommandHandler : IRequestHandler<AddEmailTemplateCommand, ServiceResponse<EmailTemplateDto>>
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<AddEmailTemplateCommandHandler> _logger;
        public AddEmailTemplateCommandHandler(
           IEmailTemplateRepository emailTemplateRepository,
            IMapper mapper,
            IUnitOfWork<DocumentContext> uow,
            UserInfoToken userInfoToken,
            ILogger<AddEmailTemplateCommandHandler> logger
            )
        {
            _emailTemplateRepository = emailTemplateRepository;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }
        public async Task<ServiceResponse<EmailTemplateDto>> Handle(AddEmailTemplateCommand request, CancellationToken cancellationToken)
        {
            var entityExist = await _emailTemplateRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();
            if (entityExist != null)
            {
                _logger.LogError("Email Template already exist.");
                return ServiceResponse<EmailTemplateDto>.Return409("Email Template already exist.");
            }
            var entity = _mapper.Map<EmailTemplate>(request);
            entity.Id = Guid.NewGuid();
            entity.CreatedBy = _userInfoToken.Id;
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedBy = _userInfoToken.Id;
            _emailTemplateRepository.Add(entity);
            if (await _uow.SaveAsync() <= 0)
            {
                return ServiceResponse<EmailTemplateDto>.Return500();
            }
            var entityDto = _mapper.Map<EmailTemplateDto>(entity);
            return ServiceResponse<EmailTemplateDto>.ReturnResultWith200(entityDto);
        }
    }
}
