using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public class CombatStatusEvent : StatusEvent
{
    public readonly int Subgroup;
    public readonly Spec Spec = Spec.Unknown;
    public readonly Spec BaseSpec = Spec.Unknown;
    internal CombatStatusEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        Subgroup = (int)evtcItem.DstAgent;
        Spec = ProfToSpec(agentData.GetSpec((uint)evtcItem.Value, (uint)evtcItem.BuffDmg));
        BaseSpec = SpecToBaseSpec(Spec);
    }

}
