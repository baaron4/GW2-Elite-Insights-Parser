using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParsedData.AgentItem;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

partial class PlayerActor
{
    // Don't use regrouped for Players, Arc automatically redirects events for them
    protected override void GetAgentStatus(List<Segment> dead, List<Segment> down, List<Segment> dc, List<Segment> actives, CombatData combatData)
    {
        var downEvents = combatData.GetDownEvents(AgentItem);
        var aliveEvents = combatData.GetAliveEvents(AgentItem);
        var deadEvents = combatData.GetDeadEvents(AgentItem);
        var spawnEvents = combatData.GetSpawnEvents(AgentItem);
        var despawnEvents = combatData.GetDespawnEvents(AgentItem);

        var status = new List<(long Time, StatusEvent evt)>(
            downEvents.Count +
            aliveEvents.Count +
            deadEvents.Count +
            spawnEvents.Count +
            despawnEvents.Count +
            (AgentItem.IsEnglobedAgent ? 1 : 0)
        );
        if (AgentItem.IsEnglobedAgent)
        {
            List<StatusEvent?> firstEvents = [
                combatData.GetDownEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware),
                combatData.GetAliveEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware),
                combatData.GetDeadEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware),
                combatData.GetSpawnEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware),
                combatData.GetDespawnEvents(EnglobingAgentItem).LastOrDefault(x => x.Time < FirstAware),
            ];
            var firstEvent = firstEvents.Where(x => x != null).OrderBy(x => x!.Time).LastOrDefault();
            if (firstEvent != null)
            {
                status.Add((FirstAware, firstEvent));
            }
        }
        status.AddRange(downEvents.Select(x => (x.Time, (StatusEvent)x)));
        status.AddRange(aliveEvents.Select(x => (x.Time, (StatusEvent)x)));
        status.AddRange(deadEvents.Select(x => (x.Time, (StatusEvent)x)));
        status.AddRange(spawnEvents.Select(x => (x.Time, (StatusEvent)x)));
        status.AddRange(despawnEvents.Select(x => (x.Time, (StatusEvent)x)));


        FillStatus(dead, down, dc, actives, status);
    }
}
