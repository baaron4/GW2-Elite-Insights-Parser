using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.NecromancerHelper;
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
        new DamageCastFinder(ExplosiveGrowthSkill, ExplosiveGrowthSkill)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Lingering Spirits
        new BuffOnActorDamageModifier(Mod_LingeringSpiritsAnguish, LingeringSpiritsAnguish, "Lingering Spirits (Anguish)", "15%", DamageSource.NoPets, 15, DamageType.StrikeAndCondition, DamageType.All, Source.Ritualist, ByPresence, SkillImages.Anguish, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_LingeringSpiritsAnguish, LingeringSpiritsAnguish, "Lingering Spirits (Anguish)", "10%", DamageSource.NoPets, 10, DamageType.StrikeAndCondition, DamageType.All, Source.Ritualist, ByPresence, SkillImages.Anguish, DamageModifierMode.sPvPWvW),
        // Explosive Growth
        new BuffOnActorDamageModifier(Mod_ExplosiveGrowth, ExplosiveGrowthBuff, "Explosive Growth", "10%", DamageSource.NoPets, 10, DamageType.StrikeAndCondition, DamageType.All, Source.Ritualist, ByPresence, SkillImages.ResilientWeapon, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_ExplosiveGrowth, ExplosiveGrowthBuff, "Explosive Growth", "7%", DamageSource.NoPets, 7, DamageType.StrikeAndCondition, DamageType.All, Source.Ritualist, ByPresence, SkillImages.ResilientWeapon, DamageModifierMode.sPvPWvW),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Ritualist's Shroud
        new BuffOnActorDamageModifier(Mod_RitualistsShroud, RitualistsShroud, "Ritualist's Shroud", "-33%", DamageSource.Incoming, -33, DamageType.StrikeAndCondition, DamageType.All, Source.Ritualist, ByPresence, SkillImages.RitualistShroud, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_RitualistsShroud, RitualistsShroud, "Ritualist's Shroud", "-50%", DamageSource.Incoming, -50, DamageType.StrikeAndCondition, DamageType.All, Source.Ritualist, ByPresence, SkillImages.RitualistShroud, DamageModifierMode.sPvPWvW),
        // Weapons
        new BuffOnActorDamageModifier(Mod_ResilientWeapon10, ResilientWeaponBuff, "Resilient Weapon", "-10% above 50% hp", DamageSource.Incoming, -10, DamageType.Strike, DamageType.All, Source.Ritualist, ByPresence, SkillImages.ResilientWeapon, DamageModifierMode.All)
            .UsingChecker((dhe, log) => !dhe.AgainstUnderFifty),
        new BuffOnActorDamageModifier(Mod_ResilientWeapon20, ResilientWeaponBuff, "Resilient Weapon", "-20% below 50% hp", DamageSource.Incoming, -20, DamageType.Strike, DamageType.All, Source.Ritualist, ByPresence, SkillImages.ResilientWeapon, DamageModifierMode.All)
            .UsingChecker((dhe, log) => dhe.AgainstUnderFifty),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Ritualist Shroud", RitualistsShroud, Source.Ritualist, BuffClassification.Other, SkillImages.RitualistShroud),
        new Buff("Painful Bond", PainfulBond, Source.Ritualist, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.Anguish),
        new Buff("Ritualist's Storm Spirit Aura (1)", RitualistStormSpiritAura1, Source.Ritualist, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Ritualist's Storm Spirit Aura (2)", RitualistStormSpiritAura2, Source.Ritualist, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Ritualist's Storm Spirit Aura (3)", RitualistStormSpiritAura3, Source.Ritualist, BuffClassification.Other, BuffImages.Unknown),
        // Weapons
        new Buff("Weapon of Remedy", WeaponOfRemedyBuff, Source.Ritualist, BuffClassification.Defensive, SkillImages.WeaponOfRemedy),
        new Buff("Weapon of Warding", WeaponOfWardingBuff, Source.Ritualist, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Defensive, SkillImages.WeaponOfWarding),
        new Buff("Nightmare Weapon", NightmareWeaponBuff, Source.Ritualist, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Offensive, SkillImages.NightmareWeapon),
        new Buff("Xinrae's Weapon", XinraesWeaponBuff, Source.Ritualist, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Defensive, SkillImages.XinraesWeapon),
        new Buff("Resilient Weapon", ResilientWeaponBuff, Source.Ritualist, BuffClassification.Defensive, SkillImages.ResilientWeapon),
        new Buff("Splinter Weapon", SplinterWeaponBuff, Source.Ritualist, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Offensive, SkillImages.SplinterWeapon),
        // Traits
        new Buff("Lingering Spirits", LingeringSpirits, Source.Ritualist, BuffClassification.Other, TraitImages.LingeringSpirits),
        new Buff("Lingering Spirits (Preservation)", LingeringSpiritsPreservation, Source.Ritualist, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.Preservation),
        new Buff("Lingering Spirits (Anguish)", LingeringSpiritsAnguish, Source.Ritualist, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.Anguish),
        new Buff("Lingering Spirits (WanderLust)", LingeringSpiritsWanderlust, Source.Ritualist, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.Wanderlust),
        new Buff("Explosive Growth", ExplosiveGrowthBuff, Source.Ritualist, BuffStackType.Queue, 10, BuffClassification.Other, TraitImages.ExplosiveGrowth),
        // Spirits
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
    internal static bool IsSpiritMinion(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return IsKnownMinionID(agentItem.ID);
    }

    internal static bool IsSummonedCreature(AgentItem agentItem)
    {
        return IsSpiritMinion(agentItem) || IsUndeadMinion(agentItem);
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
