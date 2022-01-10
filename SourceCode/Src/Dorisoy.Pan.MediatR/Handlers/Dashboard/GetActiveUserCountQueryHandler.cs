using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    /// <summary>
    /// 定义获取有效用户消息处理类
    /// </summary>
    public class GetActiveUserCountQueryHandler : IRequestHandler<GetActiveUserCountQuery, int>
    {
        private readonly IUserRepository _userRepository;
        public GetActiveUserCountQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public Task<int> Handle(GetActiveUserCountQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userRepository.All.Where(c => c.IsActive).Count());
        }
    }
}
