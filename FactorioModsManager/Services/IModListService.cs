using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services
{
    public interface IModListService
    {
        ModListJson Deserialize(byte[] bytes);
    }
}
