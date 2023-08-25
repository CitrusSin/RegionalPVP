using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalPVP
{
    public class RegionalPVPConfig : IRocketPluginConfiguration
    {
        public bool DefaultPvPMode;
        public List<RegionPolicy> RegionPolicies;

        public void LoadDefaults()
        {
            DefaultPvPMode = false;
            RegionPolicies = new List<RegionPolicy>();
        }
    }
}
