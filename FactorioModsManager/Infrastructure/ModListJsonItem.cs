using System.Text.Json.Serialization;

namespace FactorioModsManager.Infrastructure
{
    public class ModListJsonItem
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ModListJsonItem()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public ModListJsonItem(string name, bool enabled)
        {
            Name = name;
            Enabled = enabled;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("version")]
        [JsonConverter(typeof(FactorioVersionJsonConverter))]
        public FactorioVersion? Version { get; set; }
    }
}
