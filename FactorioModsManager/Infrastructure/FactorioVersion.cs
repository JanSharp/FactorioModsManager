using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FactorioModsManager.Infrastructure
{
    public class FactorioVersion : IXmlSerializable
    {
        public ushort major;
        public ushort minor;
        public ushort? patch;

        public FactorioVersion()
        {
        }

        public FactorioVersion(ushort major, ushort minor, ushort? patch = null)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }

        private static readonly Regex VersionRegex = new Regex(@"^(\d+)\.(\d+)(?:\.(\d+))?$");
        public static FactorioVersion Parse(string version, FactorioVersion? result = null)
        {
            if (result == null)
                result = new FactorioVersion();

            var match = VersionRegex.Match(version);
            if (!match.Success)
                throw new SerializationException($"Invalid version '{version}'.");
            ushort Parse(string number)
            {
                if (ushort.TryParse(number, out ushort result))
                    return result;
                throw new SerializationException($"Invalid version '{version}', {number} must be a {nameof(UInt16)}.");
            }
            result.major = Parse(match.Groups[1].Value);
            result.minor = Parse(match.Groups[2].Value);
            result.patch = match.Groups[3].Success ? Parse(match.Groups[3].Value) : new ushort?();

            return result;
        }

        public override string ToString()
        {
            return $"{major}.{minor}{(patch.HasValue ? $".{patch}" : "")}";
        }

        public bool Equals(FactorioVersion? other)
        {
            return other is object && major == other.major && minor == other.minor && patch == other.patch;
        }

        public override bool Equals(object? obj)
        {
            if (obj is FactorioVersion other)
                return this.Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(major, minor, patch);
        }

        public static bool operator ==(FactorioVersion? left, FactorioVersion? right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(FactorioVersion? left, FactorioVersion? right) => !(left == right);

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            Parse(reader.ReadElementContentAsString(), this);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString(this.ToString());
        }
    }
}
