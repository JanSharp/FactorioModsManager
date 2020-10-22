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
        public ushort patch;

        public FactorioVersion()
        {
        }

        public FactorioVersion(ushort major, ushort minor, ushort patch)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }

        [OnSerializing]
        public void OnSerializing(StreamingContext context)
        {

        }

        [OnSerialized]
        public void OnSerialized(StreamingContext context)
        {

        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        private static readonly Regex VersionRegex = new Regex(@"^(\d+)\.(\d+)\.(\d+)$");
        public void ReadXml(XmlReader reader)
        {
            var version = reader.ReadElementContentAsString();
            var match = VersionRegex.Match(version);
            if (!match.Success)
                throw new SerializationException($"Invalid version '{version}'.");
            ushort Parse(string number)
            {
                if (ushort.TryParse(number, out ushort result))
                    return result;
                throw new SerializationException($"Invalid version '{version}', {number} must be a {nameof(UInt16)}.");
            }
            major = Parse(match.Groups[1].Value);
            minor = Parse(match.Groups[2].Value);
            patch = Parse(match.Groups[3].Value);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString($"{major}.{minor}.{patch}");
        }
    }
}
