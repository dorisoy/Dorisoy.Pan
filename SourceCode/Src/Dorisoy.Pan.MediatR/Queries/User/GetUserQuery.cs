using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetUserQuery : IRequest<ServiceResponse<UserDto>>
    {
        public Guid Id { get; set; }
    }
}
