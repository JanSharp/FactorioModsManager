using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services.Implementations
{
    public class ConfigService : IConfigService
    {
        private const string ConfigFilePointerPath = @"FactorioModsManagerConfigFilePath.txt";
        private string configPath;
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
            configPath = argsService.GetArgs().configFilePath ??
                File.ReadAllText(ConfigFilePointerPath, Encoding.UTF8);
        }

        private void ReadConfigFile()
        {
            if (File.Exists(configPath))
            {
                // somehow manage migrating an old version of the config once there are multiple versions
                using var fileStream = File.OpenRead(configPath);
                config = (Config)serializer.Deserialize(fileStream);
                config.configPath = configPath;
                fileStream.Close();
            }
            else
            {
                // default config
                config = new Config()
                {
                    configPath = configPath,

                    ConfigVersion = 1,

                    MaintainedFactorioVersions = new List<FactorioVersion>()
                    {
                        new FactorioVersion(1, 0, 0),
                        new FactorioVersion(0, 18, 0),
                    },
                    FactorioUserName = "undefined",
                    FactorioUserToken = "undefined",
                    MaxApiRequestsPerMinute = 100,
                    ModsPath = "Mods",
                    DataPath = "Data",
                };
                WriteConfigFile();
            }
        }

        private void WriteConfigFile()
        {
            using FileStream fileStream = File.Exists(configPath)
                ? File.OpenWrite(configPath)
                : File.Create(configPath);
            serializer.Serialize(fileStream, config);
            fileStream.Close();
        }
    }
}
