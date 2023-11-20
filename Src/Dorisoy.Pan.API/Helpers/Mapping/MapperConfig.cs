using AutoMapper;

namespace Dorisoy.Pan.API.Helpers.Mapping
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserProfile());
                mc.AddProfile(new NLogProfile());
                mc.AddProfile(new EmailTemplateProfile());
                mc.AddProfile(new EmailProfile());
                mc.AddProfile(new FolderProfile());
                mc.AddProfile(new DocumentProfile());
                mc.AddProfile(new RecentActivityProfile());
            });
            return mappingConfig.CreateMapper();
        }
    }
}
