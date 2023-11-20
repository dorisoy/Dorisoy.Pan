using MediatR;
using System.Collections.Generic;
using Dorisoy.Pan.Data.Dto;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetRecentlyRegisteredUserQuery : IRequest<List<UserDto>>
    {
    }
}
