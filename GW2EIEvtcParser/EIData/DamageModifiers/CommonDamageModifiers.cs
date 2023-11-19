using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class CommonDamageModifiers
    {
        internal static readonly List<DamageModifier> ItemDamageModifiers = new List<DamageModifier>
        {
            new DamageLogDamageModifier("Moving Bonus", "Seaweed Salad (and the likes) – 5% while moving", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Item, BuffImages.BowlOfSeaweedSalad, (x, log) => x.IsMoving, ByPresence, DamageModifierMode.All),
            new BuffDamageModifier(FractalOffensive, "Fractal Offensive", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndCondition, DamageType.All, Source.Item, ByStack, BuffImages.FractalOffensive, DamageModifierMode.PvE),
            new BuffDamageModifier(WritOfMasterfulMalice, "Writ of Masterful Malice", "200 condition damage if hp >=90%", DamageSource.NoPets, 0.0, DamageType.Condition, DamageType.Condition, Source.Item, ByPresenceNonMultiplier, BuffImages.WritOfMasterfulMalice, DamageModifierMode.All)
                .UsingChecker((x, log) => x.IsOverNinety),
            new BuffDamageModifier(WritOfMasterfulStrength, "Writ of Masterful Strength", "200 power if hp >=90%", DamageSource.NoPets, 0.0, DamageType.Strike, DamageType.Strike, Source.Item, ByPresenceNonMultiplier, BuffImages.WritOfMasterfulStrength, DamageModifierMode.All)
                .UsingChecker((x, log) => x.IsOverNinety),
        };
        internal static readonly List<DamageModifier> GearDamageModifiers = new List<DamageModifier>
        {
            // Runes
            new DamageLogDamageModifier("Scholar Rune", "5% if hp >=90%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheScholar, (x, log) => x.IsOverNinety, ByPresence, DamageModifierMode.All)
                .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new DamageLogDamageModifier("Scholar Rune", "10% if hp >=90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheScholar, (x, log) => x.IsOverNinety, ByPresence, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune),
            new DamageLogDamageModifier("Eagle Rune", "10% if target <50% HP", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheEagle, (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All)
                .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new DamageLogDamageModifier("Eagle Rune", "6% if target <50% HP", DamageSource.NoPets, 6.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheEagle, (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune),
            new DamageLogDamageModifier("Thief Rune", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheThief, (x, log) => x.IsFlanking, ByPresence, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Thief Rune", "10% while flanking or against defiant", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, BuffImages.SuperiorRuneOfTheThief, (x, log) => x.IsFlanking || x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None, ByPresence, DamageModifierMode.All)
                .WithBuilds(GW2Builds.June2023Balance, GW2Builds.SOTOReleaseAndBalance),
            new BuffDamageModifier(Might, "Strength Rune", "5% under might", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfStrength, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new BuffDamageModifier(FireAura, "Fire Rune", "10% under fire aura", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfFire, DamageModifierMode.All)
                .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new BuffDamageModifierTarget(Burning, "Flame Legion Rune", "7% on burning target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfTheFlameLegion, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new BuffDamageModifierTarget(NumberOfBoons, "Spellbreaker Rune", "7% on boonless target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByAbsence, BuffImages.SuperiorRuneOfTheSpellbreaker, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new BuffDamageModifierTarget(Chilled, "Ice Rune", "7% on chilled target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfTheIce, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new BuffDamageModifier(Fury, "Rage Rune", "5% under fury", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorRuneOfRage, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            // Sigils
            new BuffDamageModifierTarget(new long[] { Stun, Knockdown }, "Impact Sigil", "7% on stunned or knocked-down target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.SuperiorSigilOfImpact, DamageModifierMode.All),
            // Relics
            new BuffDamageModifierTarget(RelicOfTheDragonhunterTargetBuff, "Relic of the Dragonhunter", "10% after trap hit", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfTheDragonhunter, DamageModifierMode.All).UsingChecker((x, log) =>
            {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return log.FindActor(dst).HasBuff(log, log.FindActor(src), RelicOfTheDragonhunterTargetBuff, x.Time);
            }).UsingApproximate(true), // Reapplication while buff is running is done via extension, extensions source finding is not capable of always finding the source
            new BuffDamageModifierTarget(RelicOfIsgarrenTargetBuff, "Relic of Isgarren", "10% after evade", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfIsgarren, DamageModifierMode.All).UsingChecker((x, log) =>
            {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return log.FindActor(dst).HasBuff(log, log.FindActor(src), RelicOfIsgarrenTargetBuff, x.Time);
            }).UsingApproximate(true), // Reapplication while buff is running is done via extension, extensions source finding is not capable of always finding the source
            new BuffDamageModifier(RelicOfTheThief, "Relic of the Thief", "1% per stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByStack, BuffImages.RelicOfTheThief, DamageModifierMode.All),
            new BuffDamageModifier(RelicOfFireworks, "Relic of Fireworks", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfFireworks, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.September2023Balance),
            new BuffDamageModifier(RelicOfFireworks, "Relic of Fireworks", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfFireworks, DamageModifierMode.sPvP).WithBuilds(GW2Builds.September2023Balance),
            new BuffDamageModifier(RelicOfFireworks, "Relic of Fireworks", "7%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfFireworks, DamageModifierMode.PvEWvW).WithBuilds( GW2Builds.September2023Balance),
            new BuffDamageModifier(RelicOfTheBrawler, "Relic of the Brawler", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfTheBrawler, DamageModifierMode.All),
            new BuffDamageModifier(RelicOfTheDeadeye, "Relic of the Deadeye", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfTheDeadeye, DamageModifierMode.All),
            new BuffDamageModifier(RelicOfTheWeaver, "Relic of the Weaver", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Gear, ByPresence, BuffImages.RelicOfTheWeaver, DamageModifierMode.All),
            new BuffDamageModifier(NouryssHungerDamageBuff, "Relic of Nourys", "25%", DamageSource.NoPets, 25.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ByPresence, BuffImages.RelicOfNourys, DamageModifierMode.PvE),
            new BuffDamageModifier(NouryssHungerDamageBuff, "Relic of Nourys", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Gear, ByPresence, BuffImages.RelicOfNourys, DamageModifierMode.sPvPWvW),
        };
        internal static readonly List<DamageModifier> SharedDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(Exposed31589, "Exposed", "50%", DamageSource.All, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifierTarget(Exposed31589, "Exposed (Strike)", "30%", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds( GW2Builds.May2021Balance, GW2Builds.March2022Balance2),
            new BuffDamageModifierTarget(Exposed31589, "Exposed (Condition)", "100%", DamageSource.All, 100.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.March2022Balance2),
            new BuffDamageModifierTarget(Exposed31589, "Exposed (Strike)", "10%", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffDamageModifierTarget(Exposed31589, "Exposed (Condition)", "20%", DamageSource.All, 20.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffDamageModifierTarget(OldExposed, "Old Exposed (Strike)", "30%", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffDamageModifierTarget(OldExposed, "Old Exposed (Condition)", "100%", DamageSource.All, 100.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffDamageModifierTarget(Vulnerability, "Vulnerability", "1% per Stack", DamageSource.All, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByStack, BuffImages.Vulnerability, DamageModifierMode.All)
                .UsingChecker((evt, log) => !evt.To.HasBuff(log, Resistance, evt.Time) && (!evt.To.IsSpecies(TargetID.Sabir) || !evt.To.HasBuff(log, IonShield, evt.Time))), // Ion shield disabled vulnerability, so we check the presence of the buff when hitting Sabir
            new BuffDamageModifier(FrostSpiritBuff, "Frost Spirit", "5%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.FrostSpirit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
            new DamageLogDamageModifier("Soulcleave's Summit", "per hit (no ICD)", DamageSource.NoPets, 0, DamageType.Power, DamageType.All, Source.Common, BuffImages.SoulcleavesSummit, ((x, log) => x.SkillId == SoulcleavesSummitBuff), BySkill, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new DamageLogDamageModifier("Soulcleave's Summit", "per hit (1s ICD per target)", DamageSource.NoPets, 0, DamageType.Power, DamageType.All, Source.Common, BuffImages.SoulcleavesSummit, ((x, log) => x.SkillId == SoulcleavesSummitBuff), BySkill, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2021Balance),
            new DamageLogDamageModifier("One Wolf Pack", "per hit (max. once every 0.25s)", DamageSource.NoPets, 0, DamageType.Power, DamageType.All, Source.Common, BuffImages.OneWolfPack, ((x, log) => x.SkillId == OneWolfPackDamage), BySkill, DamageModifierMode.All),
            new BuffDamageModifier(Emboldened, "Emboldened", "10% per stack", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByStack, BuffImages.Emboldened, DamageModifierMode.All)
                .WithBuilds(GW2Builds.June2022Balance),
        };
        internal static readonly List<DamageModifier> FightSpecificDamageModifiers = new List<DamageModifier>
        {
            new BuffDamageModifier(ViolentCurrents, "Violent Currents", "5% per stack", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.ViolentCurrents, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(UnnaturalSignet, "Unnatural Signet", "200%, stacks additively with Vulnerability", DamageSource.All, 200.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.UnnaturalSignet, DamageModifierMode.PvE).UsingGainAdjuster((dl, log) =>
            {
                AbstractSingleActor target = log.FindActor(dl.To);
                IReadOnlyDictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
                if (bgms.TryGetValue(Vulnerability, out BuffsGraphModel bgm))
                {
                    return 1.0 / (1.0 + 0.01 * bgm.GetStackCount(dl.Time));
                }
                return 1.0;
            }),
            new BuffDamageModifier(EmpoweredStatueOfDeath, "Empowered (Statue of Death)", "50%", DamageSource.NoPets, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.EmpoweredEater, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(Compromised, "Compromised", "75% per stack", DamageSource.All, 75.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Compromised, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(ErraticEnergy, "Erratic Energy", "5% per stack, stacks additively with Vulnerability", DamageSource.All, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.Unstable, DamageModifierMode.PvE)
                .UsingGainAdjuster((dl, log) =>
                {
                    AbstractSingleActor target = log.FindActor(dl.To);
                    IReadOnlyDictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
                    if (bgms.TryGetValue(Vulnerability, out BuffsGraphModel bgm))
                    {
                        return 1.0 / (1.0 + 0.01 * bgm.GetStackCount(dl.Time));
                    }
                    return 1.0;
                }),
            new BuffDamageModifierTarget(FracturedEnemy, "Fractured - Enemy", "10% per stack", DamageSource.All, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.BloodFueled, DamageModifierMode.PvE),
            new BuffDamageModifier(BloodFueledPlayer, "Blood Fueled", "10% per stack", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.BloodFueled, DamageModifierMode.PvE),
            new BuffDamageModifier(BloodFueledMatthias, "Blood Fueled Abo", "10% per stack", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByStack, BuffImages.BloodFueled, DamageModifierMode.PvE),
            new BuffDamageModifier(FractalSavant, "Fractal Savant", "1%", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Malign9Infusion, DamageModifierMode.PvE),
            new BuffDamageModifier(FractalProdigy, "Fractal Prodigy", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Mighty9Infusion, DamageModifierMode.PvE),
            new BuffDamageModifier(FractalChampion, "Fractal Champion", "4%", DamageSource.NoPets, 4.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Precise9Infusion, DamageModifierMode.PvE),
            new BuffDamageModifier(FractalGod, "Fractal God", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.Healing9Infusion, DamageModifierMode.PvE),
            new BuffDamageModifier(SoulReunited, "Soul Reunited", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.AllysAidPoweredUp, DamageModifierMode.PvE),
            new BuffDamageModifier(Phantasmagoria, "Phantasmagoria", "50%", DamageSource.NoPets, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.VoidAffliction, DamageModifierMode.PvE),
            new BuffDamageModifier(StickingTogetherBuff, "Sticking Together", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.ActivateGreen, DamageModifierMode.PvE),
            new BuffDamageModifier(DragonsEndContributor1, "Dragon's End Contributor 1", "1%", DamageSource.NoPets, 1.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale01, DamageModifierMode.PvE),
            new BuffDamageModifier(DragonsEndContributor2, "Dragon's End Contributor 2", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale02, DamageModifierMode.PvE),
            new BuffDamageModifier(DragonsEndContributor3, "Dragon's End Contributor 3", "3%", DamageSource.NoPets, 3.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale03, DamageModifierMode.PvE),
            new BuffDamageModifier(DragonsEndContributor4, "Dragon's End Contributor 4", "4%", DamageSource.NoPets, 4.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale04, DamageModifierMode.PvE),
            new BuffDamageModifier(DragonsEndContributor5, "Dragon's End Contributor 5", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale05, DamageModifierMode.PvE),
            new BuffDamageModifier(DragonsEndContributor6, "Dragon's End Contributor 6", "6%", DamageSource.NoPets, 6.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale06, DamageModifierMode.PvE),
            new BuffDamageModifier(DragonsEndContributor7, "Dragon's End Contributor 7", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale07, DamageModifierMode.PvE),
            new BuffDamageModifier(DragonsEndContributor8, "Dragon's End Contributor 8", "8%", DamageSource.NoPets, 8.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale08, DamageModifierMode.PvE),
            new BuffDamageModifier(DragonsEndContributor9, "Dragon's End Contributor 9", "9%", DamageSource.NoPets, 9.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale09, DamageModifierMode.PvE),
            new BuffDamageModifier(DragonsEndContributor10, "Dragon's End Contributor 10", "20%", DamageSource.NoPets, 20.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.FightSpecific, ByPresence, BuffImages.SeraphMorale10, DamageModifierMode.PvE),
        };

    }
}
