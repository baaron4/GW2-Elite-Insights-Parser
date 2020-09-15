using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

namespace GW2EIEvtcParser.EIData
{
    internal static class WarriorHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(14268, 14268, EIData.InstantCastFinder.DefaultICD, 84794, ulong.MaxValue), // Reckless Impact
            new BuffGainCastFinder(14406, 14453, EIData.InstantCastFinder.DefaultICD), // Berserker Stance
            new BuffGainCastFinder(14412, 34778, EIData.InstantCastFinder.DefaultICD), // Balanced Stance
            new BuffGainCastFinder(14392, 787, EIData.InstantCastFinder.DefaultICD), // Endure Pain
        };

        private static HashSet<AgentItem> GetBannerAgents(Dictionary<long, List<AbstractBuffEvent>> buffData, long id, HashSet<AgentItem> playerAgents)
        {
            if (buffData.TryGetValue(id, out List<AbstractBuffEvent> list))
            {
                return new HashSet<AgentItem>(list.Where(x => x is BuffApplyEvent && x.By.Type == AgentItem.AgentType.Gadget && x.By.Master == null && playerAgents.Contains(x.To.GetFinalMaster())).Select(x => x.By));
            }
            return new HashSet<AgentItem>();
        }


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(46853, "Peak Performance (absent)", "5%", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByAbsence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 90455, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(46853, "Peak Performance (present)", "20%", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 90455, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(46853, "Peak Performance (absent)", "3%", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByAbsence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 90455, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(46853, "Peak Performance (present)", "10%", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 90455, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(46853, "Peak Performance", "33%", DamageSource.NoPets, 33.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png", 0, 90455, DamageModifierMode.PvE),
            new BuffDamageModifier(719, "Warrior's Sprint", "7% under swiftness", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/e/e3/Warrior%27s_Sprint.png", 86181 , ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(719, "Warrior's Sprint", "3% under swiftness", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/e/e3/Warrior%27s_Sprint.png", 86181 , ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(742, "Cull the Weak", "7% on weakened target", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/7/72/Cull_the_Weak.png", DamageModifierMode.All),
            new BuffDamageModifier(NumberOfBoonsID, "Empowered", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByStack, "https://wiki.guildwars2.com/images/c/c2/Empowered.png", DamageModifierMode.All),
            new BuffDamageModifier(42539, "Berserker's Power", "7% per stack", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByStack, "https://wiki.guildwars2.com/images/6/6f/Berserker%27s_Power.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(NumberOfBoonsID, "Destruction of the Empowered", "3% per target boon", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByStack, "https://wiki.guildwars2.com/images/5/5c/Destruction_of_the_Empowered.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(new long[] {721, 727, 722}, "Leg Specialist", "7% to movement-impaired foes", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParserHelper.Source.Warrior, ByPresence, "https://wiki.guildwars2.com/images/9/9e/Leg_Specialist.png", 99526, ulong.MaxValue, DamageModifierMode.All)

        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            //skills
                new Buff("Riposte",14434, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/d/de/Riposte.png"),
                //signets
                new Buff("Healing Signet",786, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/8/85/Healing_Signet.png"),
                new Buff("Dolyak Signet",14458, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/60/Dolyak_Signet.png"),
                new Buff("Signet of Fury",14459, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c1/Signet_of_Fury.png"),
                new Buff("Signet of Might",14444, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/40/Signet_of_Might.png"),
                new Buff("Signet of Stamina",14478, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/6b/Signet_of_Stamina.png"),
                new Buff("Signet of Rage",14496, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/bc/Signet_of_Rage.png"),
                //banners
                new Buff("Banner of Strength", 14417, ParserHelper.Source.Warrior, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/e/e1/Banner_of_Strength.png/33px-Banner_of_Strength.png"),
                new Buff("Banner of Discipline", 14449, ParserHelper.Source.Warrior, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/5/5f/Banner_of_Discipline.png/33px-Banner_of_Discipline.png"),
                new Buff("Banner of Tactics",14450, ParserHelper.Source.Warrior, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/thumb/3/3f/Banner_of_Tactics.png/33px-Banner_of_Tactics.png"),
                new Buff("Banner of Defense",14543, ParserHelper.Source.Warrior, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/f/f1/Banner_of_Defense.png/33px-Banner_of_Defense.png"),
                //stances
                new Buff("Shield Stance",756, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/d/de/Shield_Stance.png"),
                new Buff("Berserker's Stance",14453, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/8/8a/Berserker_Stance.png"),
                new Buff("Enduring Pain",787, ParserHelper.Source.Warrior, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/24/Endure_Pain.png"),
                new Buff("Balanced Stance",34778, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/27/Balanced_Stance.png"),
                new Buff("Defiant Stance",21816, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/db/Defiant_Stance.png"),
                new Buff("Rampage",14484, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e4/Rampage.png"),
                //traits
                new Buff("Soldier's Focus", 58102, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/29/Soldier%27s_Focus.png", 99526, ulong.MaxValue),
                new Buff("Brave Stride", 43063, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b8/Death_from_Above.png"),
                new Buff("Empower Allies", 14222, ParserHelper.Source.Warrior, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/4/4c/Empower_Allies.png/20px-Empower_Allies.png"),
                new Buff("Peak Performance",46853, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/98/Peak_Performance.png"),
                new Buff("Furious Surge", 30204, ParserHelper.Source.Warrior, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/65/Furious.png"),
                //new Boon("Health Gain per Adrenaline bar Spent",-1, BoonSource.Warrior, BoonType.Intensity, 3, BoonEnum.GraphOnlyBuff,RemoveType.Normal),
                new Buff("Rousing Resilience",24383, ParserHelper.Source.Warrior, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ca/Rousing_Resilience.png"),
                new Buff("Berserker's Power",42539, ParserHelper.Source.Warrior, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/6f/Berserker%27s_Power.png"),
        };


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
                ProfHelper.SetGadgetMaster(strBanners, warrior.AgentItem);
                ProfHelper.SetGadgetMaster(disBanners, warrior.AgentItem);
                ProfHelper.SetGadgetMaster(tacBanners, warrior.AgentItem);
                ProfHelper.SetGadgetMaster(defBanners, warrior.AgentItem);
                //SetBannerMaster(battleBanner, warrior.AgentItem);
            }
            else if (warriors.Count > 1)
            {
                // land and under water cast ids
                ProfHelper.AttachMasterToGadgetByCastData(castData, strBanners, new List<long> { 14405, 14572 }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(castData, defBanners, new List<long> { 14528, 14570 }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(castData, disBanners, new List<long> { 14407, 14571 }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(castData, tacBanners, new List<long> { 14408, 14573 }, 1000);
                //AttachMasterToBanner(castData, battleBanner, 14419, 14569);
            }
        }

    }
}
