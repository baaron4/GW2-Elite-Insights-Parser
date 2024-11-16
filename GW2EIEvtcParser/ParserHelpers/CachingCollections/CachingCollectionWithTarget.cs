using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser;

public class CachingCollectionWithTarget<T>(ParsedEvtcLog log, int initialPrimaryCapacity = 0, int initialSecondaryCapacity = 0, int initialTertiaryCapacity = 0)
    : CachingCollectionCustom<AbstractSingleActor, T>(log, _nullActor, initialPrimaryCapacity, initialSecondaryCapacity, initialTertiaryCapacity)
{
    private static readonly NPC _nullActor = new(new AgentItem());
}
