using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class RangerHelper : ProfHelper
    {

        public static void AttachMasterToRangerGadgets(List<Player> players, Dictionary<long, List<AbstractDamageEvent>> damageData, Dictionary<long, List<AbstractCastEvent>> castData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // entangle works fine already
            HashSet<AgentItem> jacarandaEmbraces = GetOffensiveGadgetAgents(damageData, 1286, playerAgents);
            HashSet<AgentItem> blackHoles = GetOffensiveGadgetAgents(damageData, 31436, playerAgents);
            var rangers = players.Where(x => x.Prof == "Ranger" || x.Prof == "Soulbeast" || x.Prof == "Druid").ToList();
            // if only one ranger, could only be that one
            if (rangers.Count == 1)
            {
                Player ranger = rangers[0];
                SetGadgetMaster(jacarandaEmbraces, ranger.AgentItem);
                SetGadgetMaster(blackHoles, ranger.AgentItem);
            }
            else if (rangers.Count > 1)
            {
                AttachMasterToGadgetByCastData(castData, jacarandaEmbraces, new List<long> { 44980 }, 1000);
                AttachMasterToGadgetByCastData(castData, blackHoles, new List<long> { 31503 }, 1000);
            }
        }

    }
}
