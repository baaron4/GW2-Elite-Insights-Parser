using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal static class MechanicHelper
    {
        public static AbstractSingleActor FindEnemyActor(ParsedEvtcLog log, AgentItem a, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            if (log.FightData.Logic.TargetAgents.Contains(a))
            {
                return log.FindActor(a, true);
            }
            // We regroup trash mobs by their ID
            if (log.FightData.Logic.TrashMobAgents.Contains(a))
            {
                if (!regroupedMobs.TryGetValue(a.ID, out AbstractSingleActor amp))
                {
                    amp = log.FightData.Logic.TrashMobs.First(x => x.AgentItem == a);
                    regroupedMobs.Add(amp.ID, amp);
                }
                return amp;
            }
            return null;
        }

        public static AbstractSingleActor FindPlayerActor(ParsedEvtcLog log, AgentItem a)
        {
            if (log.PlayerAgents.Contains(a))
            {
                return log.FindActor(a);
            }
            return null;
        }
    }
}
