using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services
{
    public interface IArgsService
    {
        ProgramArgs GetArgs();
        ExecutionType GetExecutionType(ProgramArgs programArgs);
    }
}
