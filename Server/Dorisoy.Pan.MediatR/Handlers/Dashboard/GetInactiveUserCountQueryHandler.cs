using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetInactiveUserCountQueryHandler : IRequestHandler<GetInactiveUserCountQuery, int>
    {
        private readonly IUserRepository _userRepository;
        public GetInactiveUserCountQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public Task<int> Handle(GetInactiveUserCountQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userRepository.All.Where(c => !c.IsActive).Count());
        }
    }
}
