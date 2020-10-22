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

        // disable warnings because they are being initialized in the functions in the constructor
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ConfigService(IArgsService argsService)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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
            configPath = argsService.GetArgs().ConfigFilePath ??
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
                config = new Config(
                    configPath: configPath,
                    
                    configVersion: 1,
                    
                    maintainedFactorioVersions: new List<FactorioVersion>()
                    {
                        new FactorioVersion(1, 0, 0),
                        new FactorioVersion(0, 18, 0),
                    },
                    factorioUserName: "undefined",
                    factorioUserToken: "undefined",
                    maxApiRequestsPerMinute: 100,
                    modsPath: "Mods",
                    dataPath: "Data"
                    );

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
