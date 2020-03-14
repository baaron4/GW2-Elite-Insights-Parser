using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class WarriorHelper : ProfHelper
    {

        private static void AttachMasterToBanner(Dictionary<long, List<AbstractCastEvent>> castData, HashSet<AgentItem> banners, long id)
        {
            var possibleCandidates = new HashSet<AgentItem>();
            if (castData.TryGetValue(id, out List<AbstractCastEvent> bannerCast))
            {
                foreach (AbstractCastEvent castEvent in bannerCast)
                {
                    long start = castEvent.Time;
                    long end = start + castEvent.ActualDuration;
                    possibleCandidates.Add(castEvent.Caster);
                    foreach (AgentItem banner in banners)
                    {
                        if (banner.FirstAware >= start && banner.FirstAware <= end + 1000)
                        {
                            // more than one candidate, put to unknown and drop the search
                            if (banner.Master != null)
                            {
                                banner.SetMaster(GeneralHelper.UnknownAgent);
                                break;
                            }
                            banner.SetMaster(castEvent.Caster);
                        }
                    }
                }
            }
            if (possibleCandidates.Count == 1)
            {
                foreach (AgentItem banner in banners)
                {
                    if (banner.Master == null)
                    {
                        banner.SetMaster(possibleCandidates.First());
                    }
                }
            } 
        }

        private static HashSet<AgentItem> GetBannerAgents(Dictionary<long, List<AbstractBuffEvent>> buffData, long id)
        {
            if (buffData.TryGetValue(id, out List<AbstractBuffEvent> list))
            {
                return new HashSet<AgentItem>(list.Where(x => x is BuffApplyEvent && x.By.Type == AgentItem.AgentType.Gadget).Select(x => x.By));
            }
            return new HashSet<AgentItem>();
        }

        private static void SetBannerMaster(HashSet<AgentItem> banners, AgentItem master)
        {
            foreach (AgentItem banner in banners)
            {
                banner.SetMaster(master);
            }
        }

        public static void AttachMasterToBanners(List<Player> players, Dictionary<long, List<AbstractBuffEvent>> buffData, Dictionary<long, List<AbstractCastEvent>> castData)
        {
            HashSet<AgentItem> strBanners = GetBannerAgents(buffData, 14417), defBanners = GetBannerAgents(buffData, 14543), disBanners = GetBannerAgents(buffData, 14449), tacBanners = GetBannerAgents(buffData, 14450);
            var warriors = players.Where(x => x.Prof == "Warrior" || x.Prof == "Spellbreaker" || x.Prof == "Berserker").ToList();
            // if only one warrior, could only be that one
            if (warriors.Count == 1)
            {
                Player warrior = warriors[0];
                SetBannerMaster(strBanners, warrior.AgentItem);
                SetBannerMaster(disBanners, warrior.AgentItem);
                SetBannerMaster(tacBanners, warrior.AgentItem);
                SetBannerMaster(defBanners, warrior.AgentItem);
            } else if (warriors.Count > 1)
            {
                AttachMasterToBanner(castData, strBanners, 14405);
                AttachMasterToBanner(castData, defBanners, 14528);
                AttachMasterToBanner(castData, disBanners, 14407);
                AttachMasterToBanner(castData, tacBanners, 14408);
            }
        }

    }
}
