using FactorioModsManager.Infrastructure.Interfaces;

namespace FactorioModsManager.Infrastructure
{
    public class ReleaseDataForExtracting : IReleaseDataForExtracting
    {
        public ReleaseDataForExtracting(
            string modName,
            FactorioVersion version,
            uint crc)
        {
            ModName = modName;
            Version = version;
            CRC = crc;
        }

        public string ModName { get; }

        public FactorioVersion Version { get; set; }

        public uint CRC { get; set; }

        public string GetFileName() => ReleaseData.GetFileName(ModName, Version);
    }
}
