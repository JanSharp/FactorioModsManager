using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FactorioModPortalClient;
using FactorioModsManager.Infrastructure;

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
                    mapperService.MapToModData(entry, modData);
                }
                else
                {
                    var entryFull = await client.GetResultEntryFullAsync(entry.Name);
                    modData = mapperService.MapToModData(entryFull);
                    programData.Mods.Add(modData.Name, modData);

                    foreach (var release in entryFull.Releases)
                    {
                        var releaseData = mapperService.MapToReleaseData(release);
                        modData.Releases.Add(releaseData);

                        foreach (var dependency in release.InfoJson.Dependencies.dependencies)
                        {

                        }
                    }
                }
            }

            TryResolveModDependencys(programData);

            programDataService.SetProgramData(programData);
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
