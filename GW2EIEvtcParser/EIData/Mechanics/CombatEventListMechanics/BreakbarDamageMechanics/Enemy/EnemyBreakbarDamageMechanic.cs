using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyBreakbarDamageMechanic : BreakbarDamageMechanic
{

    public EnemyBreakbarDamageMechanic(MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CombatEventsGetter getter) : base(plotlySetting, shortName, description, fullName, internalCoolDown, getter)
    {
        IsEnemyMechanic = true;
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (SingleActor actor in log.LogData.Logic.Hostiles)
        {
            foreach (BreakbarDamageEvent c in GetEvents(log, actor.AgentItem))
            {
                if (Keep(c, log))
                {
                    SingleActor? actorToUse = MechanicHelper.FindEnemyActor(log, actor.AgentItem, regroupedMobs);
                    // no need to null check, we are already iterating over an existing actor list
                    InsertMechanic(log, mechanicLogs, c.Time, actorToUse!, c.BreakbarDamage);
                }
            }
        }
    }
}
