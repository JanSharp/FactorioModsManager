﻿using System;
using System.Threading.Tasks;
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

            serviceCollection.AddSingleton<IConfigService, ConfigService>();
            serviceCollection.AddSingleton<IArgsService, ArgsService>(sp
                => ActivatorUtilities.CreateInstance<ArgsService>(sp, new[] { args }));
            serviceCollection.AddSingleton<IMainService, MainService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using var scope = serviceScopeFactory.CreateScope();
            try
            {
                var configService = scope.ServiceProvider.GetService<IMainService>();
                await configService.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred. This program is using exceptions for error 'handling', so read the message please and thank you.");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
