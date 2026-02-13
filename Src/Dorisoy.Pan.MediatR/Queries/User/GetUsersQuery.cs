using MediatR;
using Dorisoy.Pan.Data.Resources;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetUsersQuery : IRequest<UserList>
    {
        public UserResource UserResource { get; set; }
    }
}
