using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class UserIntoLoginCommand : IRequest<ServiceResponse<UserAuthDto>>
    {
        public string Email { get; set; }
    }
}
