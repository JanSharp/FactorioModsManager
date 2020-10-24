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

            ProgramData programData = programDataService.GetProgramData();

            foreach (var mod in programData.Mods.Values)
                foreach (var release in mod.Releases)
                    if (release.IsMaintained)
                        Console.WriteLine(release.GetFileName());

            int count = 0;
            await foreach (var entry in client.EnumerateAsync())
            {
                if (++count > 100)
                    break;

                if (programData.Mods.TryGetValue(entry.Name, out var modData))
                {
                    if (modData.LatestRelease == null
                        || entry.LatestRelease.ReleasedAt != modData.LatestRelease.ReleasedAt)
                    {
                        SyncMod(entry, await client.GetResultEntryFullAsync(entry.Name), modData);
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
                }
            }

            // TODO: detect deleted mods

            TryResolveModDependencys(programData);

            programDataService.SetProgramData(programData);



            string modsPath = configService.GetConfig().GetFullModsPath();
            if (!Directory.Exists(modsPath))
                Directory.CreateDirectory(modsPath);

            var maintainedVersions = configService.GetConfig().MaintainedFactorioVersions
                .ToDictionary(v => v.FactorioVersion);

            foreach (var mod in programData.Mods.Values)
                await SyncMaintainedReleasesAsync(mod, maintainedVersions);

            programDataService.SetProgramData(programData);
        }

        public void SyncModPartial(ResultEntry portalMod, ModData modData)
        {
            mapperService.MapToModData(portalMod, modData);
        }

        public void SyncMod(ResultEntry entry, ResultEntryFull portalMod, ModData modData)
        {
            mapperService.MapToModData(portalMod, modData);

            var existingReleasesMap = modData.Releases.ToDictionary(r => r.Version.ToString());

            modData.Releases = new List<ReleaseData>(portalMod.Releases.Count);

            foreach (var portalRelease in portalMod.Releases.OrderByDescending(r => r.ReleasedAt))
            {
                ReleaseData releaseData;
                if (existingReleasesMap.TryGetValue(portalRelease.Version, out var release))
                    releaseData = release;
                else
                {
                    releaseData = mapperService.MapToReleaseData(modData, portalRelease);
                    foreach (var dependency in portalRelease.InfoJson.Dependencies.dependencies)
                    {
                        releaseData.Dependencies.Add(new ModDependency(releaseData, dependency));
                    }
                }
                modData.Releases.Add(releaseData);
                if (releaseData.ReleasedAt == entry.LatestRelease.ReleasedAt)
                    modData.LatestRelease = releaseData;
            }
        }

        public async Task SyncMaintainedReleasesAsync(
            ModData mod,
            Dictionary<FactorioVersion, MaintainedVersionConfig> maintainedVersions)
        {
            foreach (var versionGroup in mod.Releases.GroupBy(r => r.FactorioVersion))
            {
                if (maintainedVersions.TryGetValue(versionGroup.Key, out var maintainedVersion))
                {
                    int count = 0;
                    bool noMoreMaintainedReleases = false;
                    bool shouldDelete = maintainedVersion.DeleteNoLongerMaintainedReleases;

                    foreach (var release in versionGroup) // releases are stored ordered by ReleasedAt descending
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
                var bytes = await DownloadReleaseAsync(release);
                File.WriteAllBytes(path, bytes);
            }
            release.IsMaintained = true;
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
                    File.Delete(path);
                }
            }
            release.IsMaintained = false;
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
                foreach (var release in mod.Releases)
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
