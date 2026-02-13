using AutoMapper;
using Dorisoy.Pan.Data.Dto;

namespace Dorisoy.Pan.API.Helpers.Mapping
{
    public class NLogProfile : Profile
    {
        public NLogProfile()
        {
            CreateMap<Data.NLog, NLogDto>().ReverseMap();
        }
    }
}
