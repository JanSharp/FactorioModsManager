using System.Threading.Tasks;
using FactorioModPortalClient;

namespace FactorioModsManager.Services.Implementations
{
    public class MainService : IMainService
    {
        private readonly IConfigService configService;
        private readonly IModPortalClient client;

        public MainService(
            IConfigService configService,
            IModPortalClient client)
        {
            this.configService = configService;
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

        public async Task Run()
        {
            await foreach(var entry in client.EnumerateAsync())
            {

            }
        }
    }
}
