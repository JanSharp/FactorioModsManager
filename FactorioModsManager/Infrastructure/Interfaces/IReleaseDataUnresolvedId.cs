namespace FactorioModsManager.Infrastructure.Interfaces
{
    public interface IReleaseDataUnresolvedId : IReleaseDataId
    {
        bool HasFixedVersion { get; }

        new FactorioVersion? Version { get; set; }

        FactorioVersion GetVersion();
    }
}
