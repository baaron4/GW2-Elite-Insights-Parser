using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class CommonDamageModifiers
    {
        internal static readonly List<DamageModifierDescriptor> ItemDamageModifiers = new List<DamageModifierDescriptor>
        {
            new DamageLogDamageModifier("Moving Bonus", "Seaweed Salad (and the likes) – 5% while moving", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Item, BuffImages.BowlOfSeaweedSalad, (x, log) => x.IsMoving, DamageModifierMode.All),
            new BuffOnActorDamageModifier(FractalOffensive, "Fractal Offensive", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndCondition, DamageType.All, Source.Item, ByStack, BuffImages.FractalOffensive, DamageModifierMode.PvE),
            new CounterOnActorDamageModifier(WritOfMasterfulMalice, "Writ of Masterful Malice", "200 condition damage if hp >=90%", DamageSource.NoPets, DamageType.Condition, DamageType.Condition, Source.Item,  BuffImages.WritOfMasterfulMalice, DamageModifierMode.All)
                .UsingChecker((x, log) => x.IsOverNinety),
            new CounterOnActorDamageModifier(WritOfMasterfulStrength, "Writ of Masterful Strength", "200 power if hp >=90%", DamageSource.NoPets, DamageType.Strike, DamageType.Strike, Source.Item, BuffImages.WritOfMasterfulStrength, DamageModifierMode.All)
                .UsingChecker((x, log) => x.IsOverNinety),
        };
        internal static readonly List<DamageModifierDescriptor> GearDamageModifiers = new List<DamageModifierDescriptor>
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
        internal static readonly List<DamageModifierDescriptor> SharedDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnFoeDamageModifier(Exposed31589, "Exposed", "50%", DamageSource.All, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnFoeDamageModifier(Exposed31589, "Exposed (Strike)", "30%", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds( GW2Builds.May2021Balance, GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(Exposed31589, "Exposed (Condition)", "100%", DamageSource.All, 100.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(Exposed31589, "Exposed (Strike)", "10%", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(Exposed31589, "Exposed (Condition)", "20%", DamageSource.All, 20.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(OldExposed, "Old Exposed (Strike)", "30%", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(OldExposed, "Old Exposed (Condition)", "100%", DamageSource.All, 100.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(Vulnerability, "Vulnerability", "1% per Stack", DamageSource.All, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByStack, BuffImages.Vulnerability, DamageModifierMode.All)
                .UsingChecker((evt, log) => !evt.To.HasBuff(log, Resistance, evt.Time) && (!evt.To.IsSpecies(TargetID.Sabir) || !evt.To.HasBuff(log, IonShield, evt.Time))), // Ion shield disabled vulnerability, so we check the presence of the buff when hitting Sabir 
            new BuffOnFoeDamageModifier(Protection, "Protection", "-33%", DamageSource.All, -33.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Protection, DamageModifierMode.All),
            new BuffOnFoeDamageModifier(Resolution, "Resolution", "-33%", DamageSource.All, -33.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Resolution, DamageModifierMode.All),
            new BuffOnActorDamageModifier(FrostSpiritBuff, "Frost Spirit", "5%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.FrostSpirit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
            new SkillDamageModifier("Soulcleave's Summit", "per hit (no ICD)", SoulcleavesSummitBuff, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, BuffImages.SoulcleavesSummit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new SkillDamageModifier("Soulcleave's Summit", "per hit (1s ICD per target)", SoulcleavesSummitBuff, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, BuffImages.SoulcleavesSummit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2021Balance),
            new SkillDamageModifier("One Wolf Pack", "per hit (max. once every 0.25s)", OneWolfPackDamage, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, BuffImages.OneWolfPack, DamageModifierMode.All),
            new BuffOnActorDamageModifier(Emboldened, "Emboldened", "10% per stack", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByStack, BuffImages.Emboldened, DamageModifierMode.All)
                .WithBuilds(GW2Builds.June2022Balance),
        };
        internal static readonly List<DamageModifierDescriptor> FightSpecificDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnFoeDamageModifier(UnnaturalSignet, "Unnatural Signet", "200%, stacks additively with Vulnerability", DamageSource.All, 200.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.UnnaturalSignet, DamageModifierMode.PvE).UsingGainAdjuster(VulnerabilityAdjuster),
            new BuffOnFoeDamageModifier(Compromised, "Compromised", "75% per stack", DamageSource.All, 75.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Compromised, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(ErraticEnergy, "Erratic Energy", "5% per stack, stacks additively with Vulnerability", DamageSource.All, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Unstable, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster),
            new BuffOnFoeDamageModifier(FracturedEnemy, "Fractured - Enemy", "10% per stack", DamageSource.All, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.BloodFueled, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(CacophonousMind, "Cacophonous Mind", "-5% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.TwistedEarth, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster)
                .UsingChecker((ahde, log) =>
                {
                    return VulnerabilityAdditiveChecker(ahde, log, CacophonousMind, 5);
                }),
            new CounterOnFoeDamageModifier(CacophonousMind, "Cacophonous Mind (Invul)", "-5% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.TwistedEarth, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !VulnerabilityAdditiveChecker(ahde, log, CacophonousMind, 5);
                }),
            new BuffOnFoeDamageModifier(DagdaDemonicAura, "Demonic Aura", "-10% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.ChampionOfTheCrown, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster)
                .UsingChecker((ahde, log) =>
                {
                    return VulnerabilityAdditiveChecker(ahde, log, DemonicAura, 10);
                }),
            new CounterOnFoeDamageModifier(DagdaDemonicAura, "Demonic Aura (Invul)", "-10% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.ChampionOfTheCrown, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !VulnerabilityAdditiveChecker(ahde, log, DemonicAura, 10);
                }),
            new BuffOnFoeDamageModifier(PowerOfTheVoid, "Power of the Void", "-25% per stack, multiplicative with itself", DamageSource.All, -25.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByMultipliyingStack, BuffImages.PowerOfTheVoid, DamageModifierMode.PvE),
            new BuffOnFoeDamageModifier(PillarPandemonium, "Pillar Pandemonium", "-20% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -20.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.CaptainsInspiration, DamageModifierMode.PvE)
                .UsingGainAdjuster(VulnerabilityAdjuster)
                .UsingChecker((ahde, log) =>
                {
                    return VulnerabilityAdditiveChecker(ahde, log, PillarPandemonium, 20);
                }),
            new CounterOnFoeDamageModifier(PillarPandemonium, "Pillar Pandemonium (Invul)", "-20% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.CaptainsInspiration, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !VulnerabilityAdditiveChecker(ahde, log, PillarPandemonium, 20);
                }),
            new BuffOnFoeDamageModifier(IonShield, "Ion Shield", "-5% per stack, while still capable of doing damage", DamageSource.All, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.IonShield, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    AbstractSingleActor target = log.FindActor(ahde.To);
                    Segment segment = target.GetBuffStatus(log, IonShield, ahde.Time);
                    return segment.Value < 20;
                }),
            new CounterOnFoeDamageModifier(IonShield, "Ion Shield (Invul)", "-5% per stack, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.IonShield, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    AbstractSingleActor target = log.FindActor(ahde.To);
                    Segment segment = target.GetBuffStatus(log, IonShield, ahde.Time);
                    return segment.Value >= 20;
                }),
            new BuffOnActorDamageModifier(EmpoweredStatueOfDeath, "Empowered (Statue of Death)", "50%", DamageSource.NoPets, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.EmpoweredEater, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(ViolentCurrents, "Violent Currents", "5% per stack", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.ViolentCurrents, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(BloodFueledPlayer, "Blood Fueled", "10% per stack", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.BloodFueled, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(BloodFueledMatthias, "Blood Fueled Abo", "10% per stack", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.BloodFueled, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(FractalSavant, "Fractal Savant", "1%", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Malign9Infusion, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(FractalProdigy, "Fractal Prodigy", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Mighty9Infusion, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(FractalChampion, "Fractal Champion", "4%", DamageSource.NoPets, 4.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Precise9Infusion, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(FractalGod, "Fractal God", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Healing9Infusion, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(SoulReunited, "Soul Reunited", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.AllysAidPoweredUp, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(Phantasmagoria, "Phantasmagoria", "50%", DamageSource.NoPets, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.VoidAffliction, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(StickingTogetherBuff, "Sticking Together", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.ActivateGreen, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(CrushingGuilt, "Crushing Guilt", "-5% per stack", DamageSource.NoPets, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.GuiltExploitation, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(Debilitated, "Debilitated", "-25% per stack", DamageSource.NoPets, -25.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Debilitated, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DebilitatedToxicSickness, "Debilitated (Toxic Sickness)", "-10% per stack", DamageSource.NoPets, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Debilitated, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(SpectralDarkness, "Spectral Darkness", "-5% per stack", DamageSource.NoPets, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.SpectralDarkness, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor1, "Dragon's End Contributor 1", "1%", DamageSource.NoPets, 1.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale01, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor2, "Dragon's End Contributor 2", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale02, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor3, "Dragon's End Contributor 3", "3%", DamageSource.NoPets, 3.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale03, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor4, "Dragon's End Contributor 4", "4%", DamageSource.NoPets, 4.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale04, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor5, "Dragon's End Contributor 5", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale05, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor6, "Dragon's End Contributor 6", "6%", DamageSource.NoPets, 6.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale06, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor7, "Dragon's End Contributor 7", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale07, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor8, "Dragon's End Contributor 8", "8%", DamageSource.NoPets, 8.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale08, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor9, "Dragon's End Contributor 9", "9%", DamageSource.NoPets, 9.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale09, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(DragonsEndContributor10, "Dragon's End Contributor 10", "20%", DamageSource.NoPets, 20.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale10, DamageModifierMode.PvE),
        };

    }
}
