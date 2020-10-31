using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace FactorioModsManager.Infrastructure
{
    [DataContract(IsReference = true)]
    public class ModDependency : IExtensibleDataObject
    {
        public ModDependency(ReleaseData sourceRelease, string targetModName, ModDependencyType dependencyType)
        {
            SourceRelease = sourceRelease;
            TargetModName = targetModName;
            DependencyType = dependencyType;
        }

        static readonly Regex DependencyRegex = new Regex(@"
            ^
            (?>(?<prefix>[!?]|\(\?\))\ *)?
            (?<name>(?>\ *[a-zA-Z0-9_-]+)+(?>\ *$)?)
            (?>\ *
                (?<operator>[<>=]=?)\ *
                (?<version>(?>\d+\.){1,2}\d+)
            )?
            $
        ", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public ModDependency(ReleaseData sourceRelease, string rawDependencyString)
        {
            SourceRelease = sourceRelease;

            Match match = DependencyRegex.Match(rawDependencyString);

            TargetModName = match.Groups["name"].Value; // that mod might not even exist

            DependencyType = match.Groups["prefix"].Value switch
            {
                "?" => ModDependencyType.Optional,
                "(?)" => ModDependencyType.HiddenOptional,
                "!" => ModDependencyType.Incompatible,
                _ => ModDependencyType.Regular,
            };

            Operator = match.Groups["operator"].Value switch
            {
                "<" => ModDependencyOperator.Less,
                "<=" => ModDependencyOperator.LessEquals,
                "=" => ModDependencyOperator.Equals,
                "==" => ModDependencyOperator.Equals,
                ">=" => ModDependencyOperator.GreaterEquals,
                ">" => ModDependencyOperator.Greater,
                _ => ModDependencyOperator.None,
            };

            if (match.Groups["version"].Success)
                TargetVersion = FactorioVersion.Parse(match.Groups["version"].Value);
        }

        [DataMember(/*IsRequired = true*/)]
        public ReleaseData SourceRelease { get; set; }

        [DataMember(/*IsRequired = true*/)]
        public string TargetModName { get; set; }

        [DataMember(/*IsRequired = true*/)]
        public ModDependencyType DependencyType { get; set; }

        [DataMember(IsRequired = false)]
        public FactorioVersion? TargetVersion { get; set; }

        [DataMember(/*IsRequired = true*/)]
        public ModDependencyOperator Operator { get; set; } = ModDependencyOperator.None;

        public ExtensionDataObject? ExtensionData { get; set; }
    }
}
