using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class EngineerHelper : ProfHelper
    {

        public static void AttachMasterToEngineerTurrets(List<Player> players, Dictionary<long, List<AbstractDamageEvent>> damageData, Dictionary<long, List<AbstractCastEvent>> castData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));

            HashSet<AgentItem> flameTurrets = GetOffensiveGadgetAgents(damageData, 5903, playerAgents);

            HashSet<AgentItem> rifleTurrets = GetOffensiveGadgetAgents(damageData, 5841, playerAgents);
            rifleTurrets.UnionWith(GetOffensiveGadgetAgents(damageData, 5875, playerAgents));

            HashSet<AgentItem> netTurrets = GetOffensiveGadgetAgents(damageData, 5896, playerAgents);
            netTurrets.UnionWith(GetOffensiveGadgetAgents(damageData, 22137, playerAgents));

            HashSet<AgentItem> rocketTurrets = GetOffensiveGadgetAgents(damageData, 6108, playerAgents);
            rocketTurrets.UnionWith(GetOffensiveGadgetAgents(damageData, 5914, playerAgents));

            HashSet<AgentItem> thumperTurrets = GetOffensiveGadgetAgents(damageData, 5856, playerAgents);
            thumperTurrets.UnionWith(GetOffensiveGadgetAgents(damageData, 5890, playerAgents));
            // TODO: need ID here
            HashSet<AgentItem> harpoonTurrets = GetOffensiveGadgetAgents(damageData, -1, playerAgents);

            HashSet<AgentItem> healingTurrets = GetOffensiveGadgetAgents(damageData, 5958, playerAgents);
            healingTurrets.RemoveWhere(x => thumperTurrets.Contains(x) || rocketTurrets.Contains(x) || netTurrets.Contains(x) || rifleTurrets.Contains(x) || flameTurrets.Contains(x) || harpoonTurrets.Contains(x));

            var engineers = players.Where(x => x.Prof == "Engineer" || x.Prof == "Scrapper" || x.Prof == "Holosmith").ToList();
            // if only one engineer, could only be that one
            if (engineers.Count == 1)
            {
                Player engineer = engineers[0];
                SetGadgetMaster(flameTurrets, engineer.AgentItem);
                SetGadgetMaster(netTurrets, engineer.AgentItem);
                SetGadgetMaster(rocketTurrets, engineer.AgentItem);
                SetGadgetMaster(rifleTurrets, engineer.AgentItem);
                SetGadgetMaster(thumperTurrets, engineer.AgentItem);
                SetGadgetMaster(harpoonTurrets, engineer.AgentItem);
                SetGadgetMaster(healingTurrets, engineer.AgentItem);
            }
            else if (engineers.Count > 1)
            {
                AttachMasterToGadgetByCastData(castData, flameTurrets, new List<long> { 5836, 5868 }, 1000);
                AttachMasterToGadgetByCastData(castData, rifleTurrets, new List<long> { 5818 }, 1000);
                AttachMasterToGadgetByCastData(castData, netTurrets, new List<long> { 5837, 5868, 6183 }, 1000);
                AttachMasterToGadgetByCastData(castData, rocketTurrets, new List<long> { 5912, 22574, 6183 }, 1000);
                AttachMasterToGadgetByCastData(castData, thumperTurrets, new List<long> { 5838 }, 1000);
                //AttachMasterToGadgetByCastData(castData, harpoonTurrets, new List<long> { 6093, 6183 }, 1000);
                //AttachMasterToGadgetByCastData(castData, healingTurrets, new List<long> { 5857, 5868 }, 1000);
            }
        }

    }
}
