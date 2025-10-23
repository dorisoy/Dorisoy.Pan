using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetNLogsQueryHandler : IRequestHandler<GetNLogsQuery, NLogList>
    {
        private readonly INLogRespository _nLogRespository;
        public GetNLogsQueryHandler(INLogRespository nLogRespository)
        {
            _nLogRespository = nLogRespository;
        }
        public async Task<NLogList> Handle(GetNLogsQuery request, CancellationToken cancellationToken)
        {
            return await _nLogRespository.GetNLogsAsync(request.NLogResource);
        }
    }
}
