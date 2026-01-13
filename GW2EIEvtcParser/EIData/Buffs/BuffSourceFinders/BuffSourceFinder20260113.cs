using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders;

internal class BuffSourceFinder20260113 : BuffSourceFinder20240319
{
    public BuffSourceFinder20260113(HashSet<long> boonIDs) : base(boonIDs)
    {
        ResoundingTimbre = 2000;
    }

}
