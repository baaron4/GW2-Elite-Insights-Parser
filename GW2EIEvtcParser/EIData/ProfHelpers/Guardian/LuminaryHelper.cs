using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class LuminaryHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder = 
    [
        new BuffGainCastFinder(EnterRadiantShroud, RadiantShroud)
            .UsingBeforeWeaponSwap(),
        new BuffLossCastFinder(ExitRadiantShroud, RadiantShroud)
            .UsingBeforeWeaponSwap(),
        // Stances
        new BuffGiveCastFinder(StalwartStanceSkill, StalwartStanceBuff),
        new BuffGiveCastFinder(ValorousStanceSkill, ValorousStanceBuff),
        new BuffGiveCastFinder(EffulgentStanceSkill1, EffulgentStanceBuff1), // TODO Verify stance behaviour
        new BuffGiveCastFinder(EffulgentStanceSkill2, EffulgentStanceBuff2), // TODO Verify stance behaviour
        new BuffGiveCastFinder(PiercingStanceSkill, PiercingStanceBuff),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Empowered Armaments
        new BuffOnActorDamageModifier(Mod_EmpoweredArmaments, EmpoweredArmaments, "Empowered Armaments", "15% strike damage", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Luminary, ByPresence, TraitImages.EmpoweredArmaments, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_EmpoweredArmaments, EmpoweredArmaments, "Empowered Armaments", "10% strike damage", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Luminary, ByPresence, TraitImages.EmpoweredArmaments, DamageModifierMode.sPvPWvW),

        // Radiant Armaments
        // TODO Add Radiant Armaments
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Luminary's Blessing
        new BuffOnActorDamageModifier(Mod_RitualistsShroud, LuminarysBlessing, "Luminary's Blessing", "-10%", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Luminary, ByPresence, TraitImages.LightsGift, DamageModifierMode.All),

        // Radiant Armaments
        // TODO Add Radiant Armaments
    ];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        // TODO Figure out what is what https://wiki.guildwars2.com/wiki/Radiant_Armaments
        new Buff("Radiant Armaments (1)", RadiantArmamentsBuff1, Source.Luminary, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Radiant Armaments (2)", RadiantArmamentsBuff2, Source.Luminary, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Radiant Armaments (3)", RadiantArmamentsBuff3, Source.Luminary, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Radiant Armaments (4)", RadiantArmamentsBuff4, Source.Luminary, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Radiant Armaments (5)", RadiantArmamentsBuff5, Source.Luminary, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Radiant Armaments (6)", RadiantArmamentsBuff6, Source.Luminary, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Radiant Armaments (7)", RadiantArmamentsBuff7, Source.Luminary, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Radiant Armaments (8)", RadiantArmamentsBuff8, Source.Luminary, BuffClassification.Other, BuffImages.Unknown),
        // Stances
        new Buff("Resolute Stance", ResoluteStanceBuff, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.ResoluteStance),
        new Buff("Stalwart Stance", StalwartStanceBuff, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.StalwartStance),
        new Buff("Valorous Stance", ValorousStanceBuff, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.ValorousStance),
        new Buff("Effulgent Stance (1)", EffulgentStanceBuff1, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.EffulgentStance), // TODO improve name
        new Buff("Effulgent Stance (2)", EffulgentStanceBuff2, Source.Luminary, BuffStackType.StackingConditionalLoss, 10, BuffClassification.Other, SkillImages.EffulgentStance), // TODO improve name
        new Buff("Piercing Stance", PiercingStanceBuff, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.PiercingStance),
        new Buff("Daring Stance", DaringAdvanceBuff, Source.Luminary, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.DaringAdvance),
        // Traits
        new Buff("Luminary's Blessing", LuminarysBlessing, Source.Luminary, BuffClassification.Other, TraitImages.LightsGift),
        new Buff("Empowered Armaments", EmpoweredArmaments, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.EmpoweredArmaments),
    ];
}
