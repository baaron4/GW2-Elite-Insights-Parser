using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser;

public class CachingCollectionWithTarget<T> : CachingCollectionCustom<SingleActor, T>
{

    private static readonly SingleActor _nullActor = new DummyActor(ParserHelper._nullAgent);

    public CachingCollectionWithTarget(AgentItem src, ParsedEvtcLog log) : base(log, _nullActor, log.FriendlyAgents.Contains(src.GetFinalMaster()) ?
        log.LogData.Logic.Targets.Count
        :
        log.LogData.Logic.TargetAgents.Contains(src.GetFinalMaster()) ?
            log.Friendlies.Count
            :
            5
    )
    {
    }
}


public class CachingCollectionWithAgentTarget<T> : CachingCollectionCustom<AgentItem, T>
{
    public CachingCollectionWithAgentTarget(AgentItem src, ParsedEvtcLog log) : base(log, ParserHelper._nullAgent, log.FriendlyAgents.Contains(src.GetFinalMaster()) ?
        log.LogData.Logic.Targets.Count
        :
        log.LogData.Logic.TargetAgents.Contains(src.GetFinalMaster()) ?
            log.Friendlies.Count
            :
            5
    )
    {
    }
}
