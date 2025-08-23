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
        new BuffOnActorDamageModifier(Mod_ChantOfAction, ChantOfActionBuff, "Chant of Action", "20% strike and condition damage", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, SkillImages.ChantOfAction, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_ChantOfAction, ChantOfActionBuff, "Chant of Action", "15% strike and condition damage", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, SkillImages.ChantOfAction, DamageModifierMode.sPvPWvW),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Chant of Recuperation
        new BuffOnActorDamageModifier(Mod_ChantOfRecuperation, ChantOfRecuperationBuff, "Chant of Recuperation", "-20% strike and condition damage", DamageSource.Incoming, -20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, SkillImages.ChantOfRecuperation, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_ChantOfRecuperation, ChantOfRecuperationBuff, "Chant of Recuperation", "-15% strike and condition damage", DamageSource.Incoming, -15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Paragon, ByPresence, SkillImages.ChantOfRecuperation, DamageModifierMode.sPvPWvW),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        new Buff("Chant of Action", ChantOfActionBuff, Source.Paragon, BuffClassification.Support, SkillImages.ChantOfAction),
        new Buff("Chant of Recuperation", ChantOfRecuperationBuff, Source.Paragon, BuffClassification.Support, SkillImages.ChantOfRecuperation),
        new Buff("Chant of Freedom", ChantOfFreedomBuff, Source.Paragon, BuffClassification.Support, SkillImages.ChantOfFreedom),
        new Buff("\"We Will Never Yield!\"", WeWillNeverYieldBuff, Source.Paragon, BuffClassification.Support, SkillImages.WeWillNeverYield),
    ];
}
