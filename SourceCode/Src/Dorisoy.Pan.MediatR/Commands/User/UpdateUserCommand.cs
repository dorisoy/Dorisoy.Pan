using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class UpdateUserCommand : IRequest<ServiceResponse<UserDto>>
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string Address { get; set; }
        public bool IsAdmin { get; set; }
        public UserClaimDto UserClaims { get; set; }
    }
}
