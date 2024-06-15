using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffLossCastFinder : BuffCastFinder<BuffRemoveAllEvent>
    {
        public BuffLossCastFinder(long skillID, long buffID) : base(skillID, buffID)
        {
        }
        protected override AgentItem GetKeyAgent(BuffRemoveAllEvent evt)
        {
            return evt.To;
        }
    }
}
