using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal static class MechanicHelper
{
    public static SingleActor? FindEnemyActor(ParsedEvtcLog log, AgentItem a, Dictionary<int, SingleActor> regroupedMobs)
    {
        if (log.LogData.Logic.TargetAgents.Contains(a))
        {
            return log.FindActor(a, true);
        }
        // We regroup trash mobs by their ID
        if (log.LogData.Logic.TrashMobAgents.Contains(a))
        {
            if (!regroupedMobs.TryGetValue(a.ID, out var amp))
            {
                amp = log.LogData.Logic.TrashMobs.First(x => x.AgentItem.Is(a));
                regroupedMobs.Add(amp.ID, amp);
            }
            return amp;
        }
        return null;
    }

    public static SingleActor? FindPlayerActor(ParsedEvtcLog log, AgentItem a)
    {
        if (a.IsPlayer)
        {
            return log.FindActor(a);
        }
        return null;
    }
}
