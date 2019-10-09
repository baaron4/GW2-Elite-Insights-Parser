using System.Collections.Generic;

namespace GW2EIParser.EIData
{
    public class BuffSourceFinder01102019 : BuffSourceFinder05032019
    {
        public BuffSourceFinder01102019(HashSet<long> boonIds) : base(boonIds)
        {
            ImbuedMelodies = -1;
        }
    }
}
