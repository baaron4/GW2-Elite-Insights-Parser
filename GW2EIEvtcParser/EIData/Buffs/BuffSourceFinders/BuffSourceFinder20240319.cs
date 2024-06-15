using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders
{
    internal class BuffSourceFinder20240319 : BuffSourceFinder20221018
    {
        public BuffSourceFinder20240319(HashSet<long> boonIds) : base(boonIds)
        {
            ImperialImpactExtension = int.MinValue;
        }

        protected override List<AgentItem> CouldBeImperialImpact(long buffID, long time, long extension, ParsedEvtcLog log)
        {
            return new List<AgentItem>();
        }

    }
}
