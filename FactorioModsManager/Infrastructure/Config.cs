using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FactorioModsManager.Infrastructure
{
    public class Config
    {
        [XmlIgnore]
        public string configPath;

        public int ConfigVersion { get; set; }

        public List<FactorioVersion> FactorioVersionsToMaintain { get; set; }

        public string FactorioUserName { get; set; }

        public string FactorioUserToken { get; set; }

        public uint MaxApiRequestsPerMinute { get; set; }

        public string ModsPath { get; set; }
        public string GetFullModsPath() => GetFullPath(ModsPath);

        public string DataPath { get; set; }
        public string GetFullDataPath() => GetFullPath(DataPath);

        private string GetFullPath(string path)
        {
            if (Path.IsPathRooted(path))
                return path;
            return Path.Combine(Path.GetDirectoryName(configPath), path);
        }

    }
}
