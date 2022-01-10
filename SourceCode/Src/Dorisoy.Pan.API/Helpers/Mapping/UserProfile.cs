using AutoMapper;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Commands;

namespace Dorisoy.Pan.API.Helpers.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<AddUserCommand, User>();
            CreateMap<ResetPasswordCommand, UserDto>();
        }
    }
}
