using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class RenegadeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(LegendaryRenegadeStanceSkill, LegendaryRenegadeStanceBuff),
            new DamageCastFinder(CallOfTheRenegade, CallOfTheRenegade),
            new EffectCastFinder(OrdersFromAbove, EffectGUIDs.RenegadeOrdersFromAboveRighteousRebel)
                .UsingSrcSpecChecker(Spec.Renegade),
            new EffectCastFinder(OrdersFromAbove, EffectGUIDs.RenegadeOrdersFromAbove)
                .UsingSrcSpecChecker(Spec.Renegade)
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(KallasFervor, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, BuffImages.KallasFervor, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(ImprovedKallasFervor, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, BuffImages.KallasFervor, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(KallasFervor, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, BuffImages.KallasFervor, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(ImprovedKallasFervor, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, BuffImages.KallasFervor, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
            new SkillDamageModifier("Soulcleave's Summit", "per hit (no ICD)", SoulcleavesSummitBuff, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, BuffImages.SoulcleavesSummit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new SkillDamageModifier("Soulcleave's Summit", "per hit (1s ICD per target)", SoulcleavesSummitBuff, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, BuffImages.SoulcleavesSummit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2021Balance),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(BreakrazorsBastionBuff, "Breakrazor's Bastion", "-50%", DamageSource.NoPets, -50.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.BreakrazorsBastion, DamageModifierMode.All),
            new BuffOnActorDamageModifier(KallasFervor, "Righteous Rebel", "-33%", DamageSource.NoPets, -33.0, DamageType.Condition, DamageType.All, Source.Renegade, ByPresence, BuffImages.RighteousRebel, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance),
            new BuffOnActorDamageModifier(KallasFervor, "Righteous Rebel", "-7% per stack", DamageSource.NoPets, -7.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, BuffImages.RighteousRebel, DamageModifierMode.All).WithBuilds(GW2Builds.October2018Balance, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(KallasFervor, "Righteous Rebel", "-4% per stack", DamageSource.NoPets, -4.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, BuffImages.RighteousRebel, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Legendary Renegade Stance", LegendaryRenegadeStanceBuff, Source.Renegade, BuffClassification.Other, BuffImages.LegendaryRenegadeStance),
            new Buff("Breakrazor's Bastion", BreakrazorsBastionBuff, Source.Renegade, BuffClassification.Defensive, BuffImages.BreakrazorsBastion),
            new Buff("Razorclaw's Rage", RazorclawsRageBuff, Source.Renegade, BuffClassification.Offensive, BuffImages.RazorclawsRage),
            new Buff("Soulcleave's Summit", SoulcleavesSummitBuff, Source.Renegade, BuffClassification.Offensive, BuffImages.SoulcleavesSummit),
            new Buff("Kalla's Fervor", KallasFervor, Source.Renegade, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.KallasFervor),
            new Buff("Improved Kalla's Fervor", ImprovedKallasFervor, Source.Renegade, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.KallasFervor),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.JasRazorclaw,
            (int)MinionID.ViskIcerazor,
            (int)MinionID.KusDarkrazor,
            (int)MinionID.EraBreakrazor,
            (int)MinionID.OfelaSoulcleave,
        };
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Revenant;

            // Citadel Bombardment
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RenegadeCitadelBombardmentPortal, out IReadOnlyList<EffectEvent> citadelBombardment))
            {
                var skill = new SkillModeDescriptor(player, Spec.Revenant, CitadelBombardment);
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RenegadeCitadelBombardment1, out IReadOnlyList<EffectEvent> citadelBombardmentHits))
                {
                    foreach (EffectEvent effect in citadelBombardment)
                    {
                        Point3D playerPosition = player.AgentItem.GetCurrentPosition(log, effect.Time);
                        if (playerPosition != null)
                        {
                            var playerPositionConnector = new PositionConnector(playerPosition);
                            var positions = new List<Point3D>();
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
                            // AoE Radius and icons
                            (long, long) lifespan = (effect.Time, effect.Time + 3000);
                            var centralPoint = Point3D.FindCentralPoint(positions);
                            var centralConnector = new PositionConnector(centralPoint);
                            replay.Decorations.Add(new IconDecoration(ParserIcons.EffectCitadelBombardmentPortal, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, playerPositionConnector)
                                .UsingSkillMode(skill));
                            replay.Decorations.Add(new IconDecoration(ParserIcons.EffectCitadelBombardment, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, centralConnector)
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
