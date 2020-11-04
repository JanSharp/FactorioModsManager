using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FactorioModsManager.Services.Implementations
{
    public class ExtractModsService : IExtractModsService
    {
        private readonly ExtractModsService extractModsService;
        private readonly IArgsService argsService;

        [ActivatorUtilitiesConstructor]
        public ExtractModsService(
            IArgsService argsService)
        {
            extractModsService = this;
            this.argsService = argsService;
        }

        // for unit testing
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ExtractModsService()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public ExtractModsService(
            ExtractModsService extractModsService = null,
            IArgsService argsService = null)
        {
            this.extractModsService = extractModsService;
            this.argsService = argsService;
        }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        public async Task RunAsync()
        {

        }
    }
}
