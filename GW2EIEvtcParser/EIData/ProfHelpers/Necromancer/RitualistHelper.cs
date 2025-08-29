using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class RitualistHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder = 
    [
        new BuffGainCastFinder(EnterRitualistsShroud, RitualistsShroud)
            .UsingBeforeWeaponSwap(),
        new BuffLossCastFinder(ExitRitualistsShroud, RitualistsShroud)
            .UsingBeforeWeaponSwap(),
        new BuffGiveCastFinder(WeaponOfWarding, WeaponOfWardingBuff),
        new BuffGiveCastFinder(WeaponOfRemedy, WeaponOfRemedyBuff),
        new BuffGiveCastFinder(XinraeWeapon, XinraesWeaponBuff),
        new DamageCastFinder(ExplosiveGrowth, ExplosiveGrowth)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // TODO Check how to add Painful Bond from Anguish https://wiki.guildwars2.com/wiki/Anguish
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Ritualist's Shroud
        new BuffOnActorDamageModifier(Mod_RitualistsShroud, RitualistsShroud, "Ritualist's Shroud", "-33%", DamageSource.Incoming, -33, DamageType.StrikeAndCondition, DamageType.All, Source.Ritualist, ByPresence, SkillImages.RitualistShroud, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_RitualistsShroud, RitualistsShroud, "Ritualist's Shroud", "-50%", DamageSource.Incoming, -50, DamageType.StrikeAndCondition, DamageType.All, Source.Ritualist, ByPresence, SkillImages.RitualistShroud, DamageModifierMode.sPvPWvW),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Ritualist Shroud", RitualistsShroud, Source.Ritualist, BuffClassification.Other, SkillImages.RitualistShroud),
        new Buff("Painful Bond", PainfulBond, Source.Ritualist, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.Anguish),
        new Buff("Ritualist's Storm Spirit Aura (1)", RitualistStormSpiritAura1, Source.Ritualist, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Ritualist's Storm Spirit Aura (2)", RitualistStormSpiritAura2, Source.Ritualist, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Ritualist's Storm Spirit Aura (3)", RitualistStormSpiritAura3, Source.Ritualist, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Weapon of Remedy", WeaponOfRemedyBuff, Source.Ritualist, BuffClassification.Other, SkillImages.WeaponOfRemedy), // TODO Check buff stack type, no evtc at hand
        new Buff("Weapon of Warding", WeaponOfWardingBuff, Source.Ritualist, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, SkillImages.WeaponOfWarding),
        new Buff("Nightmare Weapon", NightmareWeaponBuff, Source.Ritualist, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, SkillImages.NightmareWeapon),
        new Buff("Xinrae's Weapon", XinraesWeaponBuff, Source.Ritualist, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, SkillImages.XinraesWeapon),
        new Buff("Resilient Weapon", ResilientWeaponBuff, Source.Ritualist, BuffClassification.Other, SkillImages.ResilientWeapon),
        new Buff("Splinter Weapon", SplinterWeaponBuff, Source.Ritualist, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, SkillImages.SplinterWeapon),
        new Buff("Lingering Spirits", LingeringSpirits, Source.Ritualist, BuffClassification.Other, TraitImages.LingeringSpirits),
        new Buff("Detonate Anguish", DetonateAnguish, Source.Ritualist, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Detonate Shelter", DetonateShelter, Source.Ritualist, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Detonate Sorrow", DetonateSorrow, Source.Ritualist, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Dark Stalker (Ritualist)", RitualistDarkStalker, Source.Ritualist, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Unknown),
    ];

    private static readonly HashSet<int> Minions = 
    [
        (int)MinionID.SpiritOfAnguish,
        (int)MinionID.SpiritOfWanderlust,
        (int)MinionID.SpiritOfPreservation,
    ];

    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    private static readonly HashSet<long> _ritualistShroudTransform = 
    [
        EnterRitualistsShroud, ExitRitualistsShroud
    ];

    public static bool IsRitualistShroudTransform(long id)
    {
        return _ritualistShroudTransform.Contains(id);
    }
}
