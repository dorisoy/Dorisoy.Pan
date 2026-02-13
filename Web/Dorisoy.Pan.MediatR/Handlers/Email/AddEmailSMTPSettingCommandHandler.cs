using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
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
    public class AddEmailSMTPSettingCommandHandler : IRequestHandler<AddEmailSMTPSettingCommand, ServiceResponse<EmailSMTPSettingDto>>
    {
        private readonly IEmailSMTPSettingRepository _emailSMTPSettingRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<AddEmailSMTPSettingCommand> _logger;
        public AddEmailSMTPSettingCommandHandler(
           IEmailSMTPSettingRepository emailSMTPSettingRepository,
            IMapper mapper,
            IUnitOfWork<DocumentContext> uow,
            ILogger<AddEmailSMTPSettingCommand> logger
            )
        {
            _emailSMTPSettingRepository = emailSMTPSettingRepository;
            _mapper = mapper;
            _uow = uow;
            _logger = logger;
        }
        public async Task<ServiceResponse<EmailSMTPSettingDto>> Handle(AddEmailSMTPSettingCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<EmailSMTPSetting>(request);
            _emailSMTPSettingRepository.Add(entity);

            // remove other as default
            if (entity.IsDefault)
            {
                var defaultEmailSMTPSettings = await _emailSMTPSettingRepository.All.Where(c => c.IsDefault).ToListAsync();
                defaultEmailSMTPSettings.ForEach(c => c.IsDefault = false);
                _emailSMTPSettingRepository.UpdateRange(defaultEmailSMTPSettings);
            }
            if (await _uow.SaveAsync() <= 0)
            {
                return ServiceResponse<EmailSMTPSettingDto>.Return500();
            }
            var entityDto = _mapper.Map<EmailSMTPSettingDto>(entity);
            return ServiceResponse<EmailSMTPSettingDto>.ReturnResultWith200(entityDto);
        }
    }
}
