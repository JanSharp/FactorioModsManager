using System.Collections.Generic;

namespace FactorioModsManager.Infrastructure
{
    public class MaintainedVersionConfig
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public MaintainedVersionConfig()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public MaintainedVersionConfig(FactorioVersion factorioVersion)
            : this(new List<FactorioVersion>() { factorioVersion })
        {

        }

        public MaintainedVersionConfig(List<FactorioVersion> factorioVersions)
        {
            FactorioVersions = factorioVersions;
        }

        public List<FactorioVersion> FactorioVersions { get; set; }

        public uint? MinMaintainedReleases { get; set; }

        public uint? MaxMaintainedReleases { get; set; }

        public uint? MaintainedDays { get; set; }

        public bool DeleteNoLongerMaintainedReleases { get; set; }
    }
}
