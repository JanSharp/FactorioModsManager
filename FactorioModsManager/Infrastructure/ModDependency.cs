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
            TargetModName = targetMod.Name;
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

        [DataMember(/*IsRequired = true*/)]
        public string TargetModName { get; set; }

        public string GetTargetModName()
        {
            if (TargetMod != null && TargetModName != TargetMod.Name)
                throw new Exception($"{nameof(TargetModName)} ({TargetModName}) does not match " +
                    $"{nameof(TargetMod)}.{nameof(TargetMod.Name)} ({TargetMod.Name}).");

            return TargetModName;
        }

        [DataMember(/*IsRequired = true*/)]
        public ModDependencyType DependencyType { get; set; }

        [DataMember(IsRequired = false)]
        public FactorioVersion? TargetVersion { get; set; }

        [DataMember(/*IsRequired = true*/)]
        public ModDependencyOperator Operator { get; set; } = ModDependencyOperator.None;
    }
}
