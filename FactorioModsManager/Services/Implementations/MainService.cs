using System;
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
            DateTime lastSaveTime = DateTime.Now;

            Console.WriteLine($"Loaded {programData.Mods.Count} mods from data file.");

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
                    Console.WriteLine($"Saving {attemptedSaveCount} changed or added mods.");
                    attemptedSaveCount = 0;
                    programDataService.SetProgramData(programData);
                    lastSaveTime = DateTime.Now;
                }
            }

            await foreach (var entry in client.EnumerateAsync())
            {
                if (programData.Mods.TryGetValue(entry.Name, out var modData))
                {
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

            // TODO: detect deleted mods

            Console.WriteLine("Attempting to resolve mod dependencies.");
            TryResolveModDependencys(programData);

            if (attemptedSaveCount > 0)
            {
                --attemptedSaveCount;
                SaveChanges(bypassConditions: true);
            }

            Console.WriteLine("Determining which mods should be maintained. Downloading and deleting accordingly.");

            string modsPath = configService.GetConfig().GetFullModsPath();
            if (!Directory.Exists(modsPath))
                Directory.CreateDirectory(modsPath);

            var maintainedVersions = configService.GetConfig().MaintainedFactorioVersions
                .ToDictionary(v => v.FactorioVersion);

            foreach (var mod in programData.Mods.Values)
                await SyncMaintainedReleasesAsync(mod, maintainedVersions);

            //programDataService.SetProgramData(programData);
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
            Dictionary<FactorioVersion, MaintainedVersionConfig> maintainedVersions)
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

        public async Task EnsureReleaseIsMaintainedAsync(ReleaseData release)
        {
            if (release.IsMaintained)
                return;

            string modsPath = configService.GetConfig().GetFullModsPath();
            string path = Path.Combine(modsPath, release.GetFileName());
            if (!File.Exists(path))
            {
                Console.WriteLine($"Downloading {release.GetFileName()}.");
                var bytes = await DownloadReleaseAsync(release);
                File.WriteAllBytes(path, bytes);
            }
            release.IsMaintained = true;
            Console.WriteLine($"{release.GetFileName()} is now maintained.");
        }

        public void EnsureReleaseIsNotMaintained(ReleaseData release, bool shouldDelete)
        {
            if (!release.IsMaintained)
                return;

            if (shouldDelete)
            {
                string modsPath = configService.GetConfig().GetFullModsPath();
                string path = Path.Combine(modsPath, release.GetFileName());
                if (File.Exists(path))
                {
                    Console.WriteLine($"Deleting {release.GetFileName()}.");
                    File.Delete(path);
                }
            }
            release.IsMaintained = false;
            Console.WriteLine($"{release.GetFileName()} is no longer maintained.");
        }

        public Task<byte[]> DownloadReleaseAsync(ReleaseData release)
        {
            return client.DownloadModAsByteArrayAsync(new Release()
            {
                DownloadUrl = release.DownloadUrl,
                Sha1 = release.Sha1,
            });
        }

        public void TryResolveModDependencys(ProgramData programData)
        {
            var mods = programData.Mods;

            foreach (var mod in mods.Values)
            {
                foreach (var releases in mod.GroupedReleases.Values)
                {
                    foreach (var release in releases)
                    {
                        foreach (var dependency in release.Dependencies)
                        {
                            if (dependency.TargetMod == null
                                && mods.TryGetValue(dependency.GetTargetModName(), out var modData))
                            {
                                dependency.TargetMod = modData;
                            }
                        }
                    }
                }
            }
        }
    }
}
