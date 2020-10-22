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

        [DataMember(IsRequired = true)]
        public ReleaseData SourceRelease { get; set; }

        [DataMember(IsRequired = true)]
        public ModData TargetMod { get; set; }

        [DataMember(IsRequired = true)]
        public ModDependencyType DependencyType { get; set; }

        [DataMember(IsRequired = false)]
        public FactorioVersion? TargetVersion { get; set; }

        [DataMember(IsRequired = true)]
        public ModDependencyOperator Operator { get; set; } = ModDependencyOperator.None;
    }
}
