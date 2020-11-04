using System.Collections.Generic;
using FactorioModsManager.Infrastructure;
using FactorioModsManager.Infrastructure.Interfaces;

namespace FactorioModsManager.Services
{
    public interface IModsStorageService
    {
        void DiscardRelease(IReleaseDataForModsStorage release);
        void DiscardRelease(string modName, FactorioVersion version);
        void ExtractRelease(IReleaseDataForModsStorage release, string extractModsPath);
        void GetAllCached(string modName, List<FactorioVersion> result);
        void GetAllCached(ModData mod, List<FactorioVersion> result);
        bool ReleaseIsCached(IReleaseDataForModsStorage release);
        bool ReleaseIsStored(IReleaseDataForModsStorage release);
        bool ReleaseIsStored(string modName, FactorioVersion version);
        void StoreRelease(IReleaseDataForModsStorage release, byte[] bytes);
    }
}
