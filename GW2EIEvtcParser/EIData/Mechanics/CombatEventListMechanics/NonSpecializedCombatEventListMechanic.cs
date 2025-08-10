using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class NonSpecializedCombatEventListMechanic<T> : CombatEventListMechanic<T> where T : TimeCombatEvent
{

    public NonSpecializedCombatEventListMechanic(MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, bool isEnemyMechanic, CombatEventsGetter getter) : base(plotlySetting, shortName, description, fullName, internalCoolDown, getter)
    {
        IsEnemyMechanic = isEnemyMechanic;
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        if (!IsEnemyMechanic)
        {
            foreach (Player p in log.PlayerList)
            {
                foreach (T c in GetEvents(log, p.AgentItem))
                {
                    if (Keep(c, log))
                    {
                        InsertMechanic(log, mechanicLogs, c.Time, p);
                    }
                }
            }
        }
        else
        {
            foreach (SingleActor actor in log.LogData.Logic.Hostiles)
            {
                foreach (T c in GetEvents(log, actor.AgentItem))
                {
                    if (Keep(c, log))
                    {
                        SingleActor? actorToUse = MechanicHelper.FindEnemyActor(log, actor.AgentItem, regroupedMobs);
                        // no need to null check, we are already iterating over an existing actor list
                        InsertMechanic(log, mechanicLogs, c.Time, actorToUse!);
                    }
                }
            }
        }
    }
}
