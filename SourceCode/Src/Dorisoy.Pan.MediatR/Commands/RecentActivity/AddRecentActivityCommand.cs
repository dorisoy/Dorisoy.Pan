using Dorisoy.Pan.Data;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class AddRecentActivityCommand : IRequest<bool>
    {
        public Guid? FolderId { get; set; }
        public Guid? DocumentId { get; set; }
        public RecentActivityType Action { get; set; }
    }
}
