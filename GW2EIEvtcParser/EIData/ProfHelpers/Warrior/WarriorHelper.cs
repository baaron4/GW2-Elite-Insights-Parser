using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class WarriorHelper : ProfHelper
    {

        private static HashSet<AgentItem> GetBannerAgents(Dictionary<long, List<AbstractBuffEvent>> buffData, long id, HashSet<AgentItem> playerAgents)
        {
            if (buffData.TryGetValue(id, out List<AbstractBuffEvent> list))
            {
                return new HashSet<AgentItem>(list.Where(x => x is BuffApplyEvent && x.By.Type == AgentItem.AgentType.Gadget && x.By.Master == null && playerAgents.Contains(x.To.GetFinalMaster())).Select(x => x.By));
            }
            return new HashSet<AgentItem>();
        }

        /*private static HashSet<AgentItem> FindBattleStandards(Dictionary<long, List<AbstractBuffEvent>> buffData, HashSet<AgentItem> playerAgents)
        {
            if (buffData.TryGetValue(725, out List<AbstractBuffEvent> list))
            {
                var battleBannerCandidates = new HashSet<AgentItem>(list.Where(x => x is BuffApplyEvent && x.By.Type == AgentItem.AgentType.Gadget && (playerAgents.Contains(x.To) || playerAgents.Contains(x.To.Master))).Select(x => x.By));
                if (battleBannerCandidates.Count > 0)
                {
                    if (buffData.TryGetValue(740, out list))
                    {
                        battleBannerCandidates.IntersectWith(new HashSet<AgentItem>(list.Where(x => x is BuffApplyEvent && x.By.Type == AgentItem.AgentType.Gadget && (playerAgents.Contains(x.To) || playerAgents.Contains(x.To.Master))).Select(x => x.By)));
                        if (battleBannerCandidates.Count > 0)
                        {
                            if (buffData.TryGetValue(719, out list))
                            {
                                battleBannerCandidates.IntersectWith(new HashSet<AgentItem>(list.Where(x => x is BuffApplyEvent && x.By.Type == AgentItem.AgentType.Gadget && (playerAgents.Contains(x.To) || playerAgents.Contains(x.To.Master))).Select(x => x.By)));
                                return battleBannerCandidates;
                            }
                        }
                    }
                }
            }
            return new HashSet<AgentItem>();
        }*/

        public static void AttachMasterToWarriorBanners(List<Player> players, Dictionary<long, List<AbstractBuffEvent>> buffData, Dictionary<long, List<AbstractCastEvent>> castData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            HashSet<AgentItem> strBanners = GetBannerAgents(buffData, 14417, playerAgents),
                defBanners = GetBannerAgents(buffData, 14543, playerAgents),
                disBanners = GetBannerAgents(buffData, 14449, playerAgents),
                tacBanners = GetBannerAgents(buffData, 14450, playerAgents);
                //battleBanner = FindBattleStandards(buffData, playerAgents);
            var warriors = players.Where(x => x.Prof == "Warrior" || x.Prof == "Spellbreaker" || x.Prof == "Berserker").ToList();
            // if only one warrior, could only be that one
            if (warriors.Count == 1)
            {
                Player warrior = warriors[0];
                SetGadgetMaster(strBanners, warrior.AgentItem);
                SetGadgetMaster(disBanners, warrior.AgentItem);
                SetGadgetMaster(tacBanners, warrior.AgentItem);
                SetGadgetMaster(defBanners, warrior.AgentItem);
                //SetBannerMaster(battleBanner, warrior.AgentItem);
            }
            else if (warriors.Count > 1)
            {
                // land and under water cast ids
                AttachMasterToGadgetByCastData(castData, strBanners, new List<long> { 14405, 14572 }, 1000);
                AttachMasterToGadgetByCastData(castData, defBanners, new List<long> { 14528, 14570 }, 1000);
                AttachMasterToGadgetByCastData(castData, disBanners, new List<long> { 14407, 14571 }, 1000);
                AttachMasterToGadgetByCastData(castData, tacBanners, new List<long> { 14408, 14573 }, 1000);
                //AttachMasterToBanner(castData, battleBanner, 14419, 14569);
            }
        }

    }
}
