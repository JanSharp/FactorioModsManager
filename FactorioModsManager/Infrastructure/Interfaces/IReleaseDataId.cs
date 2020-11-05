namespace FactorioModsManager.Infrastructure.Interfaces
{
    public interface IReleaseDataId
    {
        string ModName { get; }

        FactorioVersion Version { get; }

        string GetFileName();
    }
}
