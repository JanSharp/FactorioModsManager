using System;
using System.Collections.Generic;
using System.Text;
using FactorioModsManager.Infrastructure;

namespace FactorioModsManager.Services
{
    public interface IConfigService
    {
        Config GetConfig();

        void SetConfig(Config config);
    }
}
