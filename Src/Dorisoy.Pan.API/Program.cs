using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NLog;
namespace Dorisoy.Pan.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
            var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
            try
            {


                logger.Debug("Starting host...");


                Console.WriteLine("");
                Console.WriteLine(" Starting host...");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                Console.WriteLine(exception);

                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        /// <summary>
        /// CreateHostBuilder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder =Host.CreateDefaultBuilder(args);
            
            
            return builder
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //����Kestrel
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.AllowSynchronousIO= true;
                        options.Limits.MaxConcurrentConnections = 100;
                        options.Limits.MaxConcurrentUpgradedConnections = 100;
                        options.Limits.MinRequestBodyDataRate =
                            new MinDataRate(bytesPerSecond: 100,
                                gracePeriod: TimeSpan.FromSeconds(10));

                        options.Limits.MinResponseDataRate =
                            new MinDataRate(bytesPerSecond: 100,
                                gracePeriod: TimeSpan.FromSeconds(10));
                        
                        options.Limits.KeepAliveTimeout =
                            TimeSpan.FromMinutes(2);

                        options.Limits.RequestHeadersTimeout =
                            TimeSpan.FromMinutes(1);
                    });

                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    //�Ƴ�Ĭ��Provider
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
        }
            
    }
}
