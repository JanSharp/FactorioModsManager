using System;
using System.Collections.Generic;
using System.Text;

namespace FactorioModsManager.Infrastructure.Interfaces
{
    public interface IReleaseDataUnresolvedId
    {
        string ModName { get; }
        bool HasFixedVersion { get; }
        FactorioVersion? Version { get; set; }
        FactorioVersion GetVersion();
    }
}
