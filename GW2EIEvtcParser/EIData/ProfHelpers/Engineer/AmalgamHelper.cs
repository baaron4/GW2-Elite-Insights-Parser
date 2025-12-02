using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class AmalgamHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(GaseousStateSkill, GaseousStateBuff),
        new BuffGainCastFinder(DefensiveProtocolThorns1, ThornsBuff)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new EffectCastFinder(DefensiveProtocolCleanse, EffectGUIDs.AmalgamDefensiveProtocolCleanse1),
        new EffectCastFinder(SymbioticShielding, EffectGUIDs.AmalgamSymbioticShielding1),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Willing Host
        new BuffOnActorDamageModifier(Mod_WillingHost_StrikeCondition, WillingHost, "Willing Host", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Amalgam, ByPresence, TraitImages.WillingHost, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_WillingHost_Condition, WillingHost, "Willing Host (Condition)", "7%", DamageSource.NoPets, 7.0, DamageType.Condition, DamageType.All, Source.Amalgam, ByPresence, TraitImages.WillingHost, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_WillingHost_Strike, WillingHost, "Willing Host (Strike)", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Amalgam, ByPresence, TraitImages.WillingHost, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_WillingHost_StrikeCondition, WillingHost, "Willing Host", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Amalgam, ByPresence, TraitImages.WillingHost, DamageModifierMode.sPvPWvW),
        // Plasmatic State
        new BuffOnActorDamageModifier(Mod_PlasmaticState, PlasmaticStateBuff, "Plasmatic State", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Amalgam, ByPresence, SkillImages.PlasmaticState, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_PlasmaticState, PlasmaticStateBuff, "Plasmatic State", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Amalgam, ByPresence, SkillImages.PlasmaticState, DamageModifierMode.All)
            .WithBuilds(GW2Builds.OctoberVoERelease),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        // Traits     
        new Buff("Willing Host", WillingHost, Source.Amalgam, BuffClassification.Other, TraitImages.WillingHost),
        // Morth Skills
        new Buff("Berserker Strain", BerserkerStrain, Source.Amalgam, BuffClassification.Other, SkillImages.OffensiveProtocolDemolish),
        new Buff("Titanic Strain", TitanicStrain, Source.Amalgam, BuffClassification.Other, SkillImages.OffensiveProtocolObliterate),
        new Buff("Rapacious Strain", RapaciousStrain, Source.Amalgam, BuffClassification.Other, SkillImages.DefensiveProtocolThorns),
        new Buff("Thorns", ThornsBuff, Source.Amalgam, BuffClassification.Other, SkillImages.DefensiveProtocolThorns),
        new Buff("Replicating Strain", ReplicatingStrain, Source.Amalgam, BuffClassification.Other, SkillImages.DefensiveProtocolCleanse),
        new Buff("Evolved", Evolved, Source.Amalgam, BuffClassification.Other, BuffImages.Evolved),
        // Utility Skills
        new Buff("Mitotic State", MitoticStateBuff, Source.Amalgam, BuffClassification.Other, SkillImages.MitoticState),
        new Buff("Solid State", SolidStateBuff, Source.Amalgam, BuffClassification.Other, SkillImages.SolidState),
        new Buff("Gaseous State", GaseousStateBuff, Source.Amalgam, BuffClassification.Other, SkillImages.GaseousState),
        new Buff("Plasmatic State", PlasmaticStateBuff, Source.Amalgam, BuffClassification.Other, SkillImages.PlasmaticState),
    ];
}
