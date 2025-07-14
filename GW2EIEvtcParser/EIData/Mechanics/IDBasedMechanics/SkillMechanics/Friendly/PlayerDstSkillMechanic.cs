using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class PlayerDstSkillMechanic<T> : PlayerSkillMechanic<T> where T : SkillEvent
{

    public PlayerDstSkillMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CombatEventsGetter getter) : base(mechanicID, plotlySetting, shortName, description, fullName, internalCoolDown, getter)
    {
    }

    public PlayerDstSkillMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CombatEventsGetter getter) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown, getter)
    {
    }
    protected override AgentItem GetAgentItem(T evt)
    {
        return evt.To;
    }
}
