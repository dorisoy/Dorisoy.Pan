using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dorisoy.Pan.API.Helpers.Mapping
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var cfg = new MapperConfigurationExpression();
            cfg.AddProfile(new UserProfile());
            cfg.AddProfile(new NLogProfile());
            cfg.AddProfile(new EmailTemplateProfile());
            cfg.AddProfile(new EmailProfile());
            cfg.AddProfile(new FolderProfile());
            cfg.AddProfile(new DocumentProfile());
            cfg.AddProfile(new RecentActivityProfile());
            var mappingConfig = new MapperConfiguration(cfg, NullLoggerFactory.Instance);
            return mappingConfig.CreateMapper();
        }
    }
}
