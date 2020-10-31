using System.Collections.Generic;
using FactorioModPortalClient;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services.Implementations
{
    public class MapperService : IMapperService
    {
        /// <summary>
        /// <para>does not map <see cref="ModData.GroupedReleases"/></para>
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public ModData MapToModData(ResultEntry entry, ModData result)
        {
            result.DownloadsCount = entry.DownloadsCount;
            //result.Name = entry.Name;
            result.Owner = entry.Owner;
            result.Summary = entry.Summary;
            result.Title = entry.Title;

            return result;
        }

        /// <summary>
        /// <para>does not map <see cref="ModData.GroupedReleases"/></para>
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public ModData MapToModData(ResultEntryFull entry, ModData? result = null)
        {
            if (result == null)
                result = new ModData();

            result.DownloadsCount = entry.DownloadsCount;
            result.Name = entry.Name;
            result.Owner = entry.Owner;
            result.GroupedReleases ??= new Dictionary<FactorioVersion, List<ReleaseData>>();
            result.Summary = entry.Summary;
            result.Title = entry.Title;
            result.Changelog = entry.Changelog;
            result.CreatedAt = entry.CreatedAt;
            result.Description = entry.Description;
            result.GithubPath = entry.GithubPath;
            result.Homepage = entry.Homepage;

            return result;
        }

        /// <summary>
        /// <para>does not map <see cref="ReleaseData.Dependencies"/></para>
        /// </summary>
        /// <param name="release"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public ReleaseData MapToReleaseData(ModData mod, Release release, ReleaseData? result = null)
        {
            if (result == null)
                result = new ReleaseData();

            result.Mod = mod;
            result.DownloadUrl = release.DownloadUrl;
            result.FactorioVersion = FactorioVersion.Parse(release.InfoJson.FactorioVersion, readPatch: false);
            result.Dependencies ??= new List<ModDependency>();
            result.ReleasedAt = release.ReleasedAt;
            result.Version = FactorioVersion.Parse(release.Version);
            result.Sha1 = release.Sha1;

            return result;
        }
    }
}
