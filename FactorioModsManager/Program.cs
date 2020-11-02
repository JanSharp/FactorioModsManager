using System;
using System.Threading.Tasks;
using FactorioModPortalClient;
using FactorioModsManager.Services;
using FactorioModsManager.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace FactorioModsManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IModPortalClient, ModPortalClient>(sp =>
            {
                var configService = sp.GetService<IConfigService>();
                var config = configService.GetConfig();
                return new ModPortalClient(config.FactorioUserName,
                    config.FactorioUserToken,
                    (int)config.MaxApiRequestsPerMinute);
            });

            serviceCollection.AddSingleton<IConfigService, ConfigService>();
            serviceCollection.AddSingleton<IProgramDataService, ProgramDataService>();
            serviceCollection.AddSingleton<IMapperService, MapperService>();
            serviceCollection.AddSingleton<IArgsService, ArgsService>(sp
                => ActivatorUtilities.CreateInstance<ArgsService>(sp, new[] { args }));
            serviceCollection.AddSingleton<IMainService, MainService>();
            serviceCollection.AddSingleton<IModsStorageService, ModsStorageService>();
            serviceCollection.AddSingleton<ICrashHandlerService, CrashHandlerService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var crashService = scope.ServiceProvider.GetService<ICrashHandlerService>();
                try
                {
                    var mainService = scope.ServiceProvider.GetService<IMainService>();
                    await mainService.RunAsync();
                }
                catch (Exception ex)
                {
                    var dump = crashService.CreateDump(ex);
                    crashService.WriteDump(dump);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred. This program is using exceptions for error 'handling', so read the message please and thank you.");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
