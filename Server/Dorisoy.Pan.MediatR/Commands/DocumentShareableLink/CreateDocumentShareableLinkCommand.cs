using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class CreateDocumentShareableLinkCommand : IRequest<ServiceResponse<DocumentShareableLinkDto>>
    {
        public Guid? Id { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime? LinkExpiryTime { get; set; }
        public string Password { get; set; }
        public string LinkCode { get; set; }
        public bool IsAllowDownload { get; set; }
    }
}
