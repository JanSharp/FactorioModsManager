using System.Collections.Generic;
using System.Text;

namespace FactorioModsManager.Infrastructure
{
    public class Config
    {
        public int ConfigVersion { get; set; }

        public List<FactorioVersion> FactorioVersionsToMaintain { get; set; }

        public string FactorioUserName { get; set; }

        public string FactorioUserToken { get; set; }

        public uint MaxApiRequestsPerMinute { get; set; }
    }
}
