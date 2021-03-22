namespace DownloadManagerCliApp.Dependency_Injection
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using System.IO;
    internal static class Dependency
    {
        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", true, true)
                                    .Build();

            services.AddApplicationServices(configuration);

            Log.Logger = new LoggerConfiguration()
                             .ReadFrom.Configuration(configuration)
                             .CreateLogger();

            return services;
        }

    }
}
