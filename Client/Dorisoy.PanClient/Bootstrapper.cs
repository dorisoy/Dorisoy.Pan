using System.Reflection;
using System.Text.Json;
using AutoMapper;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;
using Dorisoy.PanClient.Common;
using Dorisoy.PanClient.Data.Contexts;
using Dorisoy.PanClient.Mapping;
using Dorisoy.PanClient.Utils;
using Dorisoy.PanClient.ViewModels;
using ILogger = Splat.ILogger;
using Locator = Splat.Locator;

namespace Dorisoy.PanClient;

public static class Bootstrapper
{
    public static void RegisterEnvironmentServices(this IMutableDependencyResolver services)
    {
        services.Register<IPlatformService>(() => new PlatformService());
    }

    public static void RegisterAvaloniaServices(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton<IAppState>(() => new AppState());
        services.RegisterLazySingleton<IApplicationCloser>(() => new ApplicationCloser());
        services.RegisterLazySingleton<IApplicationDispatcher>(() => new AvaloniaDispatcher());
        services.RegisterLazySingleton<IApplicationVersionProvider>(() => new ApplicationVersionProvider());
    }

    public static void AddApplicationInfo(this IMutableDependencyResolver services)
    {
        //注册本地资源 
        services.RegisterLazySingleton<IApplicationInfo>(() => new ApplicationInfo(Assembly.GetExecutingAssembly()));
    }

    public static void AddAutomapper(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton(() =>
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            return config.CreateMapper();
        });
    }

    public static void AddSettingsProvider(this IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        string settingsPath = "";
        var platformService = resolver.GetRequiredService<IPlatformService>();
        var platform = platformService.GetPlatform();

        AppSettings defaultAppSettings = null;

        if (platform == Platform.Windows)
        {
            settingsPath = Environment.ExpandEnvironmentVariables("%APPDATA%\\Dorisoy.PanClient\\appsettings.json");
            defaultAppSettings = new AppSettings();
        }

        var settingsProvider = new JsonSettingsProvider<AppSettings>(settingsPath, defaultAppSettings,
            new JsonSerializerOptions() { WriteIndented = true });

        services.RegisterLazySingleton<ISettingsProvider<AppSettings>>(() =>
            new JsonSettingsProvider<AppSettings>(settingsPath, defaultAppSettings,
            new JsonSerializerOptions() { WriteIndented = true }));

    }

    public static void AddLogging(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton(() =>
        {
            var settings = Locator.Current.GetService<ISettingsProvider<AppSettings>>();

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(Path.Combine(Environment.ExpandEnvironmentVariables(settings.Settings.LogsFolder), "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            return new SerilogLoggerProvider(logger).CreateLogger(nameof(App));
        });
    }

    public static void AddDatabase(this IMutableDependencyResolver services)
    {
        var settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();
        var optionsBuilder = new DbContextOptionsBuilder<CaptureManagerContext>();
        var path = Environment.ExpandEnvironmentVariables(settingsProvider.Settings.DbFilename);
        var conn = settingsProvider.Settings.DB_Conn;

        //MySQl
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 26));
        optionsBuilder
            .UseLazyLoadingProxies()
            .UseMySql(conn, serverVersion, options => options.MigrationsAssembly("Dorisoy.PanClient.Data"))
            .EnableSensitiveDataLogging();

        services.Register(() => new CaptureManagerContext(optionsBuilder.Options));
        services.RegisterLazySingleton<Services.IDbContextFactory<CaptureManagerContext>>(() => new CaptureManagerDbContextFactory());
    }

    public static void AddServices(this IMutableDependencyResolver svc)
    {
        var logger = Locator.Current.GetService<ILogger>();
        svc.Register(() => new MakeRequest(logger));

        var db = Locator.Current.GetService<Services.IDbContextFactory<CaptureManagerContext>>();
        var mapper = Locator.Current.GetService<IMapper>();
        var makeRequest = Locator.Current.GetService<MakeRequest>();
        var settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();
        var setting = settingsProvider.Settings;

        //AddRemoteServices
        svc.RegisterLazySingleton<IAuthenticationService>(() => new AuthenticationService(makeRequest, mapper, setting));
        svc.RegisterLazySingleton<IOnlineUserService>(() => new OnlineUserService(makeRequest, mapper, setting));
        svc.RegisterLazySingleton<IFolderService>(() => new FolderService(makeRequest, mapper, setting));
        svc.RegisterLazySingleton<IRemoteVirtualFolderService>(() => new RemoteVirtualFolderService(makeRequest, mapper, setting));
        svc.RegisterLazySingleton<IRemoteDocumentService>(() => new RemoteDocumentService(makeRequest, mapper, setting));

        //注册WhiteBoardService
        svc.RegisterLazySingleton<IWhiteBoardService>(() => new WhiteBoardService(makeRequest, setting));
        svc.RegisterLazySingleton<ILoginService>(() => new LoginService(db));
        svc.RegisterLazySingleton<IUsersService>(() => new UsersService(db, makeRequest, mapper, setting));
        svc.RegisterLazySingleton<IRoleService>(() => new RoleService(db, mapper));
        svc.RegisterLazySingleton<IDocumentService>(() => new DocumentService(db, mapper));
        svc.RegisterLazySingleton<IDocumentCommentService>(() => new DocumentCommentService(db, mapper));
        svc.RegisterLazySingleton<IPhysicalFolderService>(() => new PhysicalFolderService(db, mapper));
        svc.RegisterLazySingleton<IPhysicalFolderUserService>(() => new PhysicalFolderUserService(db, mapper));
        svc.RegisterLazySingleton<IVirtualFolderService>(() => new VirtualFolderService(db, mapper));
        svc.RegisterLazySingleton<IVirtualFolderUserUserService>(() => new VirtualFolderUserUserService(db, mapper));
        svc.RegisterLazySingleton<IRoleClaimService>(() => new RoleClaimService(db, mapper));
        svc.RegisterLazySingleton<IDepartmentService>(() => new DepartmentService(db, mapper));
        svc.RegisterLazySingleton<IDocumentDeletedService>(() => new DocumentDeletedService(db, mapper));
    }



    /// <summary>
    /// 注册ViewModel
    /// </summary>
    /// <param name="services"></param>
    /// <param name="resolver"></param>
    public static void RegisterViewModels(this IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        var usersService = resolver.GetService<IUsersService>();
        var departmentService = resolver.GetService<IDepartmentService>();
        var roleService = resolver.GetService<IRoleService>();

        //注册导航
        services.RegFN(new HomePageViewModel(), "HomePage");
        services.RegFN(new UserPageViewModel(usersService, departmentService, roleService), "UserPage");
        services.RegFN(new RolePageViewModel(usersService, departmentService, roleService), "RolePage");
        services.RegFN(new PermissionPageViewModel(usersService, departmentService, roleService), "PermissionPage");
        services.RegFN(new DocumentPageViewModel(usersService), "DocumentPage");
        services.RegFN(new SettingsPageViewModel(), "SettingsPage");
    }

    public static void ConfigureDatabase(this IReadonlyDependencyResolver services)
    {
        var db = services.GetService<CaptureManagerContext>();
        db.Database.Migrate();
    }
}

public static class ReadonlyDependencyResolverExtensions
{
    public static IMutableDependencyResolver RegFN(this IMutableDependencyResolver services, 
        MainPageViewModelBase mpb, string tag)
    {
        services.Register<IFrameNavigatedFrom>(() => mpb, tag);
        return services;
    }

    public static TService GetRequiredService<TService>(this IReadonlyDependencyResolver resolver)
    {
        var service = resolver.GetService<TService>();
        if (service is null)
            throw new InvalidOperationException($"Failed to resolve object of type {typeof(TService)}");

        return service;
    }
    public static object GetRequiredService(this IReadonlyDependencyResolver resolver, Type type)
    {
        var service = resolver.GetService(type);
        if (service is null)
        {
            throw new InvalidOperationException($"Failed to resolve object of type {type}");
        }
        return service;
    }
}
