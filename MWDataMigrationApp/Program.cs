using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MWDataMigrationApp;
using MWDataMigrationApp.Data;

class Program
{
    static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var migrationService = host.Services.GetRequiredService<DataMigrationService>();
        migrationService.MigrateData().Wait();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var sourceConnectionString = context.Configuration.GetConnectionString("SourceConnection");
                var targetConnectionString = context.Configuration.GetConnectionString("TargetConnection");

                services.AddDbContext<SourceContext>(options =>
                    options.UseSqlServer(sourceConnectionString));
                services.AddDbContext<TargetContext>(options =>
                    options.UseSqlServer(targetConnectionString));
                services.AddTransient<DataMigrationService>();
            });
}