using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders
{
    internal class BuffSourceFinder20191001 : BuffSourceFinder20190305
    {
        public BuffSourceFinder20191001(HashSet<long> boonIds) : base(boonIds)
        {
            ImbuedMelodies = -1;
        }

        protected override bool CouldBeImbuedMelodies(AgentItem agent, long time, long extension, ParsedEvtcLog log)
        {
            return false;
        }
    }
}
