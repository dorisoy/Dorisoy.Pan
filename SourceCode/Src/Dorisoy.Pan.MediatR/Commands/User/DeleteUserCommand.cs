using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class DeleteUserCommand : IRequest<ServiceResponse<UserDto>>
    {
        public Guid Id { get; set; }
    }
}
