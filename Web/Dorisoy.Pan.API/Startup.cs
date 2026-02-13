using Dorisoy.Pan.API.Helpers;
using Dorisoy.Pan.API.Helpers.Mapping;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
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
using Microsoft.OpenApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
                //MySQL
                options.UseMySQL(Configuration.GetConnectionString("DocumentDbConnectionString"))
                .EnableSensitiveDataLogging();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                options.UseLoggerFactory(LoggerFactory.Create(build =>
                {
                    build.AddConsole();
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


            //����֧��
            services.AddCors(options =>
            {
                options.AddPolicy("ExposeResponseHeaders",
                    builder =>
                    {
                        //������ WebApplicationUrl �������
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

            ////����֧��
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




            //����SignalR����
            services.AddSignalR(opt =>
            {
                opt.EnableDetailedErrors = true;
                opt.MaximumReceiveMessageSize = 10000000000;
            });
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                services.Configure<IISServerOptions>(options =>
                {
                    options.AutomaticAuthentication = false;
                });
            }
           
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


                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
                });

                //Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //SpaStartup.ConfigureServices(services);

            //services.AddSpaStaticFiles(c =>
            //{
            //    //��������·��
            //    c.RootPath = "ClientApp/dist";
            //});

            //services.AddSpaStaticFiles(configuration =>
            //{
            //    //��������·��
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


            //���� swagger
            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseSwagger();

            // 自动补齐数据库缺失列（代码实体新增字段但数据库未同步）
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DocumentContext>();
                try
                {
                    db.Database.ExecuteSqlRaw(@"
                        SET @dbname = DATABASE();
                        SET @tablename = 'documents';
                        SET @columnname = 'Md5';
                        SET @preparedStatement = (SELECT IF(
                            (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                             WHERE TABLE_SCHEMA = @dbname AND TABLE_NAME = @tablename AND COLUMN_NAME = @columnname) = 0,
                            CONCAT('ALTER TABLE `', @tablename, '` ADD COLUMN `', @columnname, '` VARCHAR(255) NULL;'),
                            'SELECT 1;'
                        ));
                        PREPARE stmt FROM @preparedStatement;
                        EXECUTE stmt;
                        DEALLOCATE PREPARE stmt;
                    ");
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                    logger.LogWarning(ex, "Auto-migration: failed to add missing columns, may already exist.");
                }
            }

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



            //ʹ�� Https �ض���
            //app.UseHttpsRedirection();


            //�����������Կ���������ȷ����������/api��ͷ�ģ����������/api��ͷ�ģ���ô������404֮�ڡ�����Ҳ���������ʹ��NET
            //app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
            //{
            //    builder.UseRouting();
            //});

            app.UseRouting();

            app.UseResponseCompression();


            //SpaStartup.Configure(app);

            app.UseStaticFiles();


            ////ʹ��spa ��̬�ļ�
            //app.UseSpaStaticFiles();



            //app.UseSpa(spa =>
            //{
            //    //������angular��Ŀ�ĸ�Ŀ¼
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
