using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services.Implementations
{
    public class CrashHandlerService : ICrashHandlerService
    {
        private readonly IConfigService configService;

        public CrashHandlerService(IConfigService configService)
        {
            this.configService = configService;
        }

        public string CreateDump(Exception ex)  
        {
            string? configDump = null;
            try
            {
                var originalConfig = configService.GetConfig();
                var clonedConfig = originalConfig.Clone();
                clonedConfig.FactorioUserName = "undefined";
                clonedConfig.FactorioUserToken = "undefined";

                var serializer = new XmlSerializer(typeof(Config));
                var stream = new MemoryStream();
                serializer.Serialize(stream, clonedConfig);
                configDump = Encoding.UTF8.GetString(stream.ToArray());
            }
            catch { }
            
            if (configDump != null)
            {
                return ex.ToString() + Environment.NewLine + Environment.NewLine + configDump;
            }
            else
            {
                return ex.ToString();
            }
        }

        public void WriteDump(string dump)
        {
            Console.Error.WriteLine(dump);

            string crashDumpPath = configService.GetConfig().GetFullCrashDumpPath();
            if (!Directory.Exists(crashDumpPath))
                Directory.CreateDirectory(crashDumpPath);

            File.WriteAllText(
                Path.Combine(
                    crashDumpPath,
                    $"FactorioModsManagerCrashDump{DateTime.UtcNow:yyyyMMdd\\Thhmmssfff}.txt"
                ),
                dump);
        }
    }
}
