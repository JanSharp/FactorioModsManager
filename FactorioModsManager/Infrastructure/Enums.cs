using System.Runtime.Serialization;

namespace FactorioModsManager.Infrastructure
{
    [DataContract]
    public enum ModDependencyType
    {
        [EnumMember]
        Regular,
        [EnumMember]
        Optional,
        [EnumMember]
        HiddenOptional,
        [EnumMember]
        Unordered,
        [EnumMember]
        Incompatible,
    }

    [DataContract]
    public enum ModDependencyOperator
    {
        [EnumMember]
        None,
        [EnumMember]
        Less,
        [EnumMember]
        LessEquals,
        [EnumMember]
        Equals,
        [EnumMember]
        GreaterEquals,
        [EnumMember]
        Greater,
    }
}
