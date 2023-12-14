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
    internal static class EncounterDamageModifiers
    {
        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
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
                    return VulnerabilityAdditiveChecker(ahde, log, DagdaDemonicAura, 10);
                }),
            new CounterOnFoeDamageModifier(DagdaDemonicAura, "Demonic Aura (Invul)", "-10% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.FightSpecific, BuffImages.ChampionOfTheCrown, DamageModifierMode.PvE)
                .UsingChecker((ahde, log) =>
                {
                    return !VulnerabilityAdditiveChecker(ahde, log, DagdaDemonicAura, 10);
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

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
        };
    }
}
