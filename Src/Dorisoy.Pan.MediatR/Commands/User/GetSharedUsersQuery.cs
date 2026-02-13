using Dorisoy.Pan.Data.Resources;
using Dorisoy.Pan.Repository;
using MediatR;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class GetSharedUsersQuery : IRequest<UserList>
    {
        public UserResource UserResource { get; set; }
    }
}
