using System.Text.Json;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services.Implementations
{
    public class ModListService : IModListService
    {
        public ModListJson Deserialize(byte[] bytes)
        {
            return JsonSerializer.Deserialize<ModListJson>(bytes);
        }
    }
}
