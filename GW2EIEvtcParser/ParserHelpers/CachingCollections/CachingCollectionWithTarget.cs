using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser
{
    public class CachingCollectionWithTarget<T> : CachingCollectionCustom<AbstractActor, T>
    {
        private static readonly NPC _nullActor = new NPC(new AgentItem());

        public CachingCollectionWithTarget(ParsedEvtcLog log) : base(log, _nullActor)
        {
        }

    }
}
