using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FactorioModsManager.Infrastructure
{
    public class Config
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Config()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public Config(string? configPath, List<MaintainedVersionConfig> maintainedFactorioVersions, string factorioUserName, string factorioUserToken, uint maxApiRequestsPerMinute, string modsPath, string dataPath)
        {
            this.configPath = configPath;
            MaintainedFactorioVersions = maintainedFactorioVersions;
            FactorioUserName = factorioUserName;
            FactorioUserToken = factorioUserToken;
            MaxApiRequestsPerMinute = maxApiRequestsPerMinute;
            ModsPath = modsPath;
            DataPath = dataPath;
        }

        [XmlIgnore]
        public string? configPath;

        public List<MaintainedVersionConfig> MaintainedFactorioVersions { get; set; }

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
            string? dirPath = Path.GetDirectoryName(configPath);
            if (dirPath == null)
                return path;
            return Path.Combine(dirPath, path);
        }

    }
}
