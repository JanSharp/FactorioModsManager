using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services.Implementations
{
    public class ConfigService : IConfigService
    {
        private const string ConfigFileName = @"FactorioModsManagerConfig.xml";
        private Config config;
        private readonly XmlSerializer serializer;

        public ConfigService()
        {
            serializer = new XmlSerializer(typeof(Config));
            ReadConfigFile();
        }

        public Config GetConfig()
        {
            return config;
        }

        public void SetConfig(Config config)
        {
            this.config = config;
            WriteConfigFile();
        }

        private void ReadConfigFile()
        {
            if (File.Exists(ConfigFileName))
            {
                // somehow manage migrating an old version of the config once there are multiple versions
                using var fileStream = File.OpenRead(ConfigFileName);
                config = (Config)serializer.Deserialize(fileStream);
                fileStream.Close();
            }
            else
            {
                // default config
                config = new Config()
                {
                    ConfigVersion = 1,

                    FactorioVersionsToMaintain = new List<FactorioVersion>()
                    {
                        new FactorioVersion(1, 0, 0),
                    },
                    FactorioUserName = "undefined",
                    FactorioUserToken = "undefined",
                    MaxApiRequestsPerMinute = 100,
                };
                WriteConfigFile();
            }
        }

        private void WriteConfigFile()
        {
            using FileStream fileStream = File.Exists(ConfigFileName)
                ? File.OpenWrite(ConfigFileName)
                : File.Create(ConfigFileName);
            serializer.Serialize(fileStream, config);
            fileStream.Close();
        }
    }
}
