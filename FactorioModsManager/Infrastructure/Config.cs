using System;
using System.Collections.Generic;
using System.IO;

namespace FactorioModsManager.Infrastructure
{
    public class Config
    {
        [NonSerialized]
        public string configPath;

        public int ConfigVersion { get; set; }

        public List<FactorioVersion> FactorioVersionsToMaintain { get; set; }

        public string FactorioUserName { get; set; }

        public string FactorioUserToken { get; set; }

        public uint MaxApiRequestsPerMinute { get; set; }

        public string ModsPath { get; set; }

        public string GetFullModsPath()
        {
            if (Path.IsPathRooted(ModsPath))
                return ModsPath;
            return Path.Combine(Path.GetDirectoryName(configPath), ModsPath);
        }
    }
}
