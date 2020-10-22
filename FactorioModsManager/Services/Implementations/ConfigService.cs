using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services.Implementations
{
    public class ConfigService : IConfigService
    {
        private const string ConfigFilePointerFileName = @"FactorioModsManagerConfigFilePath.txt";
        private string configFileName;
        private Config config;
        private readonly XmlSerializer serializer;
        private readonly IArgsService argsService;

        public ConfigService(IArgsService argsService)
        {
            serializer = new XmlSerializer(typeof(Config));
            this.argsService = argsService;
            LoadConfigFileName();
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

        private void LoadConfigFileName()
        {
            configFileName = argsService.GetArgs().configFilePath ??
                File.ReadAllText(ConfigFilePointerFileName, Encoding.UTF8);
        }

        private void ReadConfigFile()
        {
            if (File.Exists(configFileName))
            {
                // somehow manage migrating an old version of the config once there are multiple versions
                using var fileStream = File.OpenRead(configFileName);
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
            using FileStream fileStream = File.Exists(configFileName)
                ? File.OpenWrite(configFileName)
                : File.Create(configFileName);
            serializer.Serialize(fileStream, config);
            fileStream.Close();
        }
    }
}
