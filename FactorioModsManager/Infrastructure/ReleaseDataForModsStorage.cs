using FactorioModsManager.Infrastructure.Interfaces;

namespace FactorioModsManager.Infrastructure
{
    public struct ReleaseDataForModsStorage : IReleaseDataForModsStorage
    {
        public ReleaseDataForModsStorage(string modName, FactorioVersion version)
        {
            ModName = modName;
            Version = version;
        }

        public string ModName { get; }

        public FactorioVersion Version { get; }

        public string GetFileName() => ReleaseData.GetFileName(ModName, Version);
    }
}
