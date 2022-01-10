using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class NotificationCommand : IRequest<bool>
    {
        public List<Guid> Users { get; set; } = new List<Guid>();
        public Guid FolderId { get; set; }
    }
}
