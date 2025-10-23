using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace Dorisoy.Pan.Data.Contexts;

//public class CaptureManagerContextFactory : IDesignTimeDbContextFactory<CaptureManagerContext>
//{
//    public CaptureManagerContext CreateDbContext(string[] args)
//    {
//        var optionsBuilder = new DbContextOptionsBuilder<CaptureManagerContext>();

//        //MySQl
//        var serverVersion = new MySqlServerVersion(new Version(8, 0, 26));
//        optionsBuilder.UseMySql("data source=localhost;Port=3306;Initial Catalog=vcms;user id=root;password=xxx", serverVersion)
//        .EnableSensitiveDataLogging();

//        //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    
//        return new CaptureManagerContext(optionsBuilder.Options);
//    }
//}

