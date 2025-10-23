using MediatR;
using Microsoft.AspNetCore.Http;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class UpdateUserProfilePhotoCommand : IRequest<ServiceResponse<UserDto>>
    {
        public IFormFileCollection FormFile { get; set; }
        public string RootPath { get; set; }
    }
}
