using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class SendDocumentCommandHandler
        : IRequestHandler<SendDocumentCommand, ServiceResponse<bool>>
    {
        private readonly IEmailSMTPSettingRepository _emailSMTPSettingRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly ILogger<SendDocumentCommandHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SendDocumentCommandHandler(IEmailSMTPSettingRepository emailSMTPSettingRepository,
            ILogger<SendDocumentCommandHandler> logger,
            UserInfoToken userInfoToken,
            IDocumentRepository documentRepository,
            PathHelper pathHelper,
            IWebHostEnvironment webHostEnvironment)
        {
            _emailSMTPSettingRepository = emailSMTPSettingRepository;
            _documentRepository = documentRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _pathHelper = pathHelper;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<ServiceResponse<bool>> Handle(SendDocumentCommand request, CancellationToken cancellationToken)
        {
            var defaultSmtp = await _emailSMTPSettingRepository.FindBy(c => c.IsDefault).FirstOrDefaultAsync();
            if (defaultSmtp == null)
            {
                _logger.LogError("Default SMTP setting does not exist.");
                return ServiceResponse<bool>.Return404("Default SMTP setting does not exist.");
            }
            try
            {
                var doc = await _documentRepository.FindAsync(request.Id);

                if (doc == null)
                {
                    return ServiceResponse<bool>.Return404("Document not found.");
                }

                var fileFullpath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, doc.Path);
                if (!File.Exists(fileFullpath))
                {
                    return ServiceResponse<bool>.Return404("File not found.");
                }
                Stream fs = File.OpenRead(fileFullpath);
                var attachment = new System.Net.Mail.Attachment(fs, doc.Name);
                EmailHelper.SendFileOrFolder(new SendEmailSpecification
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
                    Attachment = attachment
                });
                return ServiceResponse<bool>.ReturnSuccess();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return ServiceResponse<bool>.ReturnFailed(500, e.Message);
            }
        }
    }
}
