using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class EnemySrcSkillMechanic<T> : EnemySkillMechanic<T> where T : SkillEvent
{

    public EnemySrcSkillMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown, CombatEventsGetter getter) : base(mechanicIDs, plotlySetting, shortName, description, fullName, severity, internalCoolDown, getter)
    {
    }
    protected override AgentItem GetAgentItem(T evt)
    {
        return evt.From;
    }
}
