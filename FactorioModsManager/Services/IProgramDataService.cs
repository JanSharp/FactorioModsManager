using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services
{
    public interface IProgramDataService
    {
        ProgramData GetProgramData();

        void SetProgramData(ProgramData programData);
    }
}
