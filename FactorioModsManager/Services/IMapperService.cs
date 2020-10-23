using FactorioModPortalClient;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services
{
    public interface IMapperService
    {
        ModData MapToModData(ResultEntry entry, ModData result);
        ModData MapToModData(ResultEntryFull entry, ModData? result = null);
        ReleaseData MapToReleaseData(Release release, ReleaseData? result = null);
    }
}