using System;
using System.Collections.Generic;
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
        private readonly IModsStorageService modsStorageService;

        public MainService(
            IArgsService argsService,
            IConfigService configService,
            IProgramDataService programDataService,
            IMapperService mapperService,
            IModPortalClient client,
            IModsStorageService modsStorageService)
        {
            this.argsService = argsService;
            this.configService = configService;
            this.programDataService = programDataService;
            this.mapperService = mapperService;
            this.client = client;
            this.modsStorageService = modsStorageService;
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

            var maintainedVersions = configService.GetConfig().MaintainedFactorioVersions;

            List<FactorioVersion> cachedReleases = new List<FactorioVersion>();
            foreach (var mod in programData.Mods.Values)
                await SyncMaintainedReleasesAsync(mod, maintainedVersions, cachedReleases);
        }

        public void SyncModPartial(ResultEntry portalEntryFull, ModData modData)
        {
            mapperService.MapToModData(portalEntryFull, modData);
        }

        public void SyncMod(ResultEntry portalEntry, ResultEntryFull portalEntryFull, ModData modData)
        {
            mapperService.MapToModData(portalEntryFull, modData);

            var duplicateVersions = new HashSet<string>();
            var existingReleasesMap = new Dictionary<string, ReleaseData>();
            foreach (var release in modData.GroupedReleases
                .SelectMany(g => g.Value))
            {
                string version = release.Version.ToString();
                if (existingReleasesMap.ContainsKey(version))
                {
                    existingReleasesMap.Remove(version);
                    duplicateVersions.Add(version);
                }
                else if (!duplicateVersions.Contains(version))
                {
                    existingReleasesMap.Add(version, release);
                }
            }

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

            if (modData.GroupedReleases.Count == 0)
                modData.LatestRelease = null;
        }

        public async Task SyncMaintainedReleasesAsync(
            ModData mod,
            List<MaintainedVersionConfig> maintainedVersions,
            List<FactorioVersion> cachedReleases)
        {
            bool shouldDelete;

            modsStorageService.GetAllCached(mod, cachedReleases);

            foreach (var maintainedVersion in maintainedVersions)
            {
                int count = 0;
                bool noMoreMaintainedReleases = false;
                shouldDelete = maintainedVersion.DeleteNoLongerMaintainedReleases;

                foreach (var maintainedFactorioVersion in maintainedVersion.FactorioVersions)
                {
                    if (mod.GroupedReleases.TryGetValue(maintainedFactorioVersion, out var releases))
                    {
                        foreach (var release in releases) // releases are stored ordered by Version descending
                        {
                            cachedReleases.Remove(release.Version);

                            if (noMoreMaintainedReleases)
                            {
                                EnsureReleaseIsNotMaintained(release, shouldDelete);
                                continue;
                            }

                            ++count;

                            if (maintainedVersion.MinMaintainedReleases.HasValue
                                && count <= maintainedVersion.MinMaintainedReleases.Value)
                            {
                                await EnsureReleaseIsMaintainedAsync(release);
                                continue;
                            }

                            if (maintainedVersion.MaxMaintainedReleases.HasValue
                                && count > maintainedVersion.MaxMaintainedReleases.Value)
                            {
                                EnsureReleaseIsNotMaintained(release, shouldDelete);
                                noMoreMaintainedReleases = true;
                                continue;
                            }

                            if (maintainedVersion.MaintainedDays.HasValue
                                && (DateTime.Now - release.ReleasedAt).Days > (int)maintainedVersion.MaintainedDays.Value)
                            {
                                EnsureReleaseIsNotMaintained(release, shouldDelete);
                                noMoreMaintainedReleases = true;
                                continue;
                            }

                            await EnsureReleaseIsMaintainedAsync(release);
                        }
                    }
                }
            }

            shouldDelete = configService.GetConfig().DeleteOldReleases;
            foreach (var deletedVersion in cachedReleases)
            {
                UnmaintainRelease(mod.Name, deletedVersion, shouldDelete);
            }
        }

        public async Task EnsureReleaseIsMaintainedAsync(ReleaseData release)
        {
            if (!modsStorageService.ReleaseIsCached(release))
                await MaintainReleaseAsync(release);
        }

        public async Task MaintainReleaseAsync(ReleaseData release)
        {
            if (!modsStorageService.ReleaseIsStored(release))
            {
                Console.WriteLine($"Downloading {release.GetFileName()}.");
                var bytes = await DownloadReleaseAsync(release);
                modsStorageService.StoreRelease(release, bytes);
            }
        }

        public Task<byte[]> DownloadReleaseAsync(ReleaseData release)
        {
            try
            {
                return client.DownloadModAsByteArrayAsync(new Release()
                {
                    DownloadUrl = release.DownloadUrl,
                    Sha1 = release.Sha1,
                });
            }
            catch (NotImplementedException ex)
            {
                throw new Exception("Invalid username and user token?", ex);
            }
        }

        public void EnsureReleaseIsNotMaintained(ReleaseData release, bool shouldDelete)
        {
            if (modsStorageService.ReleaseIsCached(release))
                UnmaintainRelease(release, shouldDelete);
        }

        public void UnmaintainRelease(ReleaseData release, bool shouldDelete)
        {
            UnmaintainRelease(release.Mod.Name, release.Version, shouldDelete);
        }

        public void UnmaintainRelease(string modName, FactorioVersion version, bool shouldDelete)
        {
            if (shouldDelete && modsStorageService.ReleaseIsStored(modName, version))
            {
                Console.WriteLine($"Deleting    {ReleaseData.GetFileName(modName, version)}.");
                modsStorageService.DiscardRelease(modName, version);
            }
        }
    }
}
