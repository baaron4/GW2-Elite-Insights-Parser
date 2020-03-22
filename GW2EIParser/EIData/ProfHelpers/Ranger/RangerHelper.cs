using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class RangerHelper : ProfHelper
    {

        public static void AttachMasterToRangerGadgets(List<Player> players, Dictionary<long, List<AbstractDamageEvent>> damageData, Dictionary<long, List<AbstractCastEvent>> castData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // entangle works fine already
            HashSet<AgentItem> jacarandaEmbrace = GetOffensiveGadgetAgents(damageData, 1286, playerAgents);
            var rangers = players.Where(x => x.Prof == "Ranger" || x.Prof == "Soulbeast" || x.Prof == "Druid").ToList();
            // if only one ranger, could only be that one
            if (rangers.Count == 1)
            {
                Player ranger = rangers[0];
                SetGadgetMaster(jacarandaEmbrace, ranger.AgentItem);
            }
            else if (rangers.Count > 1)
            {
                AttachMasterToGadgetByCastData(castData, jacarandaEmbrace, new List<long> { 44980 }, 1000);
            }
        }

    }
}
