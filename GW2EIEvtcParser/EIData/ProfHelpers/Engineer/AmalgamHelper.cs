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
    internal static readonly List<InstantCastFinder> InstantCastFinder = [];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Willing Host
        new BuffOnActorDamageModifier(Mod_WillingHost, WillingHost, "Willing Host", "15% strike and condition damage", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Amalgam, ByPresence, TraitImages.WillingHost, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_WillingHost, WillingHost, "Willing Host", "10% strike and condition damage", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Amalgam, ByPresence, TraitImages.WillingHost, DamageModifierMode.sPvPWvW),
        // Plasmatic State
        new BuffOnActorDamageModifier(Mod_PlasmaticState, PlasmaticStateBuff, "Plasmatic State", "15% strike and condition damage", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Amalgam, ByPresence, SkillImages.PlasmaticState, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        // Morth Skills
        new Buff("Berserker Strain", BerserkerStrain, Source.Amalgam, BuffClassification.Other, SkillImages.OffensiveProtocolDemolish),
        new Buff("Titanic Strain", TitanicStrain, Source.Amalgam, BuffClassification.Other, SkillImages.OffensiveProtocolObliterate),
        new Buff("Rapacious Strain", RapaciousStrain, Source.Amalgam, BuffClassification.Other, SkillImages.DefensiveProtocolThorns),
        new Buff("Thorns", Thorns, Source.Amalgam, BuffClassification.Other, SkillImages.DefensiveProtocolThorns),
        new Buff("Replicating Strain", ReplicatingStrain, Source.Amalgam, BuffClassification.Other, SkillImages.DefensiveProtocolCleanse),
        new Buff("Evolved", Evolved, Source.Amalgam, BuffClassification.Other, BuffImages.Evolved),
        // Utility Skills
        new Buff("Mitotic State", MitoticStateBuff, Source.Amalgam, BuffClassification.Other, SkillImages.MitoticState),
        new Buff("Solid State", SolidStateBuff, Source.Amalgam, BuffClassification.Other, SkillImages.SolidState),
        new Buff("Gaseous State", GaseousStateBuff, Source.Amalgam, BuffClassification.Other, SkillImages.GaseousState),
        new Buff("Plasmatic State", PlasmaticStateBuff, Source.Amalgam, BuffClassification.Other, SkillImages.PlasmaticState),
    ];
}
