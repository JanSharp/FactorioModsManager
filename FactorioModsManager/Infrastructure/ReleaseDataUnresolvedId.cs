using System;
using FactorioModsManager.Infrastructure.Interfaces;

namespace FactorioModsManager.Infrastructure
{
    public class ReleaseDataUnresolvedId : IReleaseDataId, IReleaseDataUnresolvedId
    {
        public ReleaseDataUnresolvedId(string modName, FactorioVersion? version = null)
        {
            ModName = modName;
            Version = version;
        }

        public string ModName { get; }

        public bool HasFixedVersion => false;

        public FactorioVersion? Version { get; set; }

        FactorioVersion IReleaseDataId.Version { get => GetVersion(); }

        /// <summary>
        /// <para>throws if the <see cref="Version"/> is <see langword="null"/></para>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public FactorioVersion GetVersion()
        {
            return Version ?? throw new InvalidOperationException(
                $"Unable to access {nameof(IReleaseDataId)}.{nameof(Version)} of {nameof(ReleaseDataUnresolvedId)} for it is null");
        }

        public string GetFileName() => ReleaseData.GetFileName(ModName, GetVersion());
    }
}
