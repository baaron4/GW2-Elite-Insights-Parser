using System.Collections.Generic;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class GuardianHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(ShieldOfWrathSkill, ShieldOfWrathEffect), // Shield of Wrath
            new BuffGainCastFinder(ZealotsFlameSkill, ZealotsFlameEffect).UsingICD(0), // Zealot's Flame
            //new BuffLossCastFinder(9115,9114,InstantCastFinder.DefaultICD), // Virtue of Justice
            //new BuffLossCastFinder(9120,9119,InstantCastFinder.DefaultICD), // Virtue of Resolve
            //new BuffLossCastFinder(9118,9113,InstantCastFinder.DefaultICD), // Virtue of Courage
            new DamageCastFinder(JudgesIntervention,JudgesIntervention), // Judge's Intervention
            new DamageCastFinder(WrathOfJustice,WrathOfJustice), // Wrath of Justice
            new DamageCastFinder(SmitersBoon,SmitersBoon), // Smiter's Boon
            new DamageCastFinder(SmiteCondition,SmiteCondition), // Smite Condition
            //new DamageCastFinder(9097,9097), // Symbol of Blades
            new DamageCastFinder(GlacialHeart, GlacialHeart), // Glacial Heart
            new DamageCastFinder(ShatteredAegis, ShatteredAegis), // Shattered Aegis
            new EXTHealingCastFinder(SelflessDaring, SelflessDaring), // Selfless Daring
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Zeal
            new BuffDamageModifierTarget(Burning, "Fiery Wrath", "7% on burning target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/7/70/Fiery_Wrath.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(Vulnerability, "Symbolic Exposure", "5% on vuln target", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/c/cd/Symbolic_Exposure.png", DamageModifierMode.All),
            new BuffDamageModifier(SymbolicAvenger, "Symbolic Avenger", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, "https://wiki.guildwars2.com/images/e/e5/Symbolic_Avenger.png", DamageModifierMode.All).WithBuilds(GW2Builds.July2019Balance),
            // Radiance
            new BuffDamageModifier(Retaliation, "Retribution", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/d/d7/Retribution_%28trait%29.png", DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(Resolution, "Retribution", "10% under resolution", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/d/d7/Retribution_%28trait%29.png", DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            // Virtues
            new BuffDamageModifier(Aegis, "Unscathed Contender", "20% under aegis", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/b/b4/Unscathed_Contender.png", DamageModifierMode.All),
            new BuffDamageModifier(NumberOfBoons, "Power of the Virtuous", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, "https://wiki.guildwars2.com/images/e/ee/Power_of_the_Virtuous.png",  DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(NumberOfBoons, "Inspired Virtue", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, "https://wiki.guildwars2.com/images/c/c7/Inspired_Virtue.png", DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifier(InspiringVirtue, "Inspiring Virtue", "10% (6s) after activating a virtue ", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, "https://wiki.guildwars2.com/images/8/8f/Virtuous_Solace.png", DamageModifierMode.All).WithBuilds(GW2Builds.February2020Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {        
                //skills
                new Buff("Zealot's Flame", ZealotsFlameEffect, Source.Guardian, BuffStackType.Queue, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7a/Zealot%27s_Flame.png"),
                new Buff("Purging Flames",PurgingFlames, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/28/Purging_Flames.png"),
                new Buff("Litany of Wrath",LitanyOfWrath, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4a/Litany_of_Wrath.png"),
                new Buff("Renewed Focus",RenewedFocus, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/10/Renewed_Focus.png"),
                new Buff("Shield of Wrath",ShieldOfWrathEffect, Source.Guardian, BuffStackType.Stacking, 3, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/bc/Shield_of_Wrath.png"),
                new Buff("Binding Blade (Self)",BindingBladeSelf, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/de/Binding_Blade.png"),
                new Buff("Binding Blade",BindingBlade, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/de/Binding_Blade.png"),
                //signets
                new Buff("Signet of Resolve",SignetOfResolve, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/75/Signet_of_Resolve.png"),
                new Buff("Signet of Resolve (Shared)", SignetOfResolveShared, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/7/75/Signet_of_Resolve.png", GW2Builds.StartOfLife, GW2Builds.June2022Balance),
                new Buff("Signet of Resolve (PI)", SignetOfResolvePI, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/75/Signet_of_Resolve.png", GW2Builds.June2022Balance, GW2Builds.EndOfLife),
                new Buff("Bane Signet",BaneSignet, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png"),
                new Buff("Bane Signet (PI)",BaneSignetPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png", GW2Builds.StartOfLife, GW2Builds.June2022Balance),
                new Buff("Bane Signet (PI)",BaneSignetPI, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/56/Bane_Signet.png", GW2Builds.June2022Balance, GW2Builds.EndOfLife),
                new Buff("Signet of Judgment",SignetOfJudgment, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png"),
                new Buff("Signet of Judgment (PI)",SignetOfJudgmentPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png", GW2Builds.StartOfLife, GW2Builds.June2022Balance),
                new Buff("Signet of Judgment (PI)",SignetOfJudgmentPI, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/fe/Signet_of_Judgment.png", GW2Builds.June2022Balance, GW2Builds.EndOfLife),
                new Buff("Signet of Mercy",SignetOfMercy, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png"),
                new Buff("Signet of Mercy (PI)",SignetOfMercyPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png", GW2Builds.StartOfLife, GW2Builds.June2022Balance),
                new Buff("Signet of Mercy (PI)",SignetOfMercyPI, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/37/Signet_of_Mercy.png", GW2Builds.June2022Balance, GW2Builds.EndOfLife),
                new Buff("Signet of Wrath",SignetOfWrath, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png"),
                new Buff("Signet of Wrath (PI)",SignetOfWrathPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png", GW2Builds.StartOfLife, GW2Builds.June2022Balance),
                new Buff("Signet of Wrath (PI)",SignetOfWrathPI, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/18/Signet_of_Wrath.png", GW2Builds.June2022Balance, GW2Builds.EndOfLife),
                new Buff("Signet of Courage",SignetOfCourage, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5d/Signet_of_Courage.png"),
                new Buff("Signet of Courage (Shared)",SignetOfCourageShared , Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/5/5d/Signet_of_Courage.png", GW2Builds.StartOfLife, GW2Builds.June2022Balance),
                new Buff("Signet of Courage (PI)",SignetOfCouragePI , Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5d/Signet_of_Courage.png", GW2Builds.June2022Balance, GW2Builds.EndOfLife),
                //virtues
                new Buff("Virtue of Justice", VirtueOfJustice, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/10/Virtue_of_Justice.png"),
                new Buff("Virtue of Courage", VirtueOfCourage, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a9/Virtue_of_Courage.png"),
                new Buff("Virtue of Resolve", VirtueOfResolve, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b2/Virtue_of_Resolve.png"),
                new Buff("Justice", Justice, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/10/Virtue_of_Justice.png"),
                //traits
                new Buff("Strength in Numbers",StrengthinNumbers, Source.Guardian, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/7/7b/Strength_in_Numbers.png", GW2Builds.StartOfLife, GW2Builds.June2022Balance),
                new Buff("Invigorated Bulwark",InvigoratedBulwark, Source.Guardian, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/00/Invigorated_Bulwark.png"),
                new Buff("Virtue of Resolve (Battle Presence)", VirtueOfResolveBattlePresence, Source.Guardian, BuffStackType.Queue, 2, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/2/27/Battle_Presence.png"),
                new Buff("Virtue of Resolve (Battle Presence - Absolute Resolve)", VirtueOfResolveBattlePresenceAbsoluteResolve, Source.Guardian, BuffStackType.Queue, 2, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/b/b2/Virtue_of_Resolve.png"),
                new Buff("Symbolic Avenger",SymbolicAvenger, Source.Guardian, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e5/Symbolic_Avenger.png", GW2Builds.July2019Balance, GW2Builds.EndOfLife),
                new Buff("Inspiring Virtue",InspiringVirtue, Source.Guardian, BuffStackType.Queue, 99, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8f/Virtuous_Solace.png", GW2Builds.February2020Balance, 102389),
                new Buff("Inspiring Virtue",InspiringVirtue, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8f/Virtuous_Solace.png", 102389, GW2Builds.EndOfLife),
                new Buff("Force of Will",ForceOfWill, Source.Guardian, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d2/Force_of_Will.png"),
        };

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.BowOfTruth,
            (int)MinionID.HammerOfWisdom,
            (int)MinionID.ShieldOfTheAvenger,
            (int)MinionID.SwordOfJustice,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }

    }
}
