using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
                using var deflateStream = new DeflateStream(fileStream, CompressionMode.Decompress);
                using var reader = XmlDictionaryReader.CreateTextReader(deflateStream, Encoding.UTF8, new XmlDictionaryReaderQuotas()
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

            using var memoryStream = new MemoryStream();
            using var deflateStream = new DeflateStream(memoryStream, CompressionLevel.Optimal, leaveOpen: true);
            deflateStream.Write(Encoding.UTF8.Preamble);
            using var writer = XmlDictionaryWriter.CreateTextWriter(deflateStream, Encoding.UTF8, ownsStream: false);
            serializer.WriteObject(writer, programData);
            writer.Close();
            deflateStream.Close();
            memoryStream.Position = 0;

            using var fileStream = new FileStream(GetProgramDataFilePath(dataDir), FileMode.OpenOrCreate, FileAccess.Write);
            memoryStream.CopyTo(fileStream);
            fileStream.Close();
        }

        private string GetProgramDataFilePath(string dataDir) => Path.Combine(dataDir, "data.dat");
    }
}
