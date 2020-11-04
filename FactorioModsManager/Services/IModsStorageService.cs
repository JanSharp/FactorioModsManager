using System.Collections.Generic;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services
{
    public interface IModsStorageService
    {
        void DiscardRelease(ReleaseData release);
        void DiscardRelease(string modName, FactorioVersion version);
        void ExtractRelease(ReleaseData release, string extractModsPath);
        void GetAllCached(string modName, List<FactorioVersion> result);
        void GetAllCached(ModData mod, List<FactorioVersion> result);
        bool ReleaseIsCached(ReleaseData release);
        bool ReleaseIsStored(ReleaseData release);
        bool ReleaseIsStored(string modName, FactorioVersion version);
        void StoreRelease(ReleaseData release, byte[] bytes);
    }
}
