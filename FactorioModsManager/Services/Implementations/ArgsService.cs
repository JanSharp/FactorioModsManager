using System;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services.Implementations
{
    public class ArgsService : IArgsService
    {
        private readonly ProgramArgs programArgs;

        public ArgsService(string[] args)
        {
            programArgs = new ProgramArgs();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--config":
                        ++i;
                        if (i >= args.Length)
                            throw new Exception($"Cmd arg '--config' requires a subsequent arg.");

                        programArgs.configFilePath = args[i];
                        break;

                    default:
                        throw new Exception($"Unexpected cmd arg '{args[i]}'.");
                }
            }
        }

        public ProgramArgs GetArgs() => programArgs;
    }
}
