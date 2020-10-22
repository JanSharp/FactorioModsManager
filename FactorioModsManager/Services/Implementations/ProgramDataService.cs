using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services.Implementations
{
    public class ProgramDataService : IProgramDataService
    {
        private readonly IConfigService configService;
        private readonly DataContractSerializer serializer;

        public ProgramDataService(IConfigService configService)
        {
            this.configService = configService;
            serializer = new DataContractSerializer(typeof(ProgramData), new List<Type>()
            {
                typeof(ProgramData),
                typeof(ModData),
                typeof(ReleaseData),
                typeof(ModDependency),
                typeof(ModDependencyType),
                typeof(ModDependencyOperator),
            });
        }

        public ProgramData GetProgramData()
        {
            string dataDir = configService.GetConfig().GetFullDataPath();
            string programDataFilePath = GetProgramDataFilePath(dataDir);
            if (File.Exists(programDataFilePath))
            {
                using var reader = XmlReader.Create(programDataFilePath);
                return (ProgramData)serializer.ReadObject(reader);
            }
            else
            {
                return new ProgramData(new Dictionary<string, ModData>());
            }
        }

        public void SetProgramData(ProgramData programData)
        {
            string dataDir = configService.GetConfig().GetFullDataPath();
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            using var writer = XmlWriter.Create(GetProgramDataFilePath(dataDir));
            serializer.WriteObject(writer, programData);
        }

        private string GetProgramDataFilePath(string dataDir) => Path.Combine(dataDir, "data.xml");
    }
}
