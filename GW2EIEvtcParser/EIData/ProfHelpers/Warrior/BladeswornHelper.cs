using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class BladeswornHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffLossCastFinder(GunsaberSheath, GunsaberMode)
            .WithBuilds(GW2Builds.EODBeta2)
            .UsingBeforeWeaponSwap(true),
        new BuffGainCastFinder(Gunsaber, GunsaberMode)
            .WithBuilds(GW2Builds.EODBeta2)
            .UsingBeforeWeaponSwap(true),
        new DamageCastFinder(UnseenSword, UnseenSword)
            .WithBuilds(GW2Builds.EODBeta2)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new BuffGainCastFinder(FlowStabilizer, PositiveFlow)
            .UsingChecker((bae, combatData, agentData, skillData) =>
            {
                return 2 == CombatData.FindRelatedEvents(combatData.GetBuffDataByIDByDst(PositiveFlow, bae.To).OfType<BuffApplyEvent>(), bae.Time).Count(apply => apply.By == bae.To);
            }),
        new EffectCastFinder(DragonspikeMineSkill, EffectGUIDs.BladeswornDragonspikeMine)
            .UsingSrcSpecChecker(Spec.Bladesworn),
    ];

    private static readonly HashSet<long> _gunsaberForm =
    [
        GunsaberSheath, Gunsaber,
    ];

    public static bool IsGunsaberForm(long id)
    {
        return _gunsaberForm.Contains(id);
    }

    private static readonly HashSet<long> _gunsaberFormAAs =
    [
        SwiftCut,
        SteelDivide,
        ExplosiveThrust
    ];

    public static bool IsAutoAttack(ParsedEvtcLog log, long id)
    {
        var build = log.CombatData.GetGW2BuildEvent().Build;
        return build >= GW2Builds.EODBeta1 && _gunsaberFormAAs.Contains(id);
    }

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Fierce as Fire
        new BuffOnActorDamageModifier(Mod_FierceAsFire, FierceAsFire, "Fierce as Fire", "1%", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Bladesworn, ByStack, TraitImages.FierceAsFire, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta4),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Gunsaber Mode", GunsaberMode, Source.Bladesworn, BuffClassification.Other, SkillImages.UnsheatheGunsaber),
        new Buff("Dragon Trigger", DragonTrigger, Source.Bladesworn, BuffClassification.Other, SkillImages.DragonTrigger),
        new Buff("Positive Flow", PositiveFlow, Source.Bladesworn, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.AttributeBonus),
        new Buff("Fierce as Fire", FierceAsFire, Source.Bladesworn, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.FierceAsFire),
        new Buff("Stim State", StimState, Source.Bladesworn, BuffClassification.Other, SkillImages.CombatStimulant),
        new Buff("Guns and Glory", GunsAndGlory, Source.Bladesworn, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.GunsAndGlory),
        new Buff("Tactical Reload", TacticalReload, Source.Bladesworn, BuffClassification.Other, SkillImages.TacticalReload),
        new Buff("Overcharged Cartridges", OverchargedCartridgesBuff, Source.Bladesworn, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.OverchargedCartridges)
            .WithBuilds(GW2Builds.June2022Balance),
    ];


}
