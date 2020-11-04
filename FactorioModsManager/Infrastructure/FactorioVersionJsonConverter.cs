using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FactorioModsManager.Infrastructure
{
    public class FactorioVersionJsonConverter : JsonConverter<FactorioVersion>
    {
        public override FactorioVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return FactorioVersion.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, FactorioVersion value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                writer.WriteString("version", value.ToString());
            }
        }
    }
}
