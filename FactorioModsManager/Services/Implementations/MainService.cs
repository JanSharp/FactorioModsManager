using System;
using System.Threading.Tasks;

namespace FactorioModsManager.Services.Implementations
{
    public class MainService : IMainService
    {
        private readonly IConfigService configService;

        public MainService(IConfigService configService)
        {
            this.configService = configService;
        }

        public Task Run()
        {
            Console.WriteLine("Hello World!");
            return Task.CompletedTask;
        }
    }
}
