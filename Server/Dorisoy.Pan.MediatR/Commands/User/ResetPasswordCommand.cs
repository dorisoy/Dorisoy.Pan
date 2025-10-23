using Dorisoy.Pan.Data.Dto;
using MediatR;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class ResetPasswordCommand : IRequest<ServiceResponse<UserDto>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
