﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FactorioModsManager.Infrastructure
{
    [DataContract]
    public class ProgramData : IExtensibleDataObject
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ProgramData()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public ProgramData(Dictionary<string, ModData> mods)
        {
            Mods = mods;
        }

        [DataMember(/*IsRequired = true*/)]
        public Dictionary<string, ModData> Mods { get; set; }

        public ExtensionDataObject? ExtensionData { get; set; }
    }
}
