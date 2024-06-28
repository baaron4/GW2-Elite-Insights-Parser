using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData
{
    public class EnterCombatEvent : AbstractStatusEvent
    {
        public int Subgroup { get; }
        public Spec Spec { get; } = Spec.Unknown;
        public Spec BaseSpec { get; } = Spec.Unknown;
        internal EnterCombatEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            Subgroup = (int)evtcItem.DstAgent;
            Spec = ProfToSpec(agentData.GetSpec((uint)evtcItem.Value, (uint)evtcItem.BuffDmg));
            BaseSpec = SpecToBaseSpec(Spec);
        }

    }
}
