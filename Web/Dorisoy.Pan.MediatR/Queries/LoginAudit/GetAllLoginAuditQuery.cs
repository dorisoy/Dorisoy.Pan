using MediatR;
using Dorisoy.Pan.Data.Resources;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetAllLoginAuditQuery : IRequest<LoginAuditList>
    {
        public LoginAuditResource LoginAuditResource { get; set; }
    }
}
