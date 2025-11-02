using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class EncounterDamageModifiers
{
    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnFoeDamageModifier(Mod_UnnaturalSignet, UnnaturalSignet, "Unnatural Signet", "200%, stacks additively with Vulnerability", DamageSource.All, 200.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.UnnaturalSignet, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster),
        new BuffOnFoeDamageModifier(Mod_Compromised, Compromised, "Compromised", "75% per stack", DamageSource.All, 75.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Compromised, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_ErraticEnergy, ErraticEnergy, "Erratic Energy", "5% per stack, stacks additively with Vulnerability", DamageSource.All, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Unstable, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster),
        new BuffOnFoeDamageModifier(Mod_FracturedEnemy, FracturedEnemy, "Fractured - Enemy", "50% per stack", DamageSource.All, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, TraitImages.ExposedWeakness, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_DiaphanousShielding, DiaphanousShielding, "Diaphanous Shielding", "-10% per stack, stacks additively with Vulnerability", DamageSource.All, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.DiaphanousShielding, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster),
        new BuffOnFoeDamageModifier(Mod_CacophonousMind, CacophonousMind, "Cacophonous Mind", "-5% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.TwistedEarth, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster)
            .UsingChecker((ahde, log) =>
            {
                return VulnerabilityAdditiveChecker(ahde, log, CacophonousMind, 5);
            }),
        new CounterOnFoeDamageModifier(Mod_CacophonousMindInvul, CacophonousMind, "Cacophonous Mind (Invul)", "-5% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, BuffImages.TwistedEarth, DamageModifierMode.PvE)
            .UsingChecker((ahde, log) =>
            {
                return !VulnerabilityAdditiveChecker(ahde, log, CacophonousMind, 5);
            }),
        new BuffOnFoeDamageModifier(Mod_DagdaDemonicAura, DagdaDemonicAura, "Demonic Aura", "-10% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, ItemImages.ChampionOfTheCrown, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster)
            .UsingChecker((ahde, log) =>
            {
                return VulnerabilityAdditiveChecker(ahde, log, DagdaDemonicAura, 10);
            }),
        new CounterOnFoeDamageModifier(Mod_DagdaDemonicAuraInvul, DagdaDemonicAura, "Demonic Aura (Invul)", "-10% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ItemImages.ChampionOfTheCrown, DamageModifierMode.PvE)
            .UsingChecker((ahde, log) =>
            {
                return !VulnerabilityAdditiveChecker(ahde, log, DagdaDemonicAura, 10);
            }),
        new BuffOnFoeDamageModifier(Mod_PowerOfTheVoid, PowerOfTheVoid, "Power of the Void", "-25% per stack, multiplicative with itself", DamageSource.All, -25.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByMultipliyingStack, BuffImages.PowerOfTheVoid, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_PillarPandemonium, PillarPandemonium, "Pillar Pandemonium", "-20% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -20.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.CaptainsInspiration, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster)
            .UsingChecker((ahde, log) =>
            {
                return VulnerabilityAdditiveChecker(ahde, log, PillarPandemonium, 20);
            }),
        new CounterOnFoeDamageModifier(Mod_PillarPandemoniumInvul, PillarPandemonium, "Pillar Pandemonium (Invul)", "-20% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, BuffImages.CaptainsInspiration, DamageModifierMode.PvE)
            .UsingChecker((ahde, log) =>
            {
                return !VulnerabilityAdditiveChecker(ahde, log, PillarPandemonium, 20);
            }),
        new BuffOnFoeDamageModifier(Mod_ShieldedCA, ShieldedCA, "Shielded CA", "-100% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -100.0, DamageType.Condition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.CaptainsInspiration, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster)
            .UsingChecker((ahde, log) =>
            {
                return VulnerabilityAdditiveChecker(ahde, log, ShieldedCA, 100);
            }),
        new CounterOnFoeDamageModifier(Mod_ShieldedCAInvul, ShieldedCA, "Shielded CA (Invul)", "-100% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.Condition, DamageType.All, Source.EncounterSpecific, BuffImages.PoweredShielding, DamageModifierMode.PvE)
            .UsingChecker((ahde, log) =>
            {
                return !VulnerabilityAdditiveChecker(ahde, log, ShieldedCA, 100);
            }),
        new BuffOnFoeDamageModifier(Mod_IonShield, IonShield, "Ion Shield", "-5% per stack, while still capable of doing damage", DamageSource.All, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.IonShield, DamageModifierMode.PvE)
            .UsingChecker((ahde, log) =>
            {
                var target = log.FindActor(ahde.To);
                var segment = target.GetBuffStatus(log, IonShield, ahde.Time);
                return segment.Value < 20;
            }),
        new CounterOnFoeDamageModifier(Mod_IonShieldInvul, IonShield, "Ion Shield (Invul)", "-5% per stack, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, BuffImages.IonShield, DamageModifierMode.PvE)
            .UsingChecker((ahde, log) =>
            {
                var target = log.FindActor(ahde.To);
                var segment = target.GetBuffStatus(log, IonShield, ahde.Time);
                return segment.Value >= 20;
            }),
        new BuffOnFoeDamageModifier(Mod_IcyBarrier, IcyBarrier, "Icy Barrier", "-10% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.ShieldOfIce, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster)
            .UsingChecker((ahde, log) =>
            {
                return VulnerabilityAdditiveChecker(ahde, log, IcyBarrier, 10);
            }),
        new CounterOnFoeDamageModifier(Mod_IcyBarrierInvul, IcyBarrier, "Icy Barrier (Invul)", "-10% per stack, stacks additively with Vulnerability, while doing 0 damage", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, BuffImages.ShieldOfIce, DamageModifierMode.PvE)
            .UsingChecker((ahde, log) =>
            {
                return !VulnerabilityAdditiveChecker(ahde, log, IcyBarrier, 10);
            }),
        new BuffOnActorDamageModifier(Mod_EmpoweredStatueOfDeath, EmpoweredStatueOfDeath, "Empowered (Statue of Death)", "50%", DamageSource.NoPets, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.EmpoweredEater, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_ViolentCurrents, ViolentCurrents, "Violent Currents", "5% per stack", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.ViolentCurrents, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_BloodShield, [BloodShield, BloodShieldAbo], "Blood Shield", "-90% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -90.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.BloodShield, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster)
            .UsingChecker((ahde, log) =>
            {
                return VulnerabilityAdditiveChecker(ahde, log, BloodShield, 90) || VulnerabilityAdditiveChecker(ahde, log, BloodShieldAbo, 90);
            }),
        new CounterOnFoeDamageModifier(Mod_BloodShieldInvul, [BloodShield, BloodShieldAbo], "Blood Shield (invul)", "-90% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, BuffImages.BloodShield, DamageModifierMode.PvE)
            .UsingChecker((ahde, log) =>
            {
                return !(VulnerabilityAdditiveChecker(ahde, log, BloodShield, 90) || VulnerabilityAdditiveChecker(ahde, log, BloodShieldAbo, 90));
            }),
        new BuffOnFoeDamageModifier(Mod_LethalInspiration, LethalInspiration, "Lethal Inspiration", "-90%, stacks additively with Vulnerability", DamageSource.NoPets, -90.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.PowerOfTheVoid, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster),
        new BuffOnActorDamageModifier(Mod_BloodFueledPlayer, BloodFueledPlayer, "Blood Fueled", "10% per stack", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.BloodFueled, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_FractalSavant, FractalSavant, "Fractal Savant", "1%", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Malign9Infusion, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_FractaProdigy, FractalProdigy, "Fractal Prodigy", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Mighty9Infusion, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_FractaChampion, FractalChampion, "Fractal Champion", "4%", DamageSource.NoPets, 4.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Precise9Infusion, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_FractaGod, FractalGod, "Fractal God", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Healing9Infusion, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_SoulReunited, SoulReunited, "Soul Reunited", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.AllysAidPoweredUp, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_Phantasmagoria, Phantasmagoria, "Phantasmagoria", "50%", DamageSource.NoPets, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.VoidAffliction, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_StickingTogether, StickingTogetherBuff, "Sticking Together", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.ActivateGreen, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_CrushingGuilt, CrushingGuilt, "Crushing Guilt", "-5% per stack", DamageSource.NoPets, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.GuiltExploitation, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_Debilitated, Debilitated, "Debilitated", "-25% per stack", DamageSource.NoPets, -25.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Debilitated, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_SappingSurge, SappingSurge, "Sapping Surge", "-25%", DamageSource.NoPets, -25.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.GuiltExploitation, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DebilitatedToxicSickness, DebilitatedToxicSickness, "Debilitated (Toxic Sickness)", "-10% per stack", DamageSource.NoPets, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Debilitated, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_SpectralDarkness, SpectralDarkness, "Spectral Darkness", "-5% per stack", DamageSource.NoPets, -5.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.SpectralDarkness, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DragonsEndContributor1, DragonsEndContributor1, "Dragon's End Contributor 1", "1%", DamageSource.NoPets, 1.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.SeraphMorale01, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DragonsEndContributor2, DragonsEndContributor2, "Dragon's End Contributor 2", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.SeraphMorale02, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DragonsEndContributor3, DragonsEndContributor3, "Dragon's End Contributor 3", "3%", DamageSource.NoPets, 3.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.SeraphMorale03, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DragonsEndContributor4, DragonsEndContributor4, "Dragon's End Contributor 4", "4%", DamageSource.NoPets, 4.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.SeraphMorale04, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DragonsEndContributor5, DragonsEndContributor5, "Dragon's End Contributor 5", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.SeraphMorale05, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DragonsEndContributor6, DragonsEndContributor6, "Dragon's End Contributor 6", "6%", DamageSource.NoPets, 6.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.SeraphMorale06, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DragonsEndContributor7, DragonsEndContributor7, "Dragon's End Contributor 7", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.SeraphMorale07, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DragonsEndContributor8, DragonsEndContributor8, "Dragon's End Contributor 8", "8%", DamageSource.NoPets, 8.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.SeraphMorale08, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DragonsEndContributor9, DragonsEndContributor9, "Dragon's End Contributor 9", "9%", DamageSource.NoPets, 9.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.SeraphMorale09, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DragonsEndContributor10, DragonsEndContributor10, "Dragon's End Contributor 10", "20%", DamageSource.NoPets, 20.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.SeraphMorale10, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_RageAttunement, RageAttunement, "Rage Attunement", "5%", DamageSource.NoPets, 5, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.RageAttunement, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DespairAttunement, DespairAttunement, "Despair Attunement", "-3%", DamageSource.NoPets, -3, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.RageAttunement, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_Consumed, Consumed, "Consumed", "-3%", DamageSource.NoPets, -3, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Consumed, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_RisingPressure, RisingPressure, "Rising Pressure", "-5% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.All, -5, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.RisingPressure, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster)
            .UsingChecker((ahde, log) =>
            {
                return VulnerabilityAdditiveChecker(ahde, log, RisingPressure, 5);
            }),
        new CounterOnFoeDamageModifier(Mod_RisingPressureInvul, RisingPressure, "Rising Pressure (invul)", "-5% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.All, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, BuffImages.RisingPressure, DamageModifierMode.PvE)
            .UsingChecker((ahde, log) =>
            {
                return !VulnerabilityAdditiveChecker(ahde, log, RisingPressure, 5);
            }),
        new BuffOnFoeDamageModifier(Mod_UnstrippableProtection, ProtectionUnstrippable, "Protection (Unstrippable)", "-33%", DamageSource.All, -33.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Protection, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_UnstrippableResolution, ResolutionUnstrippable, "Resolution (Unstrippable)", "-33%", DamageSource.All, -33.0, DamageType.Condition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Resolution, DamageModifierMode.PvE),
        // Enrages
        new BuffOnFoeDamageModifier(Mod_Enraged_inc25, Enraged_100_strike_25_reduc, "Enraged (-25%)", "-25%, stacks additively with Vulnerability", DamageSource.All, -25, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster),
        new BuffOnFoeDamageModifier(Mod_Enraged_inc50, Enraged_200_strike_50_reduc, "Enraged (-50%)", "-50%, stacks additively with Vulnerability", DamageSource.All, -50, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster),
        new BuffOnFoeDamageModifier(Mod_Enraged_inc75, Enraged_300_strike_75_reduc, "Enraged (-75%)", "-75%, stacks additively with Vulnerability", DamageSource.All, -75, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [

        new BuffOnActorDamageModifier(Mod_ConjuredProtection, ConjuredProtection, "Conjured Protection", "-10% per stack, stacks additively with Vulnerability, while still capable of doing damage", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Fractured, DamageModifierMode.PvE)
            .UsingGainAdjuster(VulnerabilityAdjuster)
            .UsingChecker((ahde, log) =>
            {
                return VulnerabilityAdditiveChecker(ahde, log, ConjuredProtection, 10);
            }),
        new CounterOnActorDamageModifier(Mod_ConjuredProtectionInvul, ConjuredProtection, "Conjured Protection (Invul)", "-10% per stack, stacks additively with Vulnerability, while doing 0 damages", DamageSource.Incoming, DamageType.Strike, DamageType.All, Source.EncounterSpecific, BuffImages.Fractured, DamageModifierMode.PvE)
            .UsingChecker((ahde, log) =>
            {
                return !VulnerabilityAdditiveChecker(ahde, log, ConjuredProtection, 10);
            }),
        new BuffOnActorDamageModifier(Mod_ExposedPlayer, ExposedPlayer, "Exposed (Player)", "25% per stack", DamageSource.Incoming, 25.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Exposed, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_NotStickingTogether, NotStickingTogetherBuff, "Not Sticking Together", "25%", DamageSource.Incoming, 25.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.ActivateRed, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_EnvyAttunement, EnvyAttunement, "Envy Attunement", "5%", DamageSource.Incoming, 5, DamageType.Condition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.EnvyAttunement, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_RageAttunement, RageAttunement, "Rage Attunement", "5%", DamageSource.Incoming, 5, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.RageAttunement, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DespairAttunement, DespairAttunement, "Despair Attunement", "-3%", DamageSource.Incoming, -3, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.RageAttunement, DamageModifierMode.PvE),
        //
        new BuffOnFoeDamageModifier(Mod_DiaphanousShielding, DiaphanousShielding, "Diaphanous Shielding", "25% per stack", DamageSource.Incoming, 25.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.DiaphanousShielding, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_BloodFueledMatthias, BloodFueledMatthias, "Blood Fueled Abo", "10% per stack", DamageSource.Incoming, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.BloodFueled, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_EmpoweredMO, EmpoweredMO, "Empowered (MO)", "25% per stack", DamageSource.Incoming, 25.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.EmpoweredMursaarOverseer, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_StrengthenedBondGuldhem, StrengthenedBondGuldhem, "Strengthened_Bond:_Guldhem", "10% per stack", DamageSource.Incoming, 10.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.StrengthenedBondGuldhem, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_Devour, Devour, "Devour", "2% per stack", DamageSource.Incoming, 2.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Devour, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_AquaticAura, [AquaticAuraKenut, AquaticAuraNikare], "Aquatic Aura", "2% per stack", DamageSource.Incoming, 2.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.ExposeWeakness, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_FracturedAlly, FracturedAllied, "Fractured (Ally)", "50% per stack", DamageSource.Incoming, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Fractured, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_FierySurge, FierySurge, "Fiery Surge", "20% per stack", DamageSource.Incoming, 20.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.FierySurge, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_AugmentedPower, AugmentedPower, "Augmented Power", "10% per stack", DamageSource.Incoming, 10.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.FierySurge, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_IonShield, IonShield, "Ion Shield", "3% per stack", DamageSource.Incoming, 3.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.IonShield, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_PillarPandemonium, PillarPandemonium, "Pillar Pandemonium", "20% per stack", DamageSource.Incoming, 20.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.CaptainsInspiration, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_CacophonousMind, CacophonousMind, "Cacophonous Mind", "5% per stack", DamageSource.Incoming, 5, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.TwistedEarth, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_PowerOfTheVoid ,PowerOfTheVoid, "Power of theVoid", "25% per stack", DamageSource.Incoming, 25, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.PowerOfTheVoid, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_EmpoweredWatchknight, EmpoweredWatchknightTriumverate, "Empowered (Watchknight Triumverate)", "5% per stack", DamageSource.Incoming, 5, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.ChargingEnergies, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_EmpoweredCerus, EmpoweredCerus, "Empowered (Cerus)", "5% per stack", DamageSource.Incoming, 5, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.EmpoweredMursaarOverseer, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_EmpoweredGreer, EmpoweredGreer, "Empowered (Greer)", "5% per stack", DamageSource.Incoming, 5, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.EmpoweredMursaarOverseer, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_DevourLT, DevourLonelyTower, "Devour (Lonely Tower)", "2% per stack", DamageSource.Incoming, 2, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Devour, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_BrothersUnited, BrothersUnited, "Brothers United", "50%", DamageSource.Incoming, 50, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.BrothersUnited, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_RisingPressure, RisingPressure, "Rising Pressure", "5% per stack", DamageSource.Incoming, 5, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.RisingPressure, DamageModifierMode.PvE),
        // Enrages
        new BuffOnFoeDamageModifier(Mod_Enraged_out100, [Enraged_100_strike, Enraged_100_strike_25_reduc], "Enraged (100% strike)", "100%", DamageSource.Incoming, 100.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_Enraged_outAll200, Enraged_200, "Enraged (200%)", "200%", DamageSource.Incoming, 200.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_Enraged_out200, [Enraged_200_strike, Enraged_200_strike_50_reduc], "Enraged (200% strike)", "200%", DamageSource.Incoming, 200.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_Enraged_out300, Enraged_300_strike_75_reduc, "Enraged (300% strike)", "300%", DamageSource.Incoming, 300.0, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_Enraged_outAll500, Enraged_500, "Enraged (500%)", "500%", DamageSource.Incoming, 500.0, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_EnragedFractal, EnragedFractal, "Enraged (Fractal)", "110%", DamageSource.Incoming, 110, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_EnragedCairn, EnragedCairn2, "Enraged (Cairn)", "200% per stack", DamageSource.Incoming, 200, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByStack, BuffImages.Enraged, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_EnragedTwinLargos, EnragedTwinLargos, "Enraged (Twin Largos)", "100%", DamageSource.Incoming, 100, DamageType.StrikeAndCondition, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
        new BuffOnFoeDamageModifier(Mod_EnragedTwinWyvern, EnragedWyvern, "Enraged (Wyvern)", "100%", DamageSource.Incoming, 100, DamageType.Strike, DamageType.All, Source.EncounterSpecific, ByPresence, BuffImages.Enraged, DamageModifierMode.PvE),
    ];
}
