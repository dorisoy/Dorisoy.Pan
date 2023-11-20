using AutoMapper;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Commands;

namespace Dorisoy.Pan.API.Helpers.Mapping
{
    public class EmailProfile : Profile
    {
        public EmailProfile()
        {
            CreateMap<EmailSMTPSetting, EmailSMTPSettingDto>().ReverseMap();
            CreateMap<EmailSMTPSetting, AddEmailSMTPSettingCommand>().ReverseMap();
            CreateMap<EmailSMTPSetting, UpdateEmailSMTPSettingCommand>().ReverseMap();
        }
    }
}
