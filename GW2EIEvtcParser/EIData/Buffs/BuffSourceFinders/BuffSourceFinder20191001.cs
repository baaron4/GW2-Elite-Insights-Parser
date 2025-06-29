using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders;

internal class BuffSourceFinder20191001 : BuffSourceFinder20190305
{
    public BuffSourceFinder20191001(HashSet<long> boonIDs) : base(boonIDs)
    {
        ImbuedMelodies = int.MinValue;
    }

    protected override bool CouldBeImbuedMelodies(AgentItem agent, long buffID, long time, long extension, ParsedEvtcLog log)
    {
        return false;
    }
}
