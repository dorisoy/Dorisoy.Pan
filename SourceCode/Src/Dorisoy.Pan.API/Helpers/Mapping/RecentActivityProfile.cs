using AutoMapper;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.MediatR.Commands;

namespace Dorisoy.Pan.API.Helpers.Mapping
{
    public class RecentActivityProfile : Profile
    {
        public RecentActivityProfile()
        {
            CreateMap<RecentActivity, RecentActivityDto>().ReverseMap();
            CreateMap<AddRecentActivityCommand, RecentActivity>();
        }
    }
}
