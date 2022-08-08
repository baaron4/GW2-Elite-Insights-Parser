using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ThiefHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(Shadowstep,Infiltration), // Shadowstep
            new BuffLossCastFinder(ShadowReturn,Infiltration).UsingChecker((evt, combatData) => evt.RemovedDuration > ServerDelayConstant), // Shadow Return
            new DamageCastFinder(Mug, Mug), // Mug
            new BuffGainCastFinder(AssassinsSignet,AssassinsSignetActive), // Assassin's Signet
            new BuffGiveCastFinder(DevourerVenomSkill,DevourerVenomEffect), // Devourer Venom
            new BuffGiveCastFinder(IceDrakeVenomSkill,IceDrakeVenomEffect), // Ice Drake Venom
            new BuffGiveCastFinder(SkaleVenomSkill,SkaleVenomEffect), // Skale Venom
            new BuffGiveCastFinder(SoulStoneVenomSkill,SoulStoneVenomEffect), // Soul Stone Venom
            //new BuffGiveCastFinder(13037,13036,InstantCastFinder.DefaultICD), // Spider Venom - same id as leeching venom trait?
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Deadly arts
            new BuffDamageModifierTarget(NumberOfConditions, "Exposed Weakness", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Thief, ByStack, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png", DamageModifierMode.All).WithBuilds(GW2Builds.July2018Balance),
            new BuffDamageModifierTarget(NumberOfConditions, "Exposed Weakness", "10% if condition on target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Thief, ByPresence, "https://wiki.guildwars2.com/images/0/02/Exposed_Weakness.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2018Balance),
            new DamageLogDamageModifier("Executioner", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Thief,"https://wiki.guildwars2.com/images/9/93/Executioner.png", (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            // Critical Strikes
            new DamageLogDamageModifier("Twin Fangs","7% over 90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", (x, log) => x.IsOverNinety && x.HasCrit, ByPresence, DamageModifierMode.All),
            new DamageLogDamageModifier("Ferocious Strikes", "10% on critical strikes if target >50%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Thief,"https://wiki.guildwars2.com/images/d/d1/Ferocious_Strikes.png", (x, log) => !x.AgainstUnderFifty && x.HasCrit, ByPresence, DamageModifierMode.All),
            // Trickery
            new BuffDamageModifier(LeadAttacks, "Lead Attacks", "1% (10s) per initiative spent", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Thief, ByStack, "https://wiki.guildwars2.com/images/0/01/Lead_Attacks.png", DamageModifierMode.All), // It's not always possible to detect the presence of pistol and the trait is additive with itself. Staff master is worse as we can't detect endurance at all
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                //signets
                new Buff("Signet of Malice",SignetOfMalice, Source.Thief, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/ae/Signet_of_Malice.png"),
                new Buff("Assassin's Signet (Passive)",AssassinsSignetPassive, Source.Thief, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/23/Assassin%27s_Signet.png"),
                new Buff("Assassin's Signet (Active)",AssassinsSignetActive, Source.Thief, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/23/Assassin%27s_Signet.png"),
                new Buff("Infiltrator's Signet",InfiltratorsSignet, Source.Thief, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8e/Infiltrator%27s_Signet.png"),
                new Buff("Signet of Agility",SignetOfAgility, Source.Thief, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/1d/Signet_of_Agility.png"),
                new Buff("Signet of Shadows",SignetOfShadows, Source.Thief, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/17/Signet_of_Shadows.png"),
                //venoms // src is always the user, makes generation data useless
                new Buff("Skelk Venom",SkelkVenom, Source.Thief, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/7/75/Skelk_Venom.png"),
                new Buff("Ice Drake Venom",IceDrakeVenomEffect, Source.Thief, BuffStackType.StackingConditionalLoss, 4, BuffClassification.Support, "https://wiki.guildwars2.com/images/7/7b/Ice_Drake_Venom.png"),
                new Buff("Devourer Venom", DevourerVenomEffect, Source.Thief, BuffStackType.StackingConditionalLoss, 2, BuffClassification.Support, "https://wiki.guildwars2.com/images/4/4d/Devourer_Venom.png"),
                new Buff("Skale Venom", SkaleVenomEffect, Source.Thief, BuffStackType.StackingConditionalLoss, 4, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/1/14/Skale_Venom.png"),
                new Buff("Spider Venom",SpiderVenom, Source.Thief, BuffStackType.StackingConditionalLoss, 6, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/3/39/Spider_Venom.png"),
                new Buff("Soul Stone Venom",SoulStoneVenomEffect, Source.Thief, BuffStackType.Stacking, 25, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/d/d6/Soul_Stone_Venom.png"),
                new Buff("Basilisk Venom", BasiliskVenom, Source.Thief, BuffStackType.StackingConditionalLoss, 2, BuffClassification.Support, "https://wiki.guildwars2.com/images/3/3a/Basilisk_Venom.png"),
                new Buff("Infiltration",Infiltration, Source.Thief, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/25/Shadowstep.png"),
                //transforms
                new Buff("Dagger Storm",DaggerStorm, Source.Thief, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c0/Dagger_Storm.png"),
                //traits
                new Buff("Hidden Killer",HiddenKiller, Source.Thief, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/ec/Hidden_Killer.png"),
                new Buff("Lead Attacks",LeadAttacks, Source.Thief, BuffStackType.Stacking, 15, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/01/Lead_Attacks.png"),
                new Buff("Instant Reflexes",InstantReflexes, Source.Thief, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Instant_Reflexes.png"),

        };

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.Thief1,
            (int)MinionID.Thief2,
            (int)MinionID.Thief3,
            (int)MinionID.Thief4,
            (int)MinionID.Thief5,
            (int)MinionID.Thief6,
            (int)MinionID.Thief7,
            (int)MinionID.Thief8,
            (int)MinionID.Thief9,
            (int)MinionID.Thief10,
            (int)MinionID.Thief11,
            (int)MinionID.Thief12,
            (int)MinionID.Thief13,
            (int)MinionID.Thief14,
            (int)MinionID.Thief15,
            (int)MinionID.Thief16,
            (int)MinionID.Thief17,
            (int)MinionID.Thief18,
            (int)MinionID.Thief19,
            (int)MinionID.Thief20,
            (int)MinionID.Thief21,
            (int)MinionID.Thief22,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }

    }
}
