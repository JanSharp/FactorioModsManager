using FactorioModPortalClient;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services
{
    public interface IMapperService
    {
        ModData MapToModData(ResultEntryFull entry, ModData? result);
    }
}