using Dorisoy.Pan.Data;
using MediatR;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetAllRecentActivityQuery : IRequest<List<RecentActivityDto>>
    {
    }
}
