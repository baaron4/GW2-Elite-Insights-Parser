using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class RenegadeHelper
{
    private class BandTogetherCastFinder : EffectCastFinder
    {
        public BandTogetherCastFinder(long baseSkillID, long enhancedSkill, GUID effect) : base(enhancedSkill, effect)
        {
            UsingSrcSpecChecker(Spec.Renegade);
            UsingChecker((evt, combatData, agentData, skillData) => !combatData.IsCasting(baseSkillID, evt.Src, evt.Time));
        }
    }

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(LegendaryRenegadeStanceSkill, LegendaryRenegadeStanceBuff),
        new DamageCastFinder(CallOfTheRenegade, CallOfTheRenegade),
        new EffectCastFinder(OrdersFromAbove, EffectGUIDs.RenegadeOrdersFromAboveRighteousRebel)
            .UsingSrcSpecChecker(Spec.Renegade),
        new EffectCastFinder(OrdersFromAbove, EffectGUIDs.RenegadeOrdersFromAbove)
            .UsingSrcSpecChecker(Spec.Renegade),
        new BandTogetherCastFinder(BreakrazorsBastionSkill, BreakrazorsBastionSkillEnhanced, EffectGUIDs.RenegadeBreakrazorsBastion),
        new BandTogetherCastFinder(RazorclawsRageSkill, RazorclawsRageSkillEnhanced, EffectGUIDs.RenegadeRazorclawsRage),
        new BandTogetherCastFinder(DarkrazorsDaringSkill, DarkrazorsDaringSkillEnhanced, EffectGUIDs.RenegadeDarkrazorsDaring),
        new BandTogetherCastFinder(IcerazorsIreSkill, IcerazorsIreSkillEnhanced, EffectGUIDs.RenegadeIcerazorsIre),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_KallasFervor, KallasFervor, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, TraitImages.KallasFervor, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_KallasFervor, KallasFervor, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, TraitImages.KallasFervor, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance),
        //
        new BuffOnActorDamageModifier(Mod_ImprovedKallasFervor, ImprovedKallasFervor, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, TraitImages.KallasFervor, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_ImprovedKallasFervor, ImprovedKallasFervor, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, TraitImages.KallasFervor, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.April2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_ImprovedKallasFervor, ImprovedKallasFervor, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, TraitImages.KallasFervor, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.April2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_ImprovedKallasFervorConditionLifeLeech, ImprovedKallasFervor, "Improved Kalla's Fervor (Condition and Lifeleech)", "3% per stack", DamageSource.NoPets, 3.0, DamageType.ConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, TraitImages.KallasFervor, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.April2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_ImprovedKallasFervorStrike, ImprovedKallasFervor, "Improved Kalla's Fervor (Strike)", "5% per stack", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Renegade, ByStack, TraitImages.KallasFervor, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.April2025BalancePatch),
        //
        new SkillDamageModifier(Mod_SoulcleavesSummit, "Soulcleave's Summit", "per hit (no ICD)", SoulcleavesSummitBuff, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, SkillImages.SoulcleavesSummit, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new SkillDamageModifier(Mod_SoulcleavesSummit, "Soulcleave's Summit", "per hit (1s ICD per target)", SoulcleavesSummitBuff, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, SkillImages.SoulcleavesSummit, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance),
        //
        new BuffOnActorDamageModifier(Mod_AllForOne, AllForOne, "All for One", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Renegade, ByPresence, TraitImages.AllForOne, DamageModifierMode.All)
            .WithBuilds(GW2Builds.June2024Balance, GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_AllForOne, AllForOne, "All for One", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Renegade, ByPresence, TraitImages.AllForOne, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_AllForOne, AllForOne, "All for One", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Renegade, ByPresence, TraitImages.AllForOne, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2025BalancePatch),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_BreakrazorsBastion, BreakrazorsBastionBuff, "Breakrazor's Bastion", "-50%", DamageSource.Incoming, -50.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, SkillImages.BreakrazorsBastion, DamageModifierMode.All),
        //
        new BuffOnActorDamageModifier(Mod_RighteousRebel, KallasFervor, "Righteous Rebel", "-33%", DamageSource.Incoming, -33.0, DamageType.Condition, DamageType.All, Source.Renegade, ByPresence, TraitImages.RighteousRebel, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance),
        new BuffOnActorDamageModifier(Mod_RighteousRebel, KallasFervor, "Righteous Rebel", "-7% per stack", DamageSource.Incoming, -7.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, TraitImages.RighteousRebel, DamageModifierMode.All)
            .WithBuilds(GW2Builds.October2018Balance, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_RighteousRebel, KallasFervor, "Righteous Rebel", "-4% per stack", DamageSource.Incoming, -4.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, TraitImages.RighteousRebel, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Legendary Renegade Stance", LegendaryRenegadeStanceBuff, Source.Renegade, BuffClassification.Other, SkillImages.LegendaryRenegadeStance),
        new Buff("Breakrazor's Bastion", BreakrazorsBastionBuff, Source.Renegade, BuffClassification.Defensive, SkillImages.BreakrazorsBastion),
        new Buff("Razorclaw's Rage", RazorclawsRageBuff, Source.Renegade, BuffClassification.Offensive, SkillImages.RazorclawsRage),
        new Buff("Soulcleave's Summit", SoulcleavesSummitBuff, Source.Renegade, BuffClassification.Offensive, SkillImages.SoulcleavesSummit),
        new Buff("Kalla's Fervor", KallasFervor, Source.Renegade, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.KallasFervor),
        new Buff("Improved Kalla's Fervor", ImprovedKallasFervor, Source.Renegade, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.KallasFervor),
        new Buff("All for One", AllForOne, Source.Renegade, BuffStackType.Queue, 99, BuffClassification.Other, TraitImages.AllForOne),
    ];

    public static bool IsLegendSwap(long id)
    {
        return LegendaryRenegadeStanceSkill == id;
    }

    private static readonly HashSet<int> Minions =
    [
        (int)MinionID.JasRazorclaw,
        (int)MinionID.ViskIcerazor,
        (int)MinionID.KusDarkrazor,
        (int)MinionID.EraBreakrazor,
        (int)MinionID.OfelaSoulcleave,
    ];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Revenant;

        // Citadel Bombardment
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RenegadeCitadelBombardmentPortal, out var citadelBombardment))
        {
            var skill = new SkillModeDescriptor(player, Spec.Revenant, CitadelBombardment);
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RenegadeCitadelBombardment1, out var citadelBombardmentHits))
            {
                foreach (EffectEvent effect in citadelBombardment)
                {
                    if (player.AgentItem.TryGetCurrentPosition(log, effect.Time, out var playerPosition))
                    {
                        var playerPositionConnector = new PositionConnector(playerPosition);
                        var positions = new List<Vector3>();
                        foreach (EffectEvent hitEffect in citadelBombardmentHits.Where(x => x.Time >= effect.Time && x.Time <= effect.Time + 3000))
                        {
                            positions.Add(hitEffect.Position);

                            // Shooting Animation
                            long animationDuration = hitEffect.Time - effect.Time;
                            (long start, long end) lifespanAnimation = (effect.Time, effect.Time + animationDuration);
                            var startPoint = new ParametricPoint3D(playerPosition, lifespanAnimation.start);
                            var endPoint = new ParametricPoint3D(hitEffect.Position, lifespanAnimation.end);
                            var rotationConnector = new AngleConnector(effect.Rotation.Z);
                            var shootingArrow = (RectangleDecoration)new RectangleDecoration(15, 100, lifespanAnimation, color, 0.5, new InterpolationConnector(new List<ParametricPoint3D>() { startPoint, endPoint }))
                                .UsingRotationConnector(rotationConnector)
                                .UsingSkillMode(skill);
                            replay.Decorations.Add(shootingArrow);

                            // Hit circles
                            (long, long) lifespanHit = hitEffect.ComputeLifespan(log, 500);
                            var connector = new PositionConnector(hitEffect.Position);
                            replay.Decorations.Add(new CircleDecoration(120, lifespanHit, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                        }
                        if (positions.Count > 0)
                        {
                            // AoE Radius and icons
                            (long, long) lifespan = (effect.Time, effect.Time + 3000);
                            var centralConnector = new PositionConnector(positions.Average());
                            replay.Decorations.Add(new IconDecoration(EffectImages.EffectCitadelBombardmentPortal, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, playerPositionConnector)
                                .UsingSkillMode(skill));
                            replay.Decorations.Add(new IconDecoration(EffectImages.EffectCitadelBombardment, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, centralConnector)
                                .UsingSkillMode(skill));
                            // TODO: Find a way to tell the user that the circle is approximative.
                            //replay.Decorations.Add(new CircleDecoration(230, lifespan, color, 0.5, centralConnector).UsingFilled(false).UsingSkillMode(skill));
                        }
                    }
                }
            }
        }
    }
}
