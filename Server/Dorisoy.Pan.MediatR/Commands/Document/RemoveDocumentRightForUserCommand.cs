using Dorisoy.Pan.Helper;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class RemoveDocumentRightForUserCommand : IRequest<ServiceResponse<bool>>
    {
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }
    }
}
