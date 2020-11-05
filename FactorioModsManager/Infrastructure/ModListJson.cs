using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FactorioModsManager.Infrastructure
{
    public class ModListJson
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ModListJson()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public ModListJson(List<ModListJsonItem> mods)
        {
            Mods = mods;
        }

        [JsonPropertyName("mods")]
        public List<ModListJsonItem> Mods { get; set; }
    }
}
