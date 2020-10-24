namespace FactorioModsManager.Infrastructure
{
    public class ProgramArgs
    {
        public string? ConfigFilePath { get; set; }

        /// <summary>
        /// <para>Only create the config file. Does not overwrite existing ones files.</para>
        /// </summary>
        public bool CreateConfig { get; set; }
    }
}
