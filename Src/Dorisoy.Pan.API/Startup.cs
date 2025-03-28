using Dorisoy.Pan.API.Helpers;
using Dorisoy.Pan.API.Helpers.Mapping;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.PipeLineBehavior;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Dorisoy.Pan.Repository;
using Microsoft.AspNetCore.Rewrite;
using NewLife.Redis.Core;


namespace Dorisoy.Pan.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("Dorisoy.Pan.MediatR");
            services.AddMediatR(c =>
            {
                c.RegisterServicesFromAssembly(assembly);
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblies(Enumerable.Repeat(assembly, 1));


            //JWT
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            JwtSettings settings;
            settings = GetJwtSettings();
            services.AddSingleton(settings);
            var contentRootPath = Env.ContentRootPath;
            var pathHelper = new PathHelper(Configuration, contentRootPath);
            services.AddSingleton(pathHelper);
            services.AddSingleton<IConnectionMappingRepository, ConnectionMappingRepository>();
            services.AddScoped(c => new UserInfoToken() { Id = Guid.NewGuid() });
            services.AddNewLifeRedis();

            //数据库
            services.AddDbContextPool<DocumentContext>(options =>
            {
                //MySQl
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
                options.UseMySql(Configuration.GetConnectionString("DocumentDbConnectionString"), serverVersion)
                .EnableSensitiveDataLogging();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                options.UseLoggerFactory(LoggerFactory.Create(build =>
                {
                    build.AddConsole();  // 用于控制台程序的输出
                }));
            });

            //TODO:
            services.AddIdentity<User, IdentityRole<Guid>>()
             .AddEntityFrameworkStores<DocumentContext>()
             .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });
            services.AddSingleton(MapperConfig.GetMapperConfigs());
            services.AddDependencyInjection();


            services.AddJwtAutheticationConfiguration(settings);


            //跨域支持
            services.AddCors(options =>
            {
                options.AddPolicy("ExposeResponseHeaders",
                    builder =>
                    {
                        //至允许 WebApplicationUrl 跨域访问
                        builder.WithOrigins(pathHelper.WebApplicationUrl)
                               .WithExposedHeaders("X-Pagination")
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .WithMethods("POST", "PUT", "PATCH", "GET", "DELETE")
                               .SetIsOriginAllowed(host => true);
                    });
            });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
            //    {
            //        //builder.WithOrigins("http://jsdcms.com", "https://jsdcms.com", "https://www.jsdcms.com","https://www.jsdcms.com");

            //        builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader();

            //    });
            //});

            ////跨域支持
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(name: MyAllowSpecificOrigins,
            //        builder =>
            //        {
            //            /*
            //             Access-Control-Allow-Origin: *
            //            Access-Control-Expose-Headers: X-Log, X-Reqid
            //             */
            //            builder.AllowAnyOrigin()
            //            .WithHeaders(HeaderNames.AccessControlAllowOrigin, "*")
            //            .WithHeaders(HeaderNames.AccessControlExposeHeaders, "X-Log, X-Reqid")
            //            .AllowAnyMethod()
            //            .AllowAnyHeader();
            //        });
            //});




            //添加SignalR服务
            services.AddSignalR(opt =>
            {
                opt.EnableDetailedErrors = true;
                opt.MaximumReceiveMessageSize = 10000000000;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                });

            //SwaggerConfiguration.Configure(app, MyAllowSpecificOrigins);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Dorisoy.Pan API"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });


                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });

                //Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //SpaStartup.ConfigureServices(services);

            //services.AddSpaStaticFiles(c =>
            //{
            //    //这里设置路由
            //    c.RootPath = "ClientApp/dist";
            //});

            //services.AddSpaStaticFiles(configuration =>
            //{
            //    //这里设置路由
            //    configuration.RootPath = "ClientApp";
            //});


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }
                        app.UseDeveloperExceptionPage();
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync(exceptionHandlerFeature.Error.Message);
                    });
                });
            }


            //启用 swagger
            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(c =>
            {
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.SwaggerEndpoint($"v1/swagger.json", "Dorisoy.Pan Api v1.0");
                c.RoutePrefix = "swagger";
                c.DefaultModelsExpandDepth(-1);
            });


            app.UseStaticFiles(new StaticFileOptions()
            {
                //FileProvider = new PhysicalFileProvider(
                //Path.Combine(Directory.GetCurrentDirectory(), @"Documents")),
                //RequestPath = new PathString("/Documents")
            });



            //使用 Https 重定向
            //app.UseHttpsRedirection();


            //在这里您可以看到，我们确保它不是以/api开头的，如果它是以/api开头的，那么它将在404之内。如果找不到它，请使用NET
            //app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
            //{
            //    builder.UseRouting();
            //});

            app.UseRouting();

            app.UseResponseCompression();


            //SpaStartup.Configure(app);

            app.UseStaticFiles();


            ////使用spa 静态文件
            //app.UseSpaStaticFiles();



            //app.UseSpa(spa =>
            //{
            //    //这里是angular项目的根目录
            //    spa.Options.SourcePath = "ClientApp";
            //    //if (env.IsDevelopment())
            //    //{
            //    //    spa.UseAngularCliServer(npmScript: "start");
            //    //}
            //});


            app.UseCors("ExposeResponseHeaders");


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //Hub
                endpoints.MapHub<UserHub>("/userHub");
            });

        }

        public JwtSettings GetJwtSettings()
        {
            JwtSettings settings = new JwtSettings();

            settings.Key = Configuration["JwtSettings:key"];
            settings.Audience = Configuration["JwtSettings:audience"];
            settings.Issuer = Configuration["JwtSettings:issuer"];
            settings.MinutesToExpiration =
             Convert.ToInt32(
                Configuration["JwtSettings:minutesToExpiration"]);

            return settings;
        }
    }
}
