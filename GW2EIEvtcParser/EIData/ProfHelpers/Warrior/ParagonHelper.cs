using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class ParagonHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder = 
    [
        new EffectCastFinder(NeverSurrender, EffectGUIDs.ParagonNeverSurrenderInitial),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Chant of Action
        new BuffOnActorDamageModifier(Mod_ChantOfAction, ChantOfActionBuff, "Chant of Action", "20%", DamageSource.NoPets, 20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, SkillImages.ChantOfAction, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_ChantOfAction, ChantOfActionBuff, "Chant of Action", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, SkillImages.ChantOfAction, DamageModifierMode.sPvPWvW),
        // Brisk Pacing 1
        new BuffOnActorDamageModifier(Mod_BriskPacingTier1, BriskPacingTier1, "Brisk Pacing", "7.5%", DamageSource.NoPets, 7.5, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, TraitImages.BriskPacing, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_BriskPacingTier1, BriskPacingTier1, "Brisk Pacing", "5.0%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, TraitImages.BriskPacing, DamageModifierMode.sPvPWvW),
        // Brisk Pacing 2
        new BuffOnActorDamageModifier(Mod_BriskPacingTier2, BriskPacingTier2, "Brisk Pacing", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, TraitImages.BriskPacing, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_BriskPacingTier2, BriskPacingTier2, "Brisk Pacing", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, TraitImages.BriskPacing, DamageModifierMode.sPvPWvW),
        // Brisk Pacing 3
        new BuffOnActorDamageModifier(Mod_BriskPacingTier3, BriskPacingTier3, "Brisk Pacing", "25%", DamageSource.NoPets, 25.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, TraitImages.BriskPacing, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_BriskPacingTier3, BriskPacingTier3, "Brisk Pacing", "20%", DamageSource.NoPets, 20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, TraitImages.BriskPacing, DamageModifierMode.sPvPWvW),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Chant of Recuperation
        new BuffOnActorDamageModifier(Mod_ChantOfRecuperation, ChantOfRecuperationBuff, "Chant of Recuperation", "-20%", DamageSource.Incoming, -20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, SkillImages.ChantOfRecuperation, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_ChantOfRecuperation, ChantOfRecuperationBuff, "Chant of Recuperation", "-15%", DamageSource.Incoming, -15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, SkillImages.ChantOfRecuperation, DamageModifierMode.sPvPWvW),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        new Buff("Chant of Action", ChantOfActionBuff, Source.Paragon, BuffClassification.Other, SkillImages.ChantOfAction),
        new Buff("Chant of Recuperation", ChantOfRecuperationBuff, Source.Paragon, BuffClassification.Other, SkillImages.ChantOfRecuperation),
        new Buff("Chant of Freedom", ChantOfFreedomBuff, Source.Paragon, BuffClassification.Other, SkillImages.ChantOfFreedom),
        new Buff("\"We Will Never Yield!\"", WeWillNeverYieldBuff, Source.Paragon, BuffClassification.Support, SkillImages.WeWillNeverYield),
        new Buff("Brisk Pacing (Tier 1)", BriskPacingTier1, Source.Paragon, BuffClassification.Other, TraitImages.BriskPacing),
        new Buff("Brisk Pacing (Tier 2)", BriskPacingTier2, Source.Paragon, BuffClassification.Other, TraitImages.BriskPacing),
        new Buff("Brisk Pacing (Tier 3)", BriskPacingTier3, Source.Paragon, BuffClassification.Other, TraitImages.BriskPacing),
    ];
}
