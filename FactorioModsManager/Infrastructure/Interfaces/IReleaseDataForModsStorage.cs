namespace FactorioModsManager.Infrastructure.Interfaces
{
    public interface IReleaseDataForModsStorage
    {
        string ModName { get; }

        FactorioVersion Version { get; set; }

        string GetFileName();
    }
}
