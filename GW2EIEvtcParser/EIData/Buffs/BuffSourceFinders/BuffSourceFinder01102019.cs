using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSourceFinder01102019 : BuffSourceFinder05032019
    {
        public BuffSourceFinder01102019(HashSet<long> boonIds) : base(boonIds)
        {
            ImbuedMelodies = -1;
        }

        protected override bool CouldBeImbuedMelodies(AgentItem agent, long time, long extension, ParsedEvtcLog log)
        {
            return false;
        }
    }
}
