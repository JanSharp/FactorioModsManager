﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FactorioModsManager.Infrastructure
{
    [DataContract]
    public class ProgramData
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ProgramData()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public ProgramData(List<ModData> mods)
        {
            Mods = mods;
        }

        [DataMember(IsRequired = true)]
        public int ProgramDataVersion { get; set; } = 1;

        [DataMember(IsRequired = true)]
        public List<ModData> Mods { get; set; }
    }
}
