using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services
{
    public interface IConfigService
    {
        Config GetConfig();

        void SetConfig(Config config);
    }
}
