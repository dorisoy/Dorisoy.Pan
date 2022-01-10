using Dorisoy.Pan.Data.Dto;
using MediatR;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class AddUserCommand : IRequest<ServiceResponse<UserDto>>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string Address { get; set; }
        public bool IsAdmin { get; set; }
        public UserClaimDto UserClaims { get; set; }
    }
}
