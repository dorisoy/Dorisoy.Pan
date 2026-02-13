using MediatR;

namespace Dorisoy.Pan.MediatR.Queries
{
    /// <summary>
    /// 定义获取有效用户消息类
    /// </summary>
    public class GetActiveUserCountQuery : IRequest<int>
    {
    }
}
