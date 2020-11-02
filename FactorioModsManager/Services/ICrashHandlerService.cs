using System;

namespace FactorioModsManager.Services
{
    public interface ICrashHandlerService
    {
        string CreateDump(Exception ex);
        void WriteDump(string dump);
    }
}
