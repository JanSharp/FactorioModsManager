using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FactorioModsManager.Infrastructure
{
    [DataContract(IsReference = true)]
    public class ReleaseData
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ReleaseData()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public ReleaseData(string downloadUrl, FactorioVersion factorioVersion, List<ModDependency> dependencies, DateTime releasedAt, FactorioVersion version, string sha1)
        {
            DownloadUrl = downloadUrl;
            FactorioVersion = factorioVersion;
            Dependencies = dependencies;
            ReleasedAt = releasedAt;
            Version = version;
            Sha1 = sha1;
        }

        /// <summary>
        /// Path to download for a mod. starts with "/download" and does not include a full url.
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public string DownloadUrl { get; set; }

        /// <summary>
        /// from InfoJson
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public FactorioVersion FactorioVersion { get; set; }

        /// <summary>
        /// from InfoJson
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public List<ModDependency> Dependencies { get; set; }

        /// <summary>
        /// ISO 6501 for when the mod was released.
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public DateTime ReleasedAt { get; set; }

        /// <summary>
        /// The version string of this mod release. Used to determine dependencies.
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public FactorioVersion Version { get; set; }

        /// <summary>
        /// The sha1 key for the file
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public string Sha1 { get; set; }
    }
}
