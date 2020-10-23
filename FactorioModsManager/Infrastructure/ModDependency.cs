using System;
using System.Runtime.Serialization;

namespace FactorioModsManager.Infrastructure
{
    [DataContract(IsReference = true)]
    public class ModDependency
    {
        public ModDependency(ReleaseData sourceRelease, ModData targetMod, ModDependencyType dependencyType)
        {
            SourceRelease = sourceRelease;
            TargetMod = targetMod;
            DependencyType = dependencyType;
        }

        public ModDependency(ReleaseData sourceRelease, string targetModName, ModDependencyType dependencyType)
        {
            SourceRelease = sourceRelease;
            TargetModName = targetModName;
            DependencyType = dependencyType;
        }

        [DataMember(/*IsRequired = true*/)]
        public ReleaseData SourceRelease { get; set; }

        [DataMember(IsRequired = false)]
        public ModData? TargetMod { get; set; }

        [DataMember(IsRequired = false)]
        public string? TargetModName { get; set; }

        public string GetTargetModName()
        {
            if (TargetModName != null
                && TargetMod != null
                && TargetModName != TargetMod.Name)
                throw new Exception($"{nameof(TargetModName)} ({TargetModName}) does not match " +
                    $"{nameof(TargetMod)}.{nameof(TargetMod.Name)} ({TargetMod.Name}).");

            return TargetModName ?? TargetMod?.Name
                ?? throw new Exception($"Unable to get the target mod name of a {nameof(ModDependency)} " +
                $"for {nameof(TargetMod)} and {nameof(TargetMod)}.{nameof(TargetMod.Name)} are both null.");
        }

        [DataMember(/*IsRequired = true*/)]
        public ModDependencyType DependencyType { get; set; }

        [DataMember(IsRequired = false)]
        public FactorioVersion? TargetVersion { get; set; }

        [DataMember(/*IsRequired = true*/)]
        public ModDependencyOperator Operator { get; set; } = ModDependencyOperator.None;
    }
}
