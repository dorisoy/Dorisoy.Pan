using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class AddDocumentCommentCommand : IRequest<ServiceResponse<DocumentCommentDto>>
    {
        public string Comment { get; set; }
        public Guid DocumentId { get; set; }
    }
}
