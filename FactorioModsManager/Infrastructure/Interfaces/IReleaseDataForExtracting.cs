namespace FactorioModsManager.Infrastructure.Interfaces
{
    public interface IReleaseDataForExtracting : IReleaseDataForModsStorage
    {
        uint CRC { get; set; }
    }
}
