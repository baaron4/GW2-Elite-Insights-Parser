using GW2EIEvtcParser.ParsedData;

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

        List<(long Time, StatusEvent evt)> sanityCheckAdd = [];

        status = status.OrderBy(x => x.Time).ToList();

        for (var i = 0; i < status.Count; i++) 
        {
            var state = status[i].evt;
            if (state is DespawnEvent)
            {
                long nextTime = i == status.Count - 1 ? LastAware : status[i + 1].Time;
                var movement = combatData.GetMovementData(AgentItem).FirstOrDefault(x => x.Time > status[i].Time + 50 && x.Time < nextTime);
                bool addSanity = false;
                if (movement != null)
                {
                    addSanity = true;
                    nextTime = Math.Min(nextTime, movement.Time);
                }
                var damage = combatData.GetDamageData(AgentItem).FirstOrDefault(x => x.Time > status[i].Time + 50 && x.Time < nextTime);
                if (damage != null)
                {
                    addSanity = true;
                    nextTime = Math.Min(nextTime, damage.Time);
                }
                var cast = combatData.GetAnimatedCastData(AgentItem).FirstOrDefault(x => x.Time > status[i].Time + 50 && x.Time < nextTime);
                if (cast != null)
                {
                    addSanity = true;
                    nextTime = Math.Min(nextTime, cast.Time);
                }
                var buffApply = combatData.GetBuffApplyDataByDst(AgentItem).FirstOrDefault(x => x.Time > status[i].Time + 50 && x.Time < nextTime);
                if (buffApply != null)
                {
                    addSanity = true;
                    nextTime = Math.Min(nextTime, buffApply.Time);
                }
                if (addSanity)
                {
                    sanityCheckAdd.Add((nextTime, new SpawnEvent(AgentItem, nextTime)));
                }
            }
        }

        if (sanityCheckAdd.Count > 0)
        {
            status.AddRange(sanityCheckAdd);
        }

        FillStatus(dead, down, dc, actives, status);
    }
}
