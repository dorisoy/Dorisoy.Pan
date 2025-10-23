using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetEmailSMTPSettingsQueryHandler : IRequestHandler<GetEmailSMTPSettingsQuery, List<EmailSMTPSettingDto>>
    {
        private readonly IEmailSMTPSettingRepository _emailSMTPSettingRepository;
        private readonly IMapper _mapper;

        public GetEmailSMTPSettingsQueryHandler(
            IEmailSMTPSettingRepository emailSMTPSettingRepository,
            IMapper mapper)
        {
            _emailSMTPSettingRepository = emailSMTPSettingRepository;
            _mapper = mapper;

        }
        public async Task<List<EmailSMTPSettingDto>> Handle(GetEmailSMTPSettingsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _emailSMTPSettingRepository.All.ToListAsync();
            return _mapper.Map<List<EmailSMTPSettingDto>>(entities);
        }
    }
}
