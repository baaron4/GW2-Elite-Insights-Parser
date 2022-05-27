using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal static class EnemyMechanicHelper
    {
        public static AbstractSingleActor FindActor(ParsedEvtcLog log, AgentItem a, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            if (log.FightData.Logic.TargetAgents.Contains(a))
            {
                return log.FightData.Logic.Targets.First(x => x.AgentItem == a);
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
    }
}
