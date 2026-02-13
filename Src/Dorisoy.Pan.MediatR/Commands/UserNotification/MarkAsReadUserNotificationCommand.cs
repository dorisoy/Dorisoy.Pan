using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class MarkAsReadUserNotificationCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
