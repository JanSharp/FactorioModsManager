using System;
using System.IO;

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
            return ex.ToString();
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
