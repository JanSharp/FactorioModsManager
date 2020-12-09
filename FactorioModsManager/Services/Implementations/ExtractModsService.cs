using System;
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
        private readonly IProgramDataService programDataService;
        private readonly IMapperService mapperService;
        private readonly IModsStorageService modsStorageService;
        private readonly IModListService modListService;
        private readonly ISaveFileReader saveFileReader;

        [ActivatorUtilitiesConstructor]
        public ExtractModsService(
            IArgsService argsService,
            IProgramDataService programDataService,
            IMapperService mapperService,
            IModsStorageService modsStorageService,
            IModListService modListService,
            ISaveFileReader saveFileReader)
        {
            extractModsService = this;
            this.argsService = argsService;
            this.programDataService = programDataService;
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
            IProgramDataService programDataService = null,
            IMapperService mapperService = null,
            IModsStorageService modsStorageService = null,
            IModListService modListService = null,
            ISaveFileReader saveFileReader = null)
            :
            this(argsService,
                programDataService,
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
            var programArgs = argsService.GetArgs();
            var releases = GetReleasesToExtract(programArgs);

            foreach (var release in releases)
            {
                if (modsStorageService.ReleaseIsStored(release))
                    modsStorageService.ExtractRelease(release, programArgs.ExtractModsPath!);
            }
        }

        public List<IReleaseDataUnresolvedId> GetReleasesToExtract(ProgramArgs programArgs)
        {
            if (programArgs.ModListPath != null)
            {
                var releases = GetReleasesFromModList(programArgs.ModListPath);
                ResolveReleasesAndDependencies(releases, programArgs);
                return releases;
            }

            if (programArgs.SaveFilePath != null)
            {
                return GetReleasesFromSaveFile(programArgs.SaveFilePath);
            }

            if (programArgs.ModNamesToExtract != null)
            {
                var releases = GetReleasesFromModNamesToExtract(programArgs.ModNamesToExtract);
                ResolveReleasesAndDependencies(releases, programArgs);
                return releases;
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

        private Exception GetUnknownModException(string modName)
            => new Exception($"Unknown mod {modName}.");

        public List<IReleaseDataUnresolvedId> ResolveReleasesAndDependencies(
            List<IReleaseDataUnresolvedId> releases,
            ProgramArgs programArgs)
        {
            ProgramData? programData = null;
            ProgramData GetProgramData()
            {
                if (programData == null)
                    programData = programDataService.GetProgramData();
                return programData;
            }

            var releasesToRemove = new List<IReleaseDataUnresolvedId>();

            foreach (var release in releases)
            {
                if (GetProgramData().Mods.TryGetValue(release.ModName, out var modData))
                {
                    if (release.Version == null)
                    {
                        release.Version = modData.LatestRelease?.Version
                        ?? throw new Exception($"Mod {release.ModName} does not have any releases.");
                    }
                }
                else
                {
                    releasesToRemove.Add(release);
                    Console.WriteLine($"Unknown mod {release.ModName}.");
                }
            }

            foreach (var release in releasesToRemove)
            {
                releases.Remove(release);
            }

            return releases;
        }

        private class ResolveData
        {
            public string ModName { get; set; }

            public ReleaseData CurrentRelease { get; set; }

            public List<ModDependency> DependenciesTargetingThisMod { get; set; }
        }
    }
}
