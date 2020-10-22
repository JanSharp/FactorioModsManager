using System.Collections.Generic;

namespace FactorioModsManager.Infrastructure
{
    public class Config
    {
        public int ConfigVersion { get; set; }

        public List<FactorioVersion> FactorioVersionsToMaintain { get; set; }

        public string FactorioUserName { get; set; }

        public string FactorioUserToken { get; set; }

        public uint MaxApiRequestsPerMinute { get; set; }

        public string ModsPath { get; set; }
    }
}
