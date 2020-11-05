using FactorioModsManager.Infrastructure.Interfaces;

namespace FactorioModsManager.Infrastructure
{
    public struct ReleaseDataId : IReleaseDataId
    {
        public ReleaseDataId(string modName, FactorioVersion version)
        {
            ModName = modName;
            Version = version;
        }

        public string ModName { get; }

        public FactorioVersion Version { get; }

        public string GetFileName() => ReleaseData.GetFileName(ModName, Version);
    }
}
