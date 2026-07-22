using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class EnemySrcSkillMechanic<T> : EnemySkillMechanic<T> where T : SkillEvent
{

    public EnemySrcSkillMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown, CombatEventsGetter getter) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown, getter)
    {
    }
    protected override AgentItem GetAgentItem(T evt)
    {
        return evt.From;
    }
}
