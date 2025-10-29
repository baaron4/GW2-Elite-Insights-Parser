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
        new BuffGainCastFinder(EnterRadiantForge, RadiantForge)
            .UsingBeforeWeaponSwap(),
        new BuffLossCastFinder(ExitRadiantForge, RadiantForge)
            .UsingBeforeWeaponSwap(),
        // Stances
        new BuffGainCastFinder(StalwartStanceSkill, StalwartStanceBuff),
        new BuffGainCastFinder(ValorousStanceSkill, ValorousStanceBuff),
        new BuffGainCastFinder(EffulgentStanceSkill, EffulgentStanceStackGainBuff), // TODO Improve? https://wiki.guildwars2.com/wiki/Effulgent_Stance
        new BuffLossCastFinder(EffulgentStanceDamage, EffulgentStanceStackDamageBuff), // TODO Improve? https://wiki.guildwars2.com/wiki/Effulgent_Stance
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Empowered Armaments
        new BuffOnActorDamageModifier(Mod_EmpoweredArmaments, EmpoweredArmaments, "Empowered Armaments", "15% strike damage", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Luminary, ByPresence, TraitImages.EmpoweredArmaments, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_EmpoweredArmaments, EmpoweredArmaments, "Empowered Armaments", "10% strike damage", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Luminary, ByPresence, TraitImages.EmpoweredArmaments, DamageModifierMode.sPvPWvW),
        // Radiant Armaments
        new BuffOnFoeDamageModifier(Mod_RadiantArmamentsHammer, [Stun, Daze, Knockdown, Fear, Taunt, Immobile], "Radiant Armaments (Hammer)", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Luminary, ByPresence, BuffImages.RadiantArmamentsHammer, DamageModifierMode.All)
        .UsingChecker((hde, log) => 
        {
            if (hde.From.HasAnyBuff(log, [RadiantArmamentsHammer, RadiantArmamentsHammerLingering], hde.Time))
            {
                return true;
            }
            return false;
        }),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Luminary's Blessing
        new BuffOnActorDamageModifier(Mod_LuminarysBlessing, LuminarysBlessing, "Luminary's Blessing", "-10%", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Luminary, ByPresence, TraitImages.LightsGift, DamageModifierMode.All),
        // Radiant Armaments
        new BuffOnActorDamageModifier(Mod_RadiantArmamentsShield, RadiantArmamentsShield, "Radiant Armaments (Shield)", "-15% strike damage", DamageSource.Incoming, -15.0, DamageType.Strike, DamageType.All, Source.Luminary, ByPresence, BuffImages.RadiantArmamentsShield, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_RadiantArmamentsShield, RadiantArmamentsShield, "Radiant Armaments (Shield)", "-10% strike damage", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Luminary, ByPresence, BuffImages.RadiantArmamentsShield, DamageModifierMode.sPvPWvW),
        new BuffOnActorDamageModifier(Mod_RadiantArmamentsShieldLingering, RadiantArmamentsHammerLingering, "Radiant Armaments (Shield Lingering)", "-10% strike damage", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Luminary, ByPresence, BuffImages.RadiantArmamentsShield, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Radiant Forge", RadiantForge, Source.Luminary, BuffClassification.Other, BuffImages.RadiantForge),
        // Radiant Armaments
        new Buff("Radiant Armaments (Hammer)", RadiantArmamentsHammer, Source.Luminary, BuffClassification.Other, BuffImages.RadiantArmamentsHammer),
        new Buff("Radiant Armaments (Hammer Lingering)", RadiantArmamentsHammerLingering, Source.Luminary, BuffClassification.Other, BuffImages.RadiantArmamentsHammer),
        new Buff("Radiant Armaments (Staff)", RadiantArmamentsStaff, Source.Luminary, BuffClassification.Other, BuffImages.RadiantArmamentsStaff),
        new Buff("Radiant Armaments (Staff Lingering)", RadiantArmamentsStaffLingering, Source.Luminary, BuffClassification.Other, BuffImages.RadiantArmamentsStaff),
        new Buff("Radiant Armaments (Sword)", RadiantArmamentsSword, Source.Luminary, BuffClassification.Other, BuffImages.RadiantArmamentsSword),
        new Buff("Radiant Armaments (Sword Lingering)", RadiantArmamentsSwordLingering, Source.Luminary, BuffClassification.Other, BuffImages.RadiantArmamentsSword),
        new Buff("Radiant Armaments (Shield)", RadiantArmamentsShield, Source.Luminary, BuffClassification.Other, BuffImages.RadiantArmamentsShield),
        new Buff("Radiant Armaments (Shield Lingering)", RadiantArmamentsShieldLingering, Source.Luminary, BuffClassification.Other, BuffImages.RadiantArmamentsShield),
        // Stances
        new Buff("Resolute Stance", ResoluteStanceBuff, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.ResoluteStance),
        new Buff("Stalwart Stance", StalwartStanceBuff, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.StalwartStance),
        new Buff("Valorous Stance", ValorousStanceBuff, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.ValorousStance),
        new Buff("Effulgent Stance (Stack Gain)", EffulgentStanceStackGainBuff, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.EffulgentStance),
        new Buff("Effulgent Stance (Stack Damage)", EffulgentStanceStackDamageBuff, Source.Luminary, BuffStackType.StackingConditionalLoss, 10, BuffClassification.Other, SkillImages.EffulgentStance),
        new Buff("Piercing Stance", PiercingStanceBuff, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.PiercingStance),
        new Buff("Daring Stance", DaringAdvanceBuff, Source.Luminary, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.DaringAdvance),
        // Traits
        new Buff("Luminary's Blessing", LuminarysBlessing, Source.Luminary, BuffClassification.Other, TraitImages.LightsGift),
        new Buff("Empowered Armaments", EmpoweredArmaments, Source.Luminary, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.EmpoweredArmaments),
        // Others
        new Buff("Counterattack (Luminary Shield)", CounterattackLuminary, Source.Luminary, BuffClassification.Other, SkillImages.Counterblow),
    ];
}
