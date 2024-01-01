using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class VirtuosoHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new EffectCastFinder(BladesongDistortion, EffectGUIDs.MesmerDistortionOrMindWrack).UsingChecker((evt, combatData, agentData, skillData) => {
                if(evt.Src.Spec != Spec.Virtuoso) {
                    return false;
                }
                if (!combatData.GetBuffData(DistortionBuff).Any(buffEvt => buffEvt is BuffApplyEvent && buffEvt.To == evt.Src && Math.Abs(buffEvt.Time - evt.Time) < ServerDelayConstant))
                {
                    return false;
                }
                if (combatData.GetAnimatedCastData(BladeRenewal).Any(castEvt => castEvt.Caster == evt.Src && evt.Time <= castEvt.EndTime && evt.Time >= castEvt.Time)) {
                    return false;
                }
                return true;
            }).WithBuilds(GW2Builds.October2022Balance),
            new EffectCastFinder(BladeturnRequiem, EffectGUIDs.VirtuosoBladeturnRequiem).UsingSrcSpecChecker(Spec.Virtuoso).WithBuilds(GW2Builds.June2023Balance),
            new EffectCastFinder(ThousandCuts, EffectGUIDs.VirtuosoThousandCuts).UsingSrcSpecChecker(Spec.Virtuoso),
        };


        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new DamageLogDamageModifier("Mental Focus", "10% to foes within 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Virtuoso, BuffImages.MentalFocus, (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 600;
            }, DamageModifierMode.PvE).UsingApproximate(true).WithBuilds(GW2Builds.EODBeta4),
            new BuffOnActorDamageModifier(DeadlyBlades, "Deadly Blades", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Virtuoso, ByPresence, BuffImages.DeadlyBlades, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Deadly Blades", DeadlyBlades, Source.Virtuoso, BuffClassification.Other, BuffImages.DeadlyBlades),
            new Buff("Bladeturn", Bladeturn, Source.Virtuoso, BuffClassification.Other, BuffImages.BladeturnRequiem),
            new Buff("Virtuoso Blade", VirtuosoBlades, Source.Virtuoso, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.PowerAttribute),
        };

        public static List<AbstractBuffEvent> TransformVirtuosoBladeStorage(IReadOnlyList<AbstractBuffEvent> buffs, AgentItem a, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            var bladeIDs = new HashSet<long>
            {
                VirtuosoBlade1,
                VirtuosoBlade2,
                VirtuosoBlade3,
                VirtuosoBlade4,
                VirtuosoBlade5,
            };
            var blades = buffs.Where(x => bladeIDs.Contains(x.BuffID)).ToList();
            SkillItem skill = skillData.Get(VirtuosoBlades);
            var lastAddedBuffInstance = new Dictionary<long, uint>();
            foreach (AbstractBuffEvent blade in blades)
            {
                if (blade is BuffApplyEvent bae)
                {
                    res.Add(new BuffApplyEvent(bae.By, a, bae.Time, bae.AppliedDuration, skill, bae.IFF, bae.BuffInstance, true));
                    lastAddedBuffInstance[blade.BuffID] = bae.BuffInstance;
                }
                else if (blade is BuffRemoveAllEvent brae)
                {
                    if (!lastAddedBuffInstance.TryGetValue(blade.BuffID, out uint removedInstance))
                    {
                        removedInstance = 0;
                    }
                    res.Add(new BuffRemoveSingleEvent(brae.By, a, brae.Time, brae.RemovedDuration, skill, brae.IFF, removedInstance));
                }
                else if (blade is BuffRemoveSingleEvent brse)
                {
                    res.Add(new BuffRemoveSingleEvent(brse.By, a, brse.Time, brse.RemovedDuration, skill, brse.IFF, brse.BuffInstance));
                }
            }
            return res;
        }

        private static HashSet<int> Minions = new HashSet<int>();
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Mesmer;

            // Rain of Swords
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.VirtuosoRainOfSwords, out IReadOnlyList<EffectEvent> rainOfSwords))
            {
                var skill = new SkillModeDescriptor(player, Spec.Virtuoso, RainOfSwords);
                foreach (EffectEvent effect in rainOfSwords)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 6000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(280, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectRainOfSwords, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            // Thousand Cuts
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.VirtuosoThousandCuts, out IReadOnlyList<EffectEvent> thousandCuts))
            {
                var skill = new SkillModeDescriptor(player, Spec.Virtuoso, ThousandCuts);
                foreach (EffectEvent effect in thousandCuts)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    var connector = (PositionConnector)new PositionConnector(effect.Position).WithOffset(new Point3D(0f, 600.0f), true);
                    var rotationConnector = new AngleConnector(effect.Rotation.Z);
                    // 30 units width is a guess
                    replay.Decorations.Add(new RectangleDecoration(30, 1200, lifespan, color, 0.5, connector)
                        .UsingRotationConnector(rotationConnector)
                        .UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectThousandCuts, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector)
                        .UsingRotationConnector(rotationConnector)
                        .UsingSkillMode(skill));
                }
            }
        }
    }
}
