using MediatR;
using Dorisoy.Pan.Data.Resources;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetNLogsQuery : IRequest<NLogList>
    {
        public NLogResource NLogResource { get; set; }
    }
}
