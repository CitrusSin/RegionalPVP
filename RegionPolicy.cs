using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalPVP
{
    public class RegionPolicy
    {
        public string Name;
        public CircleRegion Region;
        public bool IsPvP;

        public RegionPolicy(string name, CircleRegion region, bool isPvP = true)
        {
            Name = name;
            Region = region;
            IsPvP = isPvP;
        }

        public RegionPolicy()
        {
            Name = "Default";
            Region = new CircleRegion();
            IsPvP = true;
        }
    }
}
