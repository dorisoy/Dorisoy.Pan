using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, ServiceResponse<EmailDto>>
    {
        private readonly IEmailSMTPSettingRepository _emailSMTPSettingRepository;
        private readonly ILogger<SendEmailCommandHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        public SendEmailCommandHandler(
           IEmailSMTPSettingRepository emailSMTPSettingRepository,
            ILogger<SendEmailCommandHandler> logger,
            UserInfoToken userInfoToken
            )
        {
            _emailSMTPSettingRepository = emailSMTPSettingRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }
        public async Task<ServiceResponse<EmailDto>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            var defaultSmtp = await _emailSMTPSettingRepository.FindBy(c => c.IsDefault).FirstOrDefaultAsync();
            if (defaultSmtp == null)
            {
                _logger.LogError("Default SMTP setting does not exist.");
                return ServiceResponse<EmailDto>.Return404("Default SMTP setting does not exist.");
            }
            try
            {
                EmailHelper.SendEmail(new SendEmailSpecification
                {
                    Body = request.Body,
                    FromAddress = _userInfoToken.Email,
                    Host = defaultSmtp.Host,
                    IsEnableSSL = defaultSmtp.IsEnableSSL,
                    Password = defaultSmtp.Password,
                    Port = defaultSmtp.Port,
                    Subject = request.Subject,
                    ToAddress = request.ToAddress,
                    CCAddress = request.CCAddress,
                    UserName = defaultSmtp.UserName,
                    Attechments = request.Attechments
                });
                return ServiceResponse<EmailDto>.ReturnSuccess();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return ServiceResponse<EmailDto>.ReturnFailed(500, e.Message);
            }
        }
    }
}
