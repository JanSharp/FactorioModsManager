using System.Collections.Generic;
using FactorioModsManager.Infrastructure;
using FactorioModsManager.Infrastructure.Interfaces;

namespace FactorioModsManager.Services
{
    public interface IModsStorageService
    {
        void DiscardRelease(IReleaseDataId release);
        void ExtractRelease(IReleaseDataId release, string extractModsPath);
        void GetAllCached(string modName, List<FactorioVersion> result);
        bool ReleaseIsCached(IReleaseDataId release);
        bool ReleaseIsStored(IReleaseDataId release);
        void StoreRelease(IReleaseDataId release, byte[] bytes);
    }
}
