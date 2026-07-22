using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class EnemyDstSkillMechanic<T> : EnemySkillMechanic<T> where T : SkillEvent
{

    public EnemyDstSkillMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown, CombatEventsGetter getter) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown, getter)
    {
    }
    protected override AgentItem GetAgentItem(T evt)
    {
        return evt.To;
    }
}
