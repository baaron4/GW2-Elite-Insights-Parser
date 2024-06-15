using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class AbstractSingleActorHelper
    {

        protected AbstractSingleActor Actor { get; }

        protected AgentItem AgentItem => Actor.AgentItem;

        public AbstractSingleActorHelper(AbstractSingleActor actor)
        {
            Actor = actor;
        }
    }
}
