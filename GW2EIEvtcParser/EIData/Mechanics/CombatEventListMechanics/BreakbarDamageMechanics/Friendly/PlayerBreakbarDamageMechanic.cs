using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerBreakbarDamageMechanic : BreakbarDamageMechanic
{
    public PlayerBreakbarDamageMechanic(int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown, CombatEventsGetter getter) : base(id, plotlySetting, description, severity, internalCoolDown, getter)
    {
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (Player p in log.PlayerList)
        {
            foreach (BreakbarDamageEvent c in GetEvents(log, p.AgentItem))
            {
                if (Keep(c, log))
                {
                    InsertMechanic(log, mechanicLogs, c.Time, p, c.BreakbarDamage);
                }
            }
        }
    }
}
