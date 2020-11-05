using System;
using FactorioModsManager.Infrastructure.Interfaces;

namespace FactorioModsManager.Infrastructure
{
    public struct ReleaseDataId : IReleaseDataId, IReleaseDataUnresolvedId
    {
        public ReleaseDataId(string modName, FactorioVersion version)
        {
            ModName = modName;
            Version = version;
        }

        public string ModName { get; }

        public bool HasFixedVersion => true;

        public FactorioVersion Version { get; }

        FactorioVersion? IReleaseDataUnresolvedId.Version
        {
            get => Version;
            set => throw new InvalidOperationException($"Unable to set the {nameof(IReleaseDataUnresolvedId)}." +
                $"{nameof(Version)} of {nameof(ReleaseDataId)} for it is immutable.");
        }

        public FactorioVersion GetVersion() => Version;

        public string GetFileName() => ReleaseData.GetFileName(ModName, Version);
    }
}
