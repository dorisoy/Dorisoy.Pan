using MediatR;
using System.Collections.Generic;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class SendEmailCommand : IRequest<ServiceResponse<EmailDto>>
    {
        public string Subject { get; set; }
        public string ToAddress { get; set; }
        public string CCAddress { get; set; }
        public List<FileInfo> Attechments { get; set; } = new List<FileInfo>();
        public string Body { get; set; }
        public string FromAddress { get; set; }
    }

}
