﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FactorioModsManager.Infrastructure;
using FactorioModsManager.Infrastructure.Interfaces;

namespace FactorioModsManager.Services.Implementations
{
    public class ModsStorageService : IModsStorageService
    {
        private readonly IConfigService configService;

        public readonly string modsPath;
        public readonly Dictionary<string, List<FactorioVersion>> allStoredReleases;

        public ModsStorageService(IConfigService configService)
        {
            this.configService = configService;

            modsPath = configService.GetConfig().GetFullModsPath();
            allStoredReleases = new Dictionary<string, List<FactorioVersion>>();

            EnsureDirectoryExists(modsPath);
            InitializeStoredReleases();
        }

        private static readonly Regex ReleaseNameRegex = new Regex(@"^(?<name>.*?)_(?<version>\d+(?:\.\d+){2})\.zip$", RegexOptions.Compiled);
        public void InitializeStoredReleases()
        {
            foreach (var file in Directory.EnumerateFiles(modsPath).Select(f => Path.GetFileName(f)))
            {
                var match = ReleaseNameRegex.Match(file);
                if (match.Success)
                {
                    AddToAllStoredReleases(
                        match.Groups["name"].Value,
                        FactorioVersion.Parse(match.Groups["version"].Value));
                }
            }
        }

        public void AddToAllStoredReleases(string modName, FactorioVersion version)
        {
            if (!allStoredReleases.TryGetValue(modName, out var releases))
            {
                releases = new List<FactorioVersion>();
                allStoredReleases.Add(modName, releases);
            }
            releases.Add(version);
        }

        public void RemoveFromAllStoredReleases(string modName, FactorioVersion version)
        {
            if (allStoredReleases.TryGetValue(modName, out var releases))
            {
                releases.Remove(version);
                if (releases.Count == 0)
                    allStoredReleases.Remove(modName);
            }
        }

        public void EnsureDirectoryExists(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public bool ReleaseIsCached(IReleaseDataForModsStorage release)
        {
            return allStoredReleases.TryGetValue(release.ModName, out var releases)
                && releases.Contains(release.Version);
        }

        public bool ReleaseIsStored(IReleaseDataForModsStorage release)
        {
            return ReleaseIsStored(release.ModName, release.Version);
        }

        public bool ReleaseIsStored(string modName, FactorioVersion version)
        {
            return File.Exists(Path.Combine(modsPath, ReleaseData.GetFileName(modName, version)));
        }

        public void StoreRelease(IReleaseDataForModsStorage release, byte[] bytes)
        {
            File.WriteAllBytes(Path.Combine(modsPath, release.GetFileName()), bytes);
            AddToAllStoredReleases(release.ModName, release.Version);
        }

        public void DiscardRelease(IReleaseDataForModsStorage release)
        {
            DiscardRelease(release.ModName, release.Version);
        }

        public void DiscardRelease(string modName, FactorioVersion version)
        {
            File.Delete(Path.Combine(modsPath, ReleaseData.GetFileName(modName, version)));
            RemoveFromAllStoredReleases(modName, version);
        }

        public void GetAllCached(ModData mod, List<FactorioVersion> result)
        {
            GetAllCached(mod.Name, result);
        }

        public void GetAllCached(string modName, List<FactorioVersion> result)
        {
            result.Clear();
            if (allStoredReleases.TryGetValue(modName, out var releases))
                result.AddRange(releases);
        }

        public void ExtractRelease(IReleaseDataForModsStorage release, string extractModsPath)
        {
            string fileName = release.GetFileName();
            string source = Path.Combine(modsPath, fileName);
            string dest = Path.Combine(extractModsPath, fileName);

            if (!File.Exists(dest))
                File.Copy(source, dest);
        }
    }
}
