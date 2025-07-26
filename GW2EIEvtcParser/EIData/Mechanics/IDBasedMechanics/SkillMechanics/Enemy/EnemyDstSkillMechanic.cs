using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class EnemyDstSkillMechanic<T> : EnemySkillMechanic<T> where T : SkillEvent
{

    public EnemyDstSkillMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CombatEventsGetter getter) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown, getter)
    {
    }
    protected override AgentItem GetAgentItem(T evt)
    {
        return evt.To;
    }
}
