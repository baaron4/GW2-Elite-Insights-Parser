using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class RangerHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            //new DamageCastFinder(12573,12573,EIData.InstantCastFinder.DefaultICD), // Hunter's Shot
            //new DamageCastFinder(12507,12507,EIData.InstantCastFinder.DefaultICD), // Crippling Shot
            new BuffGainCastFinder(12633,33902,EIData.InstantCastFinder.DefaultICD), // "Sic 'Em!"
            new BuffGainCastFinder(12633,56923,EIData.InstantCastFinder.DefaultICD), // "Sic 'Em!" PvP
            new BuffGainCastFinder(12500,12543,EIData.InstantCastFinder.DefaultICD, (evt, combatData) => Math.Abs(evt.AppliedDuration - 6000) < ServerDelayConstant), // Signet of Stone
            new BuffGainCastFinder(42470,12543,EIData.InstantCastFinder.DefaultICD, (evt, combatData) => Math.Abs(evt.AppliedDuration - 5000) < ServerDelayConstant), // Lesser Signet of Stone
            new BuffGainCastFinder(12537,12536,EIData.InstantCastFinder.DefaultICD), // Sharpening Stone
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Skills
            new BuffDamageModifier(33902, "Sic 'Em!", "40%", DamageSource.NoPets, 40.0, DamageType.Power, DamageType.All, Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png", 0, 114788, DamageModifierMode.PvE, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(33902).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By;
                }
                return false;
            }),
            new BuffDamageModifier(33902, "Sic 'Em!", "25%", DamageSource.NoPets, 25.0, DamageType.Power, DamageType.All, Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png", 0, 114788, DamageModifierMode.sPvPWvW, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(33902).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By;
                }
                return false;
            }),
            new BuffDamageModifier(33902, "Sic 'Em!", "25%", DamageSource.NoPets, 25.0, DamageType.Power, DamageType.All, Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png", 114788, ulong.MaxValue, DamageModifierMode.All, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(33902).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By;
                }
                return false;
            }),
            // Marksmanship
            new DamageLogApproximateDamageModifier("Farsighted (<= 600)", "5% with weapon skills below 600 range", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, Source.Ranger, "https://wiki.guildwars2.com/images/2/2f/Steady_Focus.png", (x, log) => {
                if (!x.Skill.IsWeaponSkill)
                {
                    return false;
                }
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 600.0;
            }, ByPresence, 90455, ulong.MaxValue, DamageModifierMode.All),
            new DamageLogApproximateDamageModifier("Farsighted (> 600)", "10% with weapon skills above 600 range", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, Source.Ranger, "https://wiki.guildwars2.com/images/2/2f/Steady_Focus.png", (x, log) => {
                if (!x.Skill.IsWeaponSkill)
                {
                    return false;
                }
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) > 600.0;
            }, ByPresence, 90455, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifierTarget(new long[] { 872, 833, 721, 727, 791, 722, 27705}, "Predator's Onslaught", "15% to disabled or movement-impaired foes", DamageSource.All, 15.0, DamageType.Power, DamageType.All, Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/a/ac/Predator%27s_Onslaught.png", DamageModifierMode.All),
            // Skirmishing
            new DamageLogDamageModifier("Hunter's Tactics", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, Source.Ranger,"https://wiki.guildwars2.com/images/b/bb/Hunter%27s_Tactics.png", (x, log) => x.IsFlanking , ByPresence, 102321, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(30673, "Light on your Feet", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/2/22/Light_on_your_Feet.png", DamageModifierMode.All),
            // Nature Magic
            // We can't check buffs on minions yet
            new BuffDamageModifier(NumberOfBoonsID, "Bountiful Hunter", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, Source.Ranger, ByStack, "https://wiki.guildwars2.com/images/2/25/Bountiful_Hunter.png", DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Counterattack",14509, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c1/Counterattack.png"),
                //signets
                new Buff("Signet of Renewal",41147, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/11/Signet_of_Renewal.png"),
                new Buff("Signet of Stone (Passive)",12627, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Buff("Signet of the Hunt (Passive)",12626, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                new Buff("Signet of the Wild",12518, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Buff("Signet of the Wild (Pet)",12636, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Buff("Signet of Stone (Active)",12543, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Buff("Signet of the Hunt (Active)",12541, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                //spirits
                // new Boon("Water Spirit (old)", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Buff("Frost Spirit", 12544, Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 0, 88541),
                new Buff("Sun Spirit", 12540, Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png", 0, 88541),
                new Buff("Stone Spirit", 12547, Source.Ranger, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png", 0, 88541),
                //new Boon("Storm Spirit (old)", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //reworked
                new Buff("Water Spirit", 50386, Source.Ranger, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Frost Spirit", 50421, Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Sun Spirit", 50413, Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Stone Spirit", 50415, Source.Ranger, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Storm Spirit", 50381, Source.Ranger, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png", 88541, ulong.MaxValue),
                //skills
                new Buff("Attack of Opportunity",12574, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/47/Moment_of_Clarity.png"),
                new Buff("Call of the Wild",36781, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png",0 , 97950),
                new Buff("Call of the Wild",36781, Source.Ranger, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png",97950 , 102321),
                new Buff("Strength of the Pack!",12554, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4b/%22Strength_of_the_Pack%21%22.png"),
                new Buff("Sic 'Em!",33902, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
                new Buff("Sic 'Em! (PvP)",56923, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
                new Buff("Sharpening Stones",12536, Source.Ranger, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/af/Sharpening_Stone.png"),
                new Buff("Sharpen Spines",43266, Source.Ranger, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/95/Sharpen_Spines.png"),
                //traits
                new Buff("Spotter", 14055, Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/b/b0/Spotter.png"),
                new Buff("Opening Strike",13988, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/44/Opening_Strike_%28effect%29.png"),
                new Buff("Quick Draw",29703, Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/39/Quick_Draw.png"),
                new Buff("Light on your Feet",30673, Source.Ranger, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Light_on_your_Feet.png"),
        };


        public static void AttachMasterToRangerGadgets(List<Player> players, Dictionary<long, List<AbstractHealthDamageEvent>> damageData, Dictionary<long, List<AnimatedCastEvent>> castData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // entangle works fine already
            HashSet<AgentItem> jacarandaEmbraces = ProfHelper.GetOffensiveGadgetAgents(damageData, 1286, playerAgents);
            HashSet<AgentItem> blackHoles = ProfHelper.GetOffensiveGadgetAgents(damageData, 31436, playerAgents);
            var rangers = players.Where(x => x.Prof == "Ranger" || x.Prof == "Soulbeast" || x.Prof == "Druid").ToList();
            // if only one ranger, could only be that one
            if (rangers.Count == 1)
            {
                Player ranger = rangers[0];
                ProfHelper.SetGadgetMaster(jacarandaEmbraces, ranger.AgentItem);
                ProfHelper.SetGadgetMaster(blackHoles, ranger.AgentItem);
            }
            else if (rangers.Count > 1)
            {
                ProfHelper.AttachMasterToGadgetByCastData(castData, jacarandaEmbraces, new List<long> { 44980 }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(castData, blackHoles, new List<long> { 31503 }, 1000);
            }
        }

    }
}
