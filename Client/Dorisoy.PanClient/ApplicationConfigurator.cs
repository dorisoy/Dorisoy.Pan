using Dorisoy.PanClient.Common;
using Dorisoy.PanClient.Utils;
using Splat.Serilog;

namespace Dorisoy.PanClient;

public static class ApplicationConfigurator
{
    /// <summary>
    /// 应用程序配置 IApplication app, 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="app"></param>
    public static void ConfigureServices(this IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        //注册Serilog
        services.UseSerilogFullLogger();

        //应用程序信息
        services.AddApplicationInfo();

        //系统变量
        services.RegisterEnvironmentServices();

        //Avalonia服务
        services.RegisterAvaloniaServices();

        //系统配置提供
        services.AddSettingsProvider(resolver);

        //日志
        services.AddLogging();


        var _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();
        if (!InternetCheck.PingIpOrDomainName(_settingsProvider.Settings.ServerHost))
        {
            //服务器无法连接
            return;
        }

        //数据持久化
        services.AddDatabase();

        //对象映射
        services.AddAutomapper();

        //自定义服务
        services.AddServices();

        //注册ViewModels
        services.RegisterViewModels(resolver);

    }

    public static void Configure(IReadonlyDependencyResolver services)
    {

        //{
        //    //数据初始化
        //    services.ConfigureDatabase();
        //}
    }
}
