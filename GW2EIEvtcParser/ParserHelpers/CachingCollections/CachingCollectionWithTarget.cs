using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser;

public class CachingCollectionWithTarget<T>(ParsedEvtcLog log)
    : CachingCollectionCustom<SingleActor, T>(log, _nullActor, log.LogData.Logic.Hostiles.Count)
{
    private static readonly SingleActor _nullActor = new DummyActor(ParserHelper._nullAgent);
}


public class CachingCollectionWithAgentTarget<T>(ParsedEvtcLog log)
    : CachingCollectionCustom<AgentItem, T>(log, ParserHelper._nullAgent, log.LogData.Logic.Hostiles.Count)
{
}
