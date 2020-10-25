using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
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
                using var fileStream = new FileStream(programDataFilePath, FileMode.Open, FileAccess.Read);
                using var reader = XmlDictionaryReader.CreateTextReader(fileStream, Encoding.UTF8, new XmlDictionaryReaderQuotas()
                {
                    MaxArrayLength = int.MaxValue,
                    MaxDepth = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue,
                    MaxStringContentLength = int.MaxValue,
                }, null);
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

            using var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.Preamble);
            using var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8);
            serializer.WriteObject(writer, programData);
            using var fileStream = new FileStream(GetProgramDataFilePath(dataDir), FileMode.OpenOrCreate, FileAccess.Write);
            writer.Flush();
            fileStream.Write(stream.GetBuffer(), 0, (int)stream.Length);
        }

        private string GetProgramDataFilePath(string dataDir) => Path.Combine(dataDir, "data.xml");
    }
}
