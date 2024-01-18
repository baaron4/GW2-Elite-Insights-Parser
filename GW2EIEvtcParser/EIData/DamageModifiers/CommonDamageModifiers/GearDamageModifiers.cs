using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class GearDamageModifiers
    {
        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Runes
            new DamageLogDamageModifier("Scholar Rune", "5% if hp >=90%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheScholar, (x, log) => x.IsOverNinety, DamageModifierMode.All)
                .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new DamageLogDamageModifier("Scholar Rune", "10% if hp >=90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheScholar, (x, log) => x.IsOverNinety, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune),
            new DamageLogDamageModifier("Eagle Rune", "10% if target <50% HP", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheEagle, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All)
                .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new DamageLogDamageModifier("Eagle Rune", "6% if target <50% HP", DamageSource.NoPets, 6.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheEagle, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune),
            new DamageLogDamageModifier("Thief Rune", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheThief, (x, log) => x.IsFlanking, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Thief Rune", "10% while flanking or against defiant", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheThief, (x, log) => x.IsFlanking || x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None, DamageModifierMode.All)
                .WithBuilds(GW2Builds.June2023Balance, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnActorDamageModifier(Might, "Strength Rune", "5% under might", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfStrength, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnActorDamageModifier(FireAura, "Fire Rune", "10% under fire aura", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfFire, DamageModifierMode.All)
                .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnFoeDamageModifier(Burning, "Flame Legion Rune", "7% on burning target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfTheFlameLegion, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnFoeDamageModifier(NumberOfBoons, "Spellbreaker Rune", "7% on boonless target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByAbsence, BuffImages.SuperiorRuneOfTheSpellbreaker, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnFoeDamageModifier(Chilled, "Ice Rune", "7% on chilled target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfTheIce, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnActorDamageModifier(Fury, "Rage Rune", "5% under fury", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfRage, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnFoeDamageModifier(Daze, "Rune of the Mesmer", "10% on dazed target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfTheMesmer, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            // Sigils
            new BuffOnFoeDamageModifier(new long[] { Stun, Knockdown }, "Impact Sigil", "7% on stunned or knocked-down target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorSigilOfImpact, DamageModifierMode.All),
            // Relics
            new BuffOnFoeDamageModifier(RelicOfTheDragonhunterTargetBuff, "Relic of the Dragonhunter", "10% after trap hit", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfTheDragonhunter, DamageModifierMode.All).UsingChecker((x, log) =>
            {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return log.FindActor(dst).HasBuff(log, log.FindActor(src), RelicOfTheDragonhunterTargetBuff, x.Time);
            }).UsingApproximate(true), // Reapplication while buff is running is done via extension, extensions source finding is not capable of always finding the source
            new BuffOnFoeDamageModifier(RelicOfIsgarrenTargetBuff, "Relic of Isgarren", "10% after evade", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfIsgarren, DamageModifierMode.All).UsingChecker((x, log) =>
            {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return log.FindActor(dst).HasBuff(log, log.FindActor(src), RelicOfIsgarrenTargetBuff, x.Time);
            }).UsingApproximate(true), // Reapplication while buff is running is done via extension, extensions source finding is not capable of always finding the source
            new BuffOnFoeDamageModifier(RelicOfPeithaTargetBuff, "Relic of Peitha", "10% after blade hit", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfPeitha, DamageModifierMode.All).UsingChecker((x, log) =>
            {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return log.FindActor(dst).HasBuff(log, log.FindActor(src), RelicOfPeithaTargetBuff, x.Time);
            }).WithBuilds(GW2Builds.November2023Balance).UsingApproximate(true), // Reapplication while buff is running is done via extension, extensions source finding is not capable of always finding the source
            new BuffOnActorDamageModifier(RelicOfTheThief, "Relic of the Thief", "1% per stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByStack, BuffImages.RelicOfTheThief, DamageModifierMode.All),
            new BuffOnActorDamageModifier(RelicOfFireworks, "Relic of Fireworks", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfFireworks, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.September2023Balance),
            new BuffOnActorDamageModifier(RelicOfFireworks, "Relic of Fireworks", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfFireworks, DamageModifierMode.sPvP).WithBuilds(GW2Builds.September2023Balance),
            new BuffOnActorDamageModifier(RelicOfFireworks, "Relic of Fireworks", "7%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfFireworks, DamageModifierMode.PvEWvW).WithBuilds( GW2Builds.September2023Balance),
            new BuffOnActorDamageModifier(RelicOfTheBrawler, "Relic of the Brawler", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfTheBrawler, DamageModifierMode.All),
            new BuffOnActorDamageModifier(RelicOfTheDeadeye, "Relic of the Deadeye", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfTheDeadeye, DamageModifierMode.All),
            new BuffOnActorDamageModifier(RelicOfTheWeaver, "Relic of the Weaver", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfTheWeaver, DamageModifierMode.All),
            new BuffOnActorDamageModifier(NouryssHungerDamageBuff, "Relic of Nourys", "25%", DamageSource.NoPets, 25.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ByPresence, BuffImages.RelicOfNourys, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(NouryssHungerDamageBuff, "Relic of Nourys", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ByPresence, BuffImages.RelicOfNourys, DamageModifierMode.sPvPWvW),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Runes
            new DamageLogDamageModifier("Rune of Hoelbrak", "-10% condition damamge", DamageSource.NoPets, -10.0, DamageType.Condition, DamageType.All, Source.Gear, BuffImages.SuperiorRuneOfHoelbrak, (x, log) => true, DamageModifierMode.All)
                .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new DamageLogDamageModifier("Rune of the Stars", "-10% condition damamge", DamageSource.NoPets, -10.0, DamageType.Condition, DamageType.All, Source.Gear, BuffImages.SuperiorRuneOfTheStars, (x, log) => true, DamageModifierMode.All).WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new DamageLogDamageModifier("Rune of Mercy", "-20%", DamageSource.NoPets, -20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, BuffImages.SuperiorRuneOfMercy, (x, log) => log.CombatData.GetAnimatedCastData(Resurrect).Any(y => y.Caster == x.To && x.Time >= y.Time && x.Time <= y.EndTime), DamageModifierMode.All)
                .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new DamageLogDamageModifier("Rune of the Scrapper", "-10% condition damamge", DamageSource.NoPets, -7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, BuffImages.SuperiorRuneOfTheScrapper, (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 600.0;
            }, DamageModifierMode.PvEWvW)
                .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnFoeDamageModifier(Confusion, "Rune of Perplexity", "-10% from confused foes", DamageSource.NoPets, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfPerplexity, DamageModifierMode.All).WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            // Relics
            new BuffOnActorDamageModifier(NouryssHungerDamageBuff, "Relic of Nourys", "-15%", DamageSource.NoPets, -15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ByPresence, BuffImages.RelicOfNourys, DamageModifierMode.All),
        };
    }
}
