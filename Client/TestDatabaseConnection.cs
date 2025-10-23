using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Dorisoy.Pan.Data.Contexts;

class TestDatabaseConnection
{
    static void Main(string[] args)
    {
        try
        {
            // 测试数据库连接
            var connectionString = "server=127.0.0.1;port=3306;database=vcms;user=root;password=racing.1";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 26));
            
            var optionsBuilder = new DbContextOptionsBuilder<CaptureManagerContext>();
            optionsBuilder
                .UseMySql(connectionString, serverVersion)
                .EnableSensitiveDataLogging();
                
            using (var context = new CaptureManagerContext(optionsBuilder.Options))
            {
                Console.WriteLine("尝试连接到数据库...");
                var canConnect = context.Database.CanConnect();
                Console.WriteLine($"数据库连接结果: {canConnect}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"数据库连接失败: {ex.Message}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
        }
    }
}