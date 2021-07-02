using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class AbstractTimeExtensionCombatEvent : AbstractTimeCombatEvent
    {

        internal AbstractTimeExtensionCombatEvent(long time) : base(time)
        {
        }
    }
}
