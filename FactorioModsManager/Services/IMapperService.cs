using FactorioModPortalClient;
using FactorioModsManager.Infrastructure;
using FactorioModsManager.Infrastructure.Interfaces;
using FactorioSaveFileUtilities.Infrastructure;

namespace FactorioModsManager.Services
{
    public interface IMapperService
    {
        ModData MapToModData(ResultEntry entry, ModData result);
        ModData MapToModData(ResultEntryFull entry, ModData? result = null);
        ReleaseData MapToReleaseData(ModData mod, Release release, ReleaseData? result = null);
        ReleaseDataId MapToReleaseDataId(ModInSaveData modInSaveData);
        IReleaseDataUnresolvedId MapToIReleaseDataUnresolvedId(ModListJsonItem modListJsonItem);
    }
}
