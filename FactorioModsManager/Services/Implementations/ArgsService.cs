using System;
using System.Collections.Generic;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services.Implementations
{
    public class ArgsService : IArgsService
    {
        private readonly ProgramArgs programArgs;

        public ArgsService(string[] args)
        {
            programArgs = ArgsService.ParseArgs(args);
        }

        public ProgramArgs GetArgs() => programArgs;

        public static ProgramArgs ParseArgs(string[] args)
        {
            var result = new ProgramArgs();

            for (int i = 0; i < args.Length; i++)
            {
                void MoveToExtraArgFor(string argName)
                {
                    ++i;
                    if (i >= args.Length)
                        throw new Exception($"Cmd arg '{argName}' requires a subsequent arg.");
                }

                switch (args[i])
                {
                    case "--config":
                        MoveToExtraArgFor("--config");
                        result.ConfigFilePath = args[i];
                        break;

                    case "--create-config":
                        result.CreateConfig = true;
                        break;

                    case "--extract-mods-path":
                        MoveToExtraArgFor("--extract-mods-path");
                        result.ExtractModsPath = args[i];
                        break;

                    case "--mod-list-path":
                        MoveToExtraArgFor("--mod-list-path");
                        result.ModListPath = args[i];
                        break;

                    case "--save-file-path":
                        MoveToExtraArgFor("--save-file-path");
                        result.SaveFilePath = args[i];
                        break;

                    case "--extract-mod-name":
                    case "-e":
                        MoveToExtraArgFor("--extract-mod-name");
                        if (result.ModNamesToExtract == null)
                            result.ModNamesToExtract = new List<string>();
                        result.ModNamesToExtract.Add(args[i]);
                        break;

                    default:
                        throw new Exception($"Unexpected cmd arg '{args[i]}'.");
                }
            }

            bool extractModsPathIsSet = result.ExtractModsPath != null;

            int argsThatNeedAnExtractPathIsSetCount = (result.ModListPath != null ? 1 : 0)
                + (result.SaveFilePath != null ? 1 : 0)
                + (result.ModNamesToExtract != null ? 1 : 0);

            if (extractModsPathIsSet && argsThatNeedAnExtractPathIsSetCount == 0)
            {
                throw new Exception($"Cmd arg '--extract-mods-path' requires " +
                    $"'--mod-list-path' xor '--save-file-path' xor '--extract-mod-name' to be set.");
            }

            if (argsThatNeedAnExtractPathIsSetCount > 0 && !extractModsPathIsSet)
            {
                throw new Exception($"Must set cmd arg '--extract-mods-path' for " +
                    $"'--mod-list-path', '--save-file-path' or '--extract-mod-name'.");
            }

            if (argsThatNeedAnExtractPathIsSetCount > 1)
            {
                throw new Exception($"Only one of the cmd args '--mod-list-path', " +
                    $"'--save-file-path' and '--extract-mod-name' can be set.");
            }

            return result;
        }
    }
}
