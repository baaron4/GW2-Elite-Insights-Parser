using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;

namespace GW2EIEvtcParser.EIData
{
    internal static class ThiefHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(Shadowstep, Infiltration),
            new BuffLossCastFinder(ShadowReturn, Infiltration).UsingChecker((evt, combatData, agentData, skillData) => evt.RemovedDuration > ServerDelayConstant),
            new DamageCastFinder(Mug, Mug),
            new DamageCastFinder(InfiltratorsStrike, InfiltratorsStrike),
            new BuffGainCastFinder(AssassinsSignet, AssassinsSignetActive),
            new BuffGiveCastFinder(DevourerVenomSkill, DevourerVenomBuff),
            new BuffGiveCastFinder(IceDrakeVenomSkill, IceDrakeVenomBuff),
            new BuffGiveCastFinder(SkaleVenomSkill, SkaleVenomBuff),
            new BuffGiveCastFinder(SoulStoneVenomSkill,SoulStoneVenomBuff),
            new BuffGiveCastFinder(SpiderVenomSkill,SpiderVenomBuff).UsingChecker((evt, combatData, agentData, skillData) => evt.To != evt.By || Math.Abs(evt.AppliedDuration - 24000) < ServerDelayConstant).UsingNotAccurate(true), // same id as leeching venom trait?
            new EffectCastFinder(Pitfall, EffectGUIDs.ThiefPitfallAoE).UsingSrcBaseSpecChecker(Spec.Thief),
            new BuffLossCastFinder(ThousandNeedles, ThousandNeedlesArmedBuff)
                .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasRelatedEffect(EffectGUIDs.ThiefThousandNeedlesAoE1, evt.To, evt.Time + 280))
                .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasRelatedEffect(EffectGUIDs.ThiefThousandNeedlesAoE2, evt.To, evt.Time + 280))
                .UsingNotAccurate(true),
            new EffectCastFinder(SealArea, EffectGUIDs.ThiefSealAreaAoE).UsingSrcBaseSpecChecker(Spec.Thief),
            new BuffGainCastFinder(ShadowPortal, ShadowPortalOpenedBuff),
            new EffectCastFinderByDst(InfiltratorsSignetSkill, EffectGUIDs.ThiefInfiltratorsSignet1)
                .UsingDstBaseSpecChecker(Spec.Thief)
                .UsingSecondaryEffectChecker(EffectGUIDs.ThiefInfiltratorsSignet2),
            new EffectCastFinderByDst(SignetOfAgilitySkill, EffectGUIDs.ThiefSignetOfAgility).UsingDstBaseSpecChecker(Spec.Thief),
            new EffectCastFinderByDst(SignetOfShadowsSkill, EffectGUIDs.ThiefSignetOfShadows).UsingDstBaseSpecChecker(Spec.Thief),
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Deadly arts
            new BuffOnFoeDamageModifier(NumberOfConditions, "Exposed Weakness", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Thief, ByStack, BuffImages.ExposedWeakness, DamageModifierMode.All).WithBuilds(GW2Builds.July2018Balance),
            new BuffOnFoeDamageModifier(NumberOfConditions, "Exposed Weakness", "10% if condition on target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Thief, ByPresence, BuffImages.ExposedWeakness, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2018Balance),
            new DamageLogDamageModifier("Executioner", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Thief, BuffImages.Executioner, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All),
            // Critical Strikes
            new DamageLogDamageModifier("Twin Fangs","7% if hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Thief, BuffImages.FerociousStrikes, (x, log) => x.IsOverNinety && x.HasCrit, DamageModifierMode.All),
            new DamageLogDamageModifier("Ferocious Strikes", "10% on critical strikes if target >50%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Thief, BuffImages.FerociousStrikes, (x, log) => !x.AgainstUnderFifty && x.HasCrit, DamageModifierMode.All),
            // Trickery
            new BuffOnActorDamageModifier(LeadAttacks, "Lead Attacks", "1% (10s) per initiative spent", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Thief, ByStack, BuffImages.LeadAttacks, DamageModifierMode.All), 
            // It's not always possible to detect the presence of pistol and the trait is additive with itself. Staff master is worse as we can't detect endurance at all       
            new BuffOnActorDamageModifier(FluidStrikes, "Fluid Strikes", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Thief, ByPresence, BuffImages.FluidStrikes, DamageModifierMode.All).WithBuilds(GW2Builds.July2023BalanceAndSilentSurfCM),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new DamageLogDamageModifier("Marauder's Resilience", "-10% from foes within 360 range", DamageSource.NoPets, -10.0, DamageType.Strike, DamageType.All, Source.Thief, BuffImages.MaraudersResilience, (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 360.0;
            }, DamageModifierMode.All).UsingApproximate(true).WithBuilds(GW2Builds.April2019Balance)
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            // Skills
            new Buff("Shadow Portal (Prepared)", ShadowPortalPreparedBuff, Source.Thief, BuffClassification.Other, BuffImages.PrepareShadowPortal),
            new Buff("Shadow Portal (Open)", ShadowPortalOpenedBuff, Source.Thief, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.ShadowPortal),
            new Buff("Kneeling", Kneeling, Source.Thief, BuffClassification.Other, BuffImages.Kneel).WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            // Signets
            new Buff("Signet of Malice", SignetOfMalice, Source.Thief, BuffClassification.Other, BuffImages.SignetOfMalice),
            new Buff("Assassin's Signet (Passive)", AssassinsSignetPassive, Source.Thief, BuffClassification.Other, BuffImages.AssassinsSignet),
            new Buff("Assassin's Signet (Active)", AssassinsSignetActive, Source.Thief, BuffClassification.Other, BuffImages.AssassinsSignet),
            new Buff("Infiltrator's Signet", InfiltratorsSignetBuff, Source.Thief, BuffClassification.Other, BuffImages.InfiltratorsSignet),
            new Buff("Signet of Agility", SignetOfAgilityBuff, Source.Thief, BuffClassification.Other, BuffImages.SignetOfAgility),
            new Buff("Signet of Shadows", SignetOfShadowsBuff, Source.Thief, BuffClassification.Other, BuffImages.SignetOfShadows),
            // Venoms // src is always the user, makes generation data useless
            new Buff("Skelk Venom", SkelkVenom, Source.Thief, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Defensive, BuffImages.SkelkVenom),
            new Buff("Ice Drake Venom", IceDrakeVenomBuff, Source.Thief, BuffStackType.StackingConditionalLoss, 4, BuffClassification.Support, BuffImages.IceDrakeVenom),
            new Buff("Devourer Venom", DevourerVenomBuff, Source.Thief, BuffStackType.StackingConditionalLoss, 2, BuffClassification.Support, BuffImages.DevourerVenom),
            new Buff("Skale Venom", SkaleVenomBuff, Source.Thief, BuffStackType.StackingConditionalLoss, 4, BuffClassification.Offensive, BuffImages.SkaleVenom),
            new Buff("Spider Venom", SpiderVenomBuff, Source.Thief, BuffStackType.StackingConditionalLoss, 6, BuffClassification.Offensive, BuffImages.SpiderVenom),
            new Buff("Soul Stone Venom", SoulStoneVenomBuff, Source.Thief, BuffStackType.Stacking, 25, BuffClassification.Offensive, BuffImages.SoulStoneVenom),
            new Buff("Basilisk Venom", BasiliskVenomBuff, Source.Thief, BuffStackType.StackingConditionalLoss, 2, BuffClassification.Support, BuffImages.BasiliskVenom),
            new Buff("Petrified 1", Petrified1, Source.Thief, BuffClassification.Other, BuffImages.Stun),
            new Buff("Petrified 2", Petrified2, Source.Thief, BuffClassification.Other, BuffImages.Stun),
            new Buff("Infiltration", Infiltration, Source.Thief, BuffClassification.Other, BuffImages.Shadowstep),
            // Transforms
            new Buff("Dagger Storm", DaggerStorm, Source.Thief, BuffClassification.Other, BuffImages.DaggerStorm),
            // Traits
            new Buff("Hidden Killer", HiddenKiller, Source.Thief, BuffClassification.Other, BuffImages.Hiddenkiller),
            new Buff("Lead Attacks", LeadAttacks, Source.Thief, BuffStackType.Stacking, 15, BuffClassification.Other, BuffImages.LeadAttacks),
            new Buff("Instant Reflexes", InstantReflexes, Source.Thief, BuffClassification.Other, BuffImages.InstantReflexes),
            new Buff("Fluid Strikes", FluidStrikes, Source.Thief, BuffClassification.Other, BuffImages.FluidStrikes).WithBuilds(GW2Builds.July2023BalanceAndSilentSurfCM),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.Thief1,
            (int)MinionID.Thief2,
            (int)MinionID.Thief3,
            (int)MinionID.Thief4,
            (int)MinionID.Thief5,
            (int)MinionID.Thief6,
            (int)MinionID.Thief7,
            (int)MinionID.Thief8,
            (int)MinionID.Thief9,
            (int)MinionID.Thief10,
            (int)MinionID.Thief11,
            (int)MinionID.Thief12,
            (int)MinionID.Thief13,
            (int)MinionID.Thief14,
            (int)MinionID.Thief15,
            (int)MinionID.Thief16,
            (int)MinionID.Thief17,
            (int)MinionID.Thief18,
            (int)MinionID.Thief19,
            (int)MinionID.Thief20,
            (int)MinionID.Thief21,
            (int)MinionID.Thief22,
        };

        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Thief;

            // Shadow Portal locations
            var entranceDecorations = new List<GenericAttachedDecoration>();
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ThiefShadowPortalActiveEntrance, out IReadOnlyList<EffectEvent> shadowPortalActiveEntrance))
            {
                var skill = new SkillModeDescriptor(player, Spec.Thief, PrepareShadowPortal, SkillModeCategory.Portal);
                foreach (EffectEvent enter in shadowPortalActiveEntrance)
                {
                    (long, long) lifespan = enter.ComputeLifespan(log, 8000, player.AgentItem, ShadowPortalOpenedBuff);
                    var connector = new PositionConnector(enter.Position);
                    replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.5, connector).UsingSkillMode(skill));
                    GenericAttachedDecoration icon = new IconDecoration(ParserIcons.PortalShadowPortalPrepare, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, connector).UsingSkillMode(skill);
                    replay.Decorations.Add(icon);
                    entranceDecorations.Add(icon);
                }
            }
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ThiefShadowPortalActiveExit, out IReadOnlyList<EffectEvent> shadowPortalActiveExit))
            {
                foreach (EffectEvent exit in shadowPortalActiveExit)
                {
                    var skill = new SkillModeDescriptor(player, Spec.Thief, ShadowPortal, SkillModeCategory.Portal); 
                    (long, long) lifespan = exit.ComputeLifespan(log, 8000, player.AgentItem, ShadowPortalOpenedBuff);
                    var connector = new PositionConnector(exit.Position);
                    replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.5, connector).UsingSkillMode(skill));
                    GenericAttachedDecoration icon = new IconDecoration(ParserIcons.PortalShadowPortalOpen, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, connector).UsingSkillMode(skill);
                    GenericAttachedDecoration entranceDecoration = entranceDecorations.FirstOrDefault(x => Math.Abs(x.Lifespan.start - exit.Time) < ServerDelayConstant);
                    if (entranceDecoration != null)
                    {
                        replay.Decorations.Add(entranceDecoration.LineTo(icon, color, 0.5).UsingSkillMode(skill));
                    }
                    replay.Decorations.Add(icon);
                }
            }

            // Seal Area
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ThiefSealAreaAoE, out IReadOnlyList<EffectEvent> sealAreaAoEs))
            {
                var skill = new SkillModeDescriptor(player, Spec.Thief, SealArea, SkillModeCategory.ProjectileManagement);
                foreach (EffectEvent effect in sealAreaAoEs)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 8000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectSealArea, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            // Shadow Refuge
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ThiefShadowRefuge, out IReadOnlyList<EffectEvent> shadowRefuges))
            {
                var skill = new SkillModeDescriptor(player, Spec.Thief, ShadowRefuge, SkillModeCategory.ImportantBuffs | SkillModeCategory.Heal);
                foreach (EffectEvent effect in shadowRefuges)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectShadowRefuge, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
        }
    }
}
