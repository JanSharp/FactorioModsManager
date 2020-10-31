using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FactorioModPortalClient;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services.Implementations
{
    public class MainService : IMainService
    {
        private readonly IArgsService argsService;
        private readonly IConfigService configService;
        private readonly IProgramDataService programDataService;
        private readonly IMapperService mapperService;
        private readonly IModPortalClient client;

        public MainService(
            IArgsService argsService,
            IConfigService configService,
            IProgramDataService programDataService,
            IMapperService mapperService,
            IModPortalClient client)
        {
            this.argsService = argsService;
            this.configService = configService;
            this.programDataService = programDataService;
            this.mapperService = mapperService;
            this.client = client;
        }

        /* concept:
         * 
         * foreach mod on the portal, check if we have an entry for said mod alraedy
         * if we do, compare latest release release date
         * if it differs, get ResultEntryFull from the protal
         * for every release with a maintained factorio version,
         * check if we already have this version downloaded
         * if we don't download it
         * 
         * if we don't have an entry already, create an entry for the mod
         * and then do the same download logic as above
         * this includes checking for existing ones, so mods that were manually
         * copied into the mods folder would be identified
         * 
         */

        public async Task RunAsync()
        {
            if (argsService.GetArgs().CreateConfig)
                return;

            Console.WriteLine("Syncronizing with mod portal...");

            ProgramData programData = programDataService.GetProgramData();
            HashSet<string> modNamesToDelete = programData.Mods.Values.Select(m => m.Name).ToHashSet();

            Console.WriteLine($"Loaded {programData.Mods.Count} mods from data file.");

            DateTime lastSaveTime = DateTime.Now;
            int attemptedSaveCount = 0;
            void SaveChanges(bool bypassConditions = false)
            {
                // only save every 5 minutes, but also only if there have been 32 attempts
                // to save, which indicates that something changed 32 times
                ++attemptedSaveCount;
                if (bypassConditions
                    || (
                        attemptedSaveCount >= 32
                        && (DateTime.Now - lastSaveTime).Minutes >= 5
                    ))
                {
                    Console.WriteLine($"Saving {attemptedSaveCount} changes.");
                    attemptedSaveCount = 0;
                    programDataService.SetProgramData(programData);
                    lastSaveTime = DateTime.Now;
                }
            }

            // create and update
            await foreach (var entry in client.EnumerateAsync())
            {
                if (programData.Mods.TryGetValue(entry.Name, out var modData))
                {
                    modNamesToDelete.Remove(entry.Name);

                    if ((entry.LatestRelease?.ReleasedAt ?? null) != (modData.LatestRelease?.ReleasedAt ?? null))
                    {
                        SyncMod(entry, await client.GetResultEntryFullAsync(entry.Name), modData);
                        Console.WriteLine($"Changed {entry.Name}.");
                        SaveChanges();
                    }
                    else
                    {
                        SyncModPartial(entry, modData);
                    }
                }
                else
                {
                    var entryFull = await client.GetResultEntryFullAsync(entry.Name);
                    modData = mapperService.MapToModData(entryFull);
                    SyncMod(entry, entryFull, modData);
                    programData.Mods.Add(modData.Name, modData);
                    Console.WriteLine($"Added   {entry.Name}.");
                    SaveChanges();
                }
            }

            // delete mods
            if (modNamesToDelete.Count > 0)
            {
                foreach (var modNameToDelete in modNamesToDelete)
                {
                    programData.Mods.Remove(modNameToDelete);
                    Console.WriteLine($"Removed {modNameToDelete}.");
                }
                SaveChanges();
            }

            if (attemptedSaveCount > 0)
            {
                --attemptedSaveCount;
                SaveChanges(bypassConditions: true);
            }

            Console.WriteLine("Determining which releases should be maintained. Downloading and deleting accordingly.");

            string modsPath = configService.GetConfig().GetFullModsPath();
            if (!Directory.Exists(modsPath))
                Directory.CreateDirectory(modsPath);

            var maintainedVersions = configService.GetConfig().MaintainedFactorioVersions
                .ToDictionary(v => v.FactorioVersion);

            var maintainedReleaseFileNames = Directory.EnumerateFiles(configService.GetConfig().GetFullModsPath())
                .Select(f => Path.GetFileName(f))
                .ToHashSet();

            foreach (var mod in programData.Mods.Values)
                await SyncMaintainedReleasesAsync(mod, maintainedVersions, maintainedReleaseFileNames);
        }

        public void SyncModPartial(ResultEntry portalEntryFull, ModData modData)
        {
            mapperService.MapToModData(portalEntryFull, modData);
        }

        public void SyncMod(ResultEntry portalEntry, ResultEntryFull portalEntryFull, ModData modData)
        {
            mapperService.MapToModData(portalEntryFull, modData);

            var existingReleasesMap = modData.GroupedReleases
                .SelectMany(g => g.Value)
                .ToDictionary(r => r.Version.ToString());

            modData.GroupedReleases = new Dictionary<FactorioVersion, List<ReleaseData>>();

            foreach (var portalRelease in portalEntryFull.Releases.OrderByDescending(r => r.ReleasedAt))
            {
                ReleaseData releaseData;
                if (existingReleasesMap.TryGetValue(portalRelease.Version, out var release)
                    && release.ReleasedAt == portalRelease.ReleasedAt)
                {
                    releaseData = release;
                }
                else
                {
                    releaseData = mapperService.MapToReleaseData(modData, portalRelease);
                    if (portalRelease.InfoJson.Dependencies != null)
                    {
                        foreach (var dependency in portalRelease.InfoJson.Dependencies.dependencies)
                        {
                            releaseData.Dependencies.Add(new ModDependency(releaseData, dependency));
                        }
                    }
                }

                if (!modData.GroupedReleases.TryGetValue(releaseData.FactorioVersion, out var releases))
                {
                    releases = new List<ReleaseData>();
                    modData.GroupedReleases.Add(releaseData.FactorioVersion, releases);
                }
                releases.Add(releaseData);
                if (releaseData.ReleasedAt == portalEntry.LatestRelease.ReleasedAt)
                    modData.LatestRelease = releaseData;
            }
        }

        public async Task SyncMaintainedReleasesAsync(
            ModData mod,
            Dictionary<FactorioVersion, MaintainedVersionConfig> maintainedVersions,
            HashSet<string> maintainedReleaseFileNames)
        {
            foreach (var versionGroup in mod.GroupedReleases)
            {
                if (maintainedVersions.TryGetValue(versionGroup.Key, out var maintainedVersion))
                {
                    int count = 0;
                    bool noMoreMaintainedReleases = false;
                    bool shouldDelete = maintainedVersion.DeleteNoLongerMaintainedReleases;

                    foreach (var release in versionGroup.Value) // releases are stored ordered by Version descending
                    {
                        if (noMoreMaintainedReleases)
                        {
                            EnsureReleaseIsNotMaintained(release, shouldDelete, maintainedReleaseFileNames);
                            continue;
                        }

                        ++count;

                        if (maintainedVersion.MinMaintainedReleases.HasValue
                            && count <= maintainedVersion.MinMaintainedReleases.Value)
                        {
                            await EnsureReleaseIsMaintainedAsync(release, maintainedReleaseFileNames);
                            continue;
                        }

                        if (maintainedVersion.MaxMaintainedReleases.HasValue
                            && count > maintainedVersion.MaxMaintainedReleases.Value)
                        {
                            EnsureReleaseIsNotMaintained(release, shouldDelete, maintainedReleaseFileNames);
                            noMoreMaintainedReleases = true;
                            continue;
                        }

                        if (maintainedVersion.MaintainedDays.HasValue
                            && (DateTime.Now - release.ReleasedAt).Days > (int)maintainedVersion.MaintainedDays.Value)
                        {
                            EnsureReleaseIsNotMaintained(release, shouldDelete, maintainedReleaseFileNames);
                            noMoreMaintainedReleases = true;
                            continue;
                        }

                        await EnsureReleaseIsMaintainedAsync(release, maintainedReleaseFileNames);
                    }
                }
            }
        }

        public async Task EnsureReleaseIsMaintainedAsync(
            ReleaseData release,
            HashSet<string> maintainedReleaseFileNames)
        {
            string fileName = release.GetFileName();
            if (maintainedReleaseFileNames.Contains(fileName))
                return;

            string modsPath = configService.GetConfig().GetFullModsPath();
            string path = Path.Combine(modsPath, fileName);
            if (!File.Exists(path) && false)
            {
                Console.WriteLine($"Downloading {fileName}.");
                var bytes = await DownloadReleaseAsync(release);
                File.WriteAllBytes(path, bytes);
            }
            maintainedReleaseFileNames.Add(fileName);
        }

        public void EnsureReleaseIsNotMaintained(
            ReleaseData release,
            bool shouldDelete,
            HashSet<string> maintainedReleaseFileNames)
        {
            string fileName = release.GetFileName();
            if (!maintainedReleaseFileNames.Contains(fileName))
                return;

            if (shouldDelete)
            {
                string modsPath = configService.GetConfig().GetFullModsPath();
                string path = Path.Combine(modsPath, fileName);
                if (File.Exists(path))
                {
                    Console.WriteLine($"Deleting    {fileName}.");
                    File.Delete(path);
                }
            }
            maintainedReleaseFileNames.Remove(fileName);
        }

        public Task<byte[]> DownloadReleaseAsync(ReleaseData release)
        {
            return client.DownloadModAsByteArrayAsync(new Release()
            {
                DownloadUrl = release.DownloadUrl,
                Sha1 = release.Sha1,
            });
        }
    }
}
