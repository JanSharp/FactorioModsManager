using System.Collections.Generic;

namespace FactorioModsManager.Infrastructure
{
    public class ProgramArgs
    {
        public string? ConfigFilePath { get; set; }

        /// <summary>
        /// <para>Only create the config file. Does not overwrite existing ones files.</para>
        /// </summary>
        public bool CreateConfig { get; set; }

        public string? ExtractModsPath { get; set; }

        public string? ModListPath { get; set; }

        public string? SaveFilePath { get; set; }

        public List<string>? ModNamesToExtract { get; set; }
    }
}
