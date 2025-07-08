using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class GearDamageModifiers
{
    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Runes
        new DamageLogDamageModifier(Mod_ScholarRune, "Scholar Rune", "5% if hp >=90%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Gear, ItemImages.SuperiorRuneOfTheScholar, (x, log) => x.IsOverNinety, DamageModifierMode.All)
            .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
        new DamageLogDamageModifier(Mod_ScholarRune, "Scholar Rune", "10% if hp >=90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ItemImages.SuperiorRuneOfTheScholar, (x, log) => x.IsOverNinety, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune),
        new DamageLogDamageModifier(Mod_EagleRune,"Eagle Rune", "10% if target <50% HP", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ItemImages.SuperiorRuneOfTheEagle, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All)
            .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
        new DamageLogDamageModifier(Mod_EagleRune, "Eagle Rune", "6% if target <50% HP", DamageSource.NoPets, 6.0, DamageType.Strike, DamageType.Strike, Source.Gear, ItemImages.SuperiorRuneOfTheEagle, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune),
        new DamageLogDamageModifier(Mod_ThiefRune, "Thief Rune", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ItemImages.SuperiorRuneOfTheThief, (x, log) => x.IsFlanking, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new DamageLogDamageModifier(Mod_ThiefRune, "Thief Rune", "10% while flanking or against defiant", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ItemImages.SuperiorRuneOfTheThief, (x, log) => x.IsFlanking || x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None, DamageModifierMode.All)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnActorDamageModifier(Mod_StrengthRune, Might, "Strength Rune", "5% under might", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.SuperiorRuneOfStrength, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnActorDamageModifier(Mod_FireRune, FireAura, "Fire Rune", "10% under fire aura", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.SuperiorRuneOfFire, DamageModifierMode.All)
            .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnFoeDamageModifier(Mod_FlameLegionRune, Burning, "Flame Legion Rune", "7% on burning target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.SuperiorRuneOfTheFlameLegion, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnFoeDamageModifier(Mod_SpellbreakerRune, NumberOfBoons, "Spellbreaker Rune", "7% on boonless target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByAbsence, ItemImages.SuperiorRuneOfTheSpellbreaker, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnFoeDamageModifier(Mod_IceRune, Chilled, "Ice Rune", "7% on chilled target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.SuperiorRuneOfTheIce, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnActorDamageModifier(Mod_RageRune, Fury, "Rage Rune", "5% under fury", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.SuperiorRuneOfRage, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnFoeDamageModifier(Mod_RuneOfMesmer, Daze, "Rune of the Mesmer", "10% on dazed target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.SuperiorRuneOfTheMesmer, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        // Sigils
        new BuffOnFoeDamageModifier(Mod_ImpactSigil, [Stun, Knockdown], "Impact Sigil", "7% on stunned or knocked-down target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.SuperiorSigilOfImpact, DamageModifierMode.All),
        // Relics
        new BuffOnFoeDamageModifier(Mod_RelicOfTheDragonhunter, RelicOfTheDragonhunterTargetBuff, "Relic of the Dragonhunter", "10% after trap hit", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfTheDragonhunter, DamageModifierMode.All).UsingChecker((x, log) =>
        {
            var src = log.FindActor(x.From);
            var dst = log.FindActor(x.To);
            return dst.HasBuff(log, src, RelicOfTheDragonhunterTargetBuff, x.Time);
        }).UsingApproximate(), // Reapplication while buff is running is done via extension, extensions source finding is not capable of always finding the source
        new BuffOnFoeDamageModifier(Mod_RelicOfIsgarren, RelicOfIsgarrenTargetBuff, "Relic of Isgarren", "10% after evade", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfIsgarren, DamageModifierMode.All).UsingChecker((x, log) =>
        {
            var src = log.FindActor(x.From);
            var dst = log.FindActor(x.To);
            return dst.HasBuff(log, src, RelicOfIsgarrenTargetBuff, x.Time);
        }).UsingApproximate(), // Reapplication while buff is running is done via extension, extensions source finding is not capable of always finding the source
        new BuffOnFoeDamageModifier(Mod_RelicOfPeitha, RelicOfPeithaTargetBuff, "Relic of Peitha", "10% after blade hit", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfPeitha, DamageModifierMode.All).UsingChecker((x, log) =>
        {
            var src = log.FindActor(x.From);
            var dst = log.FindActor(x.To);
            return dst.HasBuff(log, src, RelicOfPeithaTargetBuff, x.Time);
        }).WithBuilds(GW2Builds.November2023Balance).UsingApproximate(), // Reapplication while buff is running is done via extension, extensions source finding is not capable of always finding the source
        new BuffOnActorDamageModifier(Mod_RelicOfTheThief, RelicOfTheThief, "Relic of the Thief", "1% per stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByStack, ItemImages.RelicOfTheThief, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_RelicOfFireworks, RelicOfFireworks, "Relic of Fireworks", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfFireworks, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_RelicOfFireworks, RelicOfFireworks, "Relic of Fireworks", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfFireworks, DamageModifierMode.sPvP).WithBuilds(GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_RelicOfFireworks, RelicOfFireworks, "Relic of Fireworks", "7%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfFireworks, DamageModifierMode.PvEWvW).WithBuilds( GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_RelicOfTheBrawler, RelicOfTheBrawler, "Relic of the Brawler", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfTheBrawler, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_RelicOfTheDeadeye, RelicOfTheDeadeye, "Relic of the Deadeye", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfTheDeadeye, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_RelicOfTheWeaver, RelicOfTheWeaver, "Relic of the Weaver", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfTheWeaver, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_RelicOfNourys, NouryssHungerDamageBuff, "Relic of Nourys", "25%", DamageSource.NoPets, 25.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ByPresence, ItemImages.RelicOfNourys, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_RelicOfNourys, NouryssHungerDamageBuff, "Relic of Nourys", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ByPresence, ItemImages.RelicOfNourys, DamageModifierMode.sPvPWvW),
        new BuffOnActorDamageModifier(Mod_RelicOfTheClaw, RelicOfTheClaw, "Relic of the Claw", "7% after disabling a foe", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Gear, ByPresence, ItemImages.RelicOfTheClaw, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_RelicOfMountBalrior, RelicOfMountBalrior, "Relic of Mount Balrior", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Gear, ByPresence, ItemImages.RelicOfMountBalrior, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_RelicOfFire, FireAura, "Relic of Fire", "10% under fire aura", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfFire, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.March2025W8CMReleaseAndNewCoreRelics),
        new BuffOnActorDamageModifier(Mod_RelicOfFire, FireAura, "Relic of Fire", "7% under fire aura", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfFire, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.March2025W8CMReleaseAndNewCoreRelics),
        // Relic of Bloodstone
        new BuffOnActorDamageModifier(Mod_RelicOfBloodstone, BloodstoneFervor, "Bloodstone Fervor", "15% strike damage", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfBloodstone, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.June2025Balance, GW2Builds.July2025BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_RelicOfBloodstone, BloodstoneFervor, "Bloodstone Fervor", "10% strike damage", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfBloodstone, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.June2025Balance, GW2Builds.July2025BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_RelicOfBloodstone, BloodstoneFervor, "Bloodstone Fervor", "10% strike damage", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, ItemImages.RelicOfBloodstone, DamageModifierMode.All)
            .WithBuilds(GW2Builds.July2025BalanceHotFix),
        //
        new DamageLogDamageModifier(Mod_RelicOfTheEagle,"Relic of the Eagle", "10% if target <50% HP", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ItemImages.RelicOfTheEagle, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All)
            .WithBuilds(GW2Builds.March2025W8CMReleaseAndNewCoreRelics),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Runes
        new DamageLogDamageModifier(Mod_RuneOfHoelbrak, "Rune of Hoelbrak", "-10% condition damamge", DamageSource.Incoming, -10.0, DamageType.Condition, DamageType.All, Source.Gear, ItemImages.SuperiorRuneOfHoelbrak, (x, log) => true, DamageModifierMode.All)
            .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
        new DamageLogDamageModifier(Mod_RuneOfTheStars, "Rune of the Stars", "-10% condition damamge", DamageSource.Incoming, -10.0, DamageType.Condition, DamageType.All, Source.Gear, ItemImages.SuperiorRuneOfTheStars, (x, log) => true, DamageModifierMode.All).WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
        new DamageLogDamageModifier(Mod_RuneOfMercy, "Rune of Mercy", "-20%", DamageSource.Incoming, -20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ItemImages.SuperiorRuneOfMercy, (x, log) => log.CombatData.GetAnimatedCastData(Resurrect).Any(y => y.Caster == x.To && x.Time >= y.Time && x.Time <= y.EndTime), DamageModifierMode.All)
            .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
        new DamageLogDamageModifier(Mod_RuneOfTheScrapper, "Rune of the Scrapper", "-10% condition damamge", DamageSource.Incoming, -7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ItemImages.SuperiorRuneOfTheScrapper, (x,log) =>
                x.From.TryGetCurrentPosition(log, x.Time, out var currentPosition)
                && x.To.TryGetCurrentPosition(log, x.Time, out var currentTargetPosition)
                && (currentPosition - currentTargetPosition).Length() >= 600.0
            , DamageModifierMode.PvEWvW)
            .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnFoeDamageModifier(Mod_RuneOfPerplexity, Confusion, "Rune of Perplexity", "-10% from confused foes", DamageSource.Incoming, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ByPresence, ItemImages.SuperiorRuneOfPerplexity, DamageModifierMode.All).WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
        // Relics
        new BuffOnActorDamageModifier(Mod_RelicOfNourys, NouryssHungerDamageBuff, "Relic of Nourys", "-15%", DamageSource.Incoming, -15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ByPresence, ItemImages.RelicOfNourys, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_RelicOfSorrow, RelicOfSorrowBuff, "Relic of Sorrow", "-20%", DamageSource.Incoming, -20.0, DamageType.Strike, DamageType.All, Source.Gear, ByPresence, ItemImages.RelicOfTheSorrow, DamageModifierMode.All),
    ];
}
