using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FactorioModPortalClient;
using FactorioModsManager.Infrastructure;

using static MoreLinq.Extensions.FullGroupJoinExtension;

namespace FactorioModsManager.Services.Implementations
{
    public class MainService : IMainService
    {
        private readonly IConfigService configService;
        private readonly IProgramDataService programDataService;
        private readonly IMapperService mapperService;
        private readonly IModPortalClient client;

        public MainService(
            IConfigService configService,
            IProgramDataService programDataService,
            IMapperService mapperService,
            IModPortalClient client)
        {
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
            ProgramData programData = programDataService.GetProgramData();

            int count = 0;
            await foreach (var entry in client.EnumerateAsync())
            {
                if (++count > 10)
                    break;

                if (programData.Mods.TryGetValue(entry.Name, out var modData))
                {
                    var existingLatestRelease = modData.GetLatestReleaseData();
                    if (existingLatestRelease == null
                        || entry.LatestRelease.ReleasedAt != existingLatestRelease.ReleasedAt)
                    {
                        SyncMod(await client.GetResultEntryFullAsync(entry.Name), modData);
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
                    SyncMod(entryFull, modData);
                    programData.Mods.Add(modData.Name, modData);
                }
            }

            TryResolveModDependencys(programData);

            programDataService.SetProgramData(programData);
        }

        public void SyncModPartial(ResultEntry portalMod, ModData modData)
        {
            mapperService.MapToModData(portalMod, modData);
        }

        public void SyncMod(ResultEntryFull portalMod, ModData modData)
        {
            mapperService.MapToModData(portalMod, modData);

            HashSet<string> maintainedFactorioVersions = configService.GetConfig().MaintainedFactorioVersions
                .Select(v => v.ToString())
                .ToHashSet();

            var portalMaintainedReleases = portalMod.Releases
                .Where(r => maintainedFactorioVersions.Contains(r.InfoJson.FactorioVersion));

            foreach (var joined in modData.Releases.FullGroupJoin(portalMaintainedReleases,
                release => release.Version.ToString(), portalRelease => portalRelease.Version,
                (key, releases, portalReleases) => (release: releases.SingleOrDefault(), portalRelease: portalReleases.SingleOrDefault())))
            {
                if (joined.release == null)
                {
                    // TODO: check if a zip for this release already exists
                    // otherwise download

                    var releaseData = mapperService.MapToReleaseData(joined.portalRelease!);
                    modData.Releases.Add(releaseData);

                    foreach (var dependency in joined.portalRelease!.InfoJson.Dependencies.dependencies)
                    {
                        releaseData.Dependencies.Add(new ModDependency(releaseData, dependency));
                    }
                }
                else if (joined.portalRelease == null) // doesn't exist in the portal anymore
                {
                    // TODO: should this delete the local release?
                }
            }
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
