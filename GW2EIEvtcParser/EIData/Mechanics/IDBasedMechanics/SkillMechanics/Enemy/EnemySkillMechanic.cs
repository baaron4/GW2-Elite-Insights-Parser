using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class EnemySkillMechanic<T> : SkillMechanic<T> where T : SkillEvent
{

    public EnemySkillMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown, CombatEventsGetter getter) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown, getter)
    {
        IsEnemyMechanic = true;
    }
    protected override bool TryGetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs, [NotNullWhen(true)] out SingleActor? actor)
    {
        actor = MechanicHelper.FindEnemyActor(log, agentItem, regroupedMobs);
        return actor != null;
    }

}
