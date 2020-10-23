using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FactorioModsManager.Infrastructure
{
    [DataContract(IsReference = true)]
    public class ModData
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ModData()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public ModData(string name, string owner, List<ReleaseData> releases, string title, DateTime createdAt)
        {
            Name = name;
            Owner = owner;
            Releases = releases;
            Title = title;
            CreatedAt = createdAt;
        }

        /// <summary>
        /// Number of downloads.
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public int DownloadsCount { get; set; }

        /// <summary>
        /// The mod's machine-readable ID string.
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public string Name { get; set; }

        /// <summary>
        /// The Factorio username of the mod's author.
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public string Owner { get; set; }

        /// <summary>
        /// A list of different versions of the mod available for download.
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public List<ReleaseData> Releases { get; set; }

        /// <summary>
        /// A shorter mod description.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string? Summary { get; set; }

        /// <summary>
        /// The mod's human-readable name.
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public string Title { get; set; }

        /// <summary>
        /// A string describing the recent changes to a mod.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string? Changelog { get; set; }

        /// <summary>
        /// ISO 6501 for when the mod was created.
        /// </summary>
        [DataMember(/*IsRequired = true*/)]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// A longer description of the mod, in text only format.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string? Description { get; set; }

        /// <summary>
        /// A link to the mod's github project page, just prepend "github.com/". Can be blank ("").
        /// </summary>
        [DataMember(IsRequired = false)]
        public string? GithubPath { get; set; }

        /// <summary>
        /// Usually a URL to the mod's main project page, but can be any string.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string? Homepage { get; set; }

        public ReleaseData? GetLatestReleaseData()
        {
            return Releases.OrderByDescending(r => r.ReleasedAt).FirstOrDefault();
        }
    }
}
