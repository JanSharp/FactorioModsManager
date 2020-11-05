using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FactorioModsManager.Infrastructure;
using FactorioModsManager.Infrastructure.Interfaces;
using FactorioSaveFileUtilities.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FactorioModsManager.Services.Implementations
{
    public class ExtractModsService : IExtractModsService
    {
        private readonly ExtractModsService extractModsService;
        private readonly IArgsService argsService;
        private readonly IMapperService mapperService;
        private readonly IModsStorageService modsStorageService;
        private readonly IModListService modListService;
        private readonly ISaveFileReader saveFileReader;

        [ActivatorUtilitiesConstructor]
        public ExtractModsService(
            IArgsService argsService,
            IMapperService mapperService,
            IModsStorageService modsStorageService,
            IModListService modListService,
            ISaveFileReader saveFileReader)
        {
            extractModsService = this;
            this.argsService = argsService;
            this.mapperService = mapperService;
            this.modsStorageService = modsStorageService;
            this.modListService = modListService;
            this.saveFileReader = saveFileReader;
        }

        // for unit testing
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ExtractModsService()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public ExtractModsService(
            ExtractModsService extractModsService = null,
            IArgsService argsService = null,
            IMapperService mapperService = null,
            IModsStorageService modsStorageService = null,
            IModListService modListService = null,
            ISaveFileReader saveFileReader = null)
            :
            this(argsService,
                mapperService,
                modsStorageService,
                modListService,
                saveFileReader)
        {
            this.extractModsService = extractModsService;
        }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        public async Task RunAsync()
        {
            var releases = GetReleasesToExtract(argsService.GetArgs());
        }

        public List<IReleaseDataUnresolvedId> GetReleasesToExtract(ProgramArgs programArgs)
        {
            if (programArgs.ModListPath != null)
            {
                return GetReleasesFromModList(programArgs.ModListPath);
            }

            if (programArgs.SaveFilePath != null)
            {
                return GetReleasesFromSaveFile(programArgs.SaveFilePath);
            }

            if (programArgs.ModNamesToExtract != null)
            {
                return GetReleasesFromModNamesToExtract(programArgs.ModNamesToExtract);
            }

            throw new ImpossibleException($"{nameof(ProgramArgs.ModListPath)}, {nameof(ProgramArgs.SaveFilePath)} and " +
                $"{nameof(ProgramArgs.ModNamesToExtract)} are all null when trying to get all releases to extract.");
        }

        public List<IReleaseDataUnresolvedId> GetReleasesFromModList(string modListPath)
        {
            var bytes = File.ReadAllBytes(modListPath);
            var modListJson = modListService.Deserialize(bytes);
            return modListJson.Mods
                .Where(m => m.Enabled)
                .Select(m => mapperService.MapToIReleaseDataUnresolvedId(m))
                .ToList();
        }

        public List<IReleaseDataUnresolvedId> GetReleasesFromSaveFile(string saveFilePath)
        {
            using var fileStream = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read);
            var saveFileData = saveFileReader.ReadSaveFile(fileStream);
            return saveFileData.ModsInSave
                .Select(m => (IReleaseDataUnresolvedId)mapperService.MapToReleaseDataId(m))
                .ToList();
        }

        public List<IReleaseDataUnresolvedId> GetReleasesFromModNamesToExtract(List<string> modNamesToExtract)
        {
            return modNamesToExtract
                .Select(m => (IReleaseDataUnresolvedId)new ReleaseDataUnresolvedId(m))
                .ToList();
        }
    }
}
