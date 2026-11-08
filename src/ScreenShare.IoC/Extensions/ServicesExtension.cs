using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScreenShare.Application;
using ScreenShare.Domain;
using StackExchange.Redis;

namespace ScreenShare.IoC.Extensions;

public static class ServicesExtension
{
    extension(IServiceCollection collection)
    {
        public void ConfigureServices()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            collection.AddSingleton(configuration);


            collection.ConfigureRedis(configuration);
        }

        private void ConfigureRedis(IConfigurationRoot configuration)
        {
            collection.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var options = ConfigurationOptions.Parse(
                    configuration["Redis:ConnectionString"]!
                );
                options.AbortOnConnectFail = false;

                return ConnectionMultiplexer.Connect(options);
            });
            collection.AddScoped<IRedisService, RedisService>();
        }
    }
}