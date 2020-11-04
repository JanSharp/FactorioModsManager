using System;
using System.Threading.Tasks;
using FactorioModPortalClient;
using FactorioModsManager.Infrastructure;
using FactorioModsManager.Services;
using FactorioModsManager.Services.Implementations;
using FactorioSaveFileUtilities.Services;
using FactorioSaveFileUtilities.Services.Implementations;
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
            serviceCollection.AddSingleton<ISyncModsWithPortalService, SyncModsWithPortalService>(sp
                => ActivatorUtilities.CreateInstance<SyncModsWithPortalService>(sp));
            serviceCollection.AddSingleton<IModsStorageService, ModsStorageService>();
            serviceCollection.AddSingleton<ICrashHandlerService, CrashHandlerService>();
            serviceCollection.AddSingleton<ISaveFileReader, SaveFileReader>();
            serviceCollection.AddSingleton<IExtractModsService, ExtractModsService>(sp
                => ActivatorUtilities.CreateInstance<ExtractModsService>(sp));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var crashService = scope.ServiceProvider.GetService<ICrashHandlerService>();
                try
                {
                    var argsService = scope.ServiceProvider.GetService<IArgsService>();

                    switch (argsService.GetExecutionType(argsService.GetArgs()))
                    {
                        case ExecutionType.CreateConfig:
                            // the constructor currently creates the config file
                            scope.ServiceProvider.GetService<IConfigService>();
                            break;

                        case ExecutionType.Sync:
                            var syncService = scope.ServiceProvider.GetService<ISyncModsWithPortalService>();
                            await syncService.RunAsync();
                            break;

                        case ExecutionType.ExtractMods:
                            var extractModsServcie = scope.ServiceProvider.GetService<IExtractModsService>();
                            await extractModsServcie.RunAsync();
                            break;
                    }
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
