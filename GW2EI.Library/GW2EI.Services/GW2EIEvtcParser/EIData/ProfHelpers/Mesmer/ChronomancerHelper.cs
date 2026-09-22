using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class ChronomancerHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(ContinuumSplit, TimeAnchored),
        new BuffLossCastFinder(ContinuumShift, TimeAnchored),
        /*
        new EffectCastFinder(SplitSecond, EffectGUIDs.ChronomancerSplitSecond)
            .UsingSecondaryEffectSameSrcChecker(EffectGUIDs.ChronomancerSeizeTheMomentShatter)
            .UsingSrcSpecChecker(Spec.Chronomancer),
        new EffectCastFinder(Rewinder, EffectGUIDs.ChronomancerRewinder)
            .UsingSecondaryEffectSameSrcChecker(EffectGUIDs.ChronomancerSeizeTheMomentShatter)
            .UsingSrcSpecChecker(Spec.Chronomancer),
        new EffectCastFinder(TimeSink, EffectGUIDs.ChronomancerTimeSink)
            .UsingSecondaryEffectSameSrcChecker(EffectGUIDs.ChronomancerSeizeTheMomentShatter)
            .UsingSrcSpecChecker(Spec.Chronomancer),
        */
        new DamageCastFinder(TimeBombDamage, TimeBombDamage),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Danger Time
        new BuffOnFoeDamageModifier(Mod_DangerTime, Slow, "Danger Time", "10% crit damage on slowed target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.DangerTime, DamageModifierMode.All)
            .UsingChecker((x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.February2018Balance, GW2Builds.December2018Balance),
        new BuffOnFoeDamageModifier(Mod_DangerTime, Slow, "Danger Time", "10% crit damage on slowed target", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.DangerTime, DamageModifierMode.All)
            .UsingChecker((x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.December2018Balance, GW2Builds.May2021Balance),
        new BuffOnFoeDamageModifier(Mod_DangerTime, Slow, "Danger Time", "15% crit damage on slowed target", DamageSource.All, 15.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.DangerTime, DamageModifierMode.All)
            .UsingChecker((x, log) => x.HasCrit)
            .UsingChecker(MesmerHelper.IllusionsWithMesmerChecker)
            .WithBuilds(GW2Builds.June2025Balance, GW2Builds.January2026Balance),
        new BuffOnActorDamageModifier(Mod_DangerTime, DangerTime, "Danger Time", "10%", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.DangerTime, DamageModifierMode.All)
            .UsingChecker((x, log) => x.HasCrit)
            .UsingChecker(MesmerHelper.IllusionsWithMesmerChecker)
            .UsingActorFetchIsAlwaysMaster()
            .WithBuilds(GW2Builds.January2026Balance, GW2Builds.April2026Balancepocalypse),
        new BuffOnActorDamageModifier(Mod_DangerTime, DangerTime, "Danger Time", "10%", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.DangerTime, DamageModifierMode.sPvPWvW)
            .UsingChecker((x, log) => x.HasCrit)
            .UsingChecker(MesmerHelper.IllusionsWithMesmerChecker)
            .UsingActorFetchIsAlwaysMaster()
            .WithBuilds(GW2Builds.April2026Balancepocalypse),
        new BuffOnActorDamageModifier(Mod_DangerTime, DangerTime, "Danger Time", "5%", DamageSource.All, 5.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.DangerTime, DamageModifierMode.PvE)
            .UsingChecker((x, log) => x.HasCrit)
            .UsingChecker(MesmerHelper.IllusionsWithMesmerChecker)
            .UsingActorFetchIsAlwaysMaster()
            .WithBuilds(GW2Builds.April2026Balancepocalypse),
        // Time Bomb
        new BuffOnFoeDamageModifier(Mod_TimeBomb, TimeBombBuff, "Time Bomb", "15%", DamageSource.All, 15.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.TimeBomb, DamageModifierMode.All)
            .WithBuffOnFoeFromActor()
            .UsingEarlyExit((a, log) => log.CombatData.GetBuffApplyDataByIDBySrc(TimeBombBuff, a.AgentItem).Count == 0)
            .WithBuilds(GW2Builds.January2026Balance),
        // Improved Alacrity
        new BuffOnActorDamageModifier(Mod_ImprovedAlacrity, Alacrity, "Improved Alacrity", "10% crit under alacrity", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.ImprovedAlacrity, DamageModifierMode.All)
            .UsingChecker((x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.August2022BalanceHotFix, GW2Builds.June2025Balance),
        // Chronophantasma
        new BuffOnActorDamageModifier(Mod_Chronophantasma, ChronophantasmaResummonBuff, "Chronophantasma", "Phantasm resummon (100%)", DamageSource.PetsOnly, 100.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.Chronophantasma, DamageModifierMode.All)
            .UsingEarlyExit((a, log) => !a.GetMinions(log).Any(x => MesmerHelper.IsPhantasm(x.ReferenceAgentItem)))
            .UsingChecker(MesmerHelper.PhantasmsChecker)
            .WithBuilds(GW2Builds.February2018Balance, GW2Builds.May2018Balance),
        new BuffOnActorDamageModifier(Mod_Chronophantasma, ChronophantasmaResummonBuff, "Chronophantasma", "Phantasm resummon (50%)", DamageSource.PetsOnly, 100.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.Chronophantasma, DamageModifierMode.sPvPWvW)
            .UsingEarlyExit((a, log) => !a.GetMinions(log).Any(x => MesmerHelper.IsPhantasm(x.ReferenceAgentItem)))
            .UsingChecker(MesmerHelper.PhantasmsChecker)
            .WithBuilds(GW2Builds.May2018Balance),
        new BuffOnActorDamageModifier(Mod_Chronophantasma, ChronophantasmaResummonBuff, "Chronophantasma", "Phantasm resummon (100%)", DamageSource.PetsOnly, 100.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.Chronophantasma, DamageModifierMode.PvE)
            .UsingEarlyExit((a, log) => !a.GetMinions(log).Any(x => MesmerHelper.IsPhantasm(x.ReferenceAgentItem)))
            .UsingChecker(MesmerHelper.PhantasmsChecker)
            .WithBuilds(GW2Builds.May2018Balance, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_Chronophantasma, ChronophantasmaResummonBuff, "Chronophantasma", "Phantasm resummon (75%)", DamageSource.PetsOnly, 100.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.Chronophantasma, DamageModifierMode.PvE)
            .UsingEarlyExit((a, log) => !a.GetMinions(log).Any(x => MesmerHelper.IsPhantasm(x.ReferenceAgentItem)))
            .UsingChecker(MesmerHelper.PhantasmsChecker)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.November2022Balance),
        new BuffOnActorDamageModifier(Mod_Chronophantasma, ChronophantasmaResummonBuff, "Chronophantasma", "Phantasm resummon (100%)", DamageSource.PetsOnly, 100.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.Chronophantasma, DamageModifierMode.PvE)
            .UsingEarlyExit((a, log) => !a.GetMinions(log).Any(x => MesmerHelper.IsPhantasm(x.ReferenceAgentItem)))
            .UsingChecker(MesmerHelper.PhantasmsChecker)
            .WithBuilds(GW2Builds.November2022Balance, GW2Builds.April2025Balance),
        new BuffOnActorDamageModifier(Mod_Chronophantasma, ChronophantasmaResummonBuff, "Chronophantasma", "Phantasm resummon (115%)", DamageSource.PetsOnly, 100.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.Chronophantasma, DamageModifierMode.PvE)
            .UsingEarlyExit((a, log) => !a.GetMinions(log).Any(x => MesmerHelper.IsPhantasm(x.ReferenceAgentItem)))
            .UsingChecker(MesmerHelper.PhantasmsChecker)
            .WithBuilds(GW2Builds.April2025Balance, GW2Builds.April2026Balancepocalypse),
        new BuffOnActorDamageModifier(Mod_Chronophantasma, ChronophantasmaResummonBuff, "Chronophantasma", "Phantasm resummon (105%)", DamageSource.PetsOnly, 100.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.Chronophantasma, DamageModifierMode.PvE)
            .UsingEarlyExit((a, log) => !a.GetMinions(log).Any(x => MesmerHelper.IsPhantasm(x.ReferenceAgentItem)))
            .UsingChecker(MesmerHelper.PhantasmsChecker)
            .WithBuilds(GW2Builds.April2026Balancepocalypse),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];


    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Time Echo", TimeEcho, Source.Chronomancer, BuffClassification.Other, SkillImages.DejaVu)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Time Anchored", TimeAnchored, Source.Chronomancer, BuffStackType.Queue, 25, BuffClassification.Other, SkillImages.ContinuumSplit),
        new Buff("Danger Time", DangerTime, Source.Chronomancer, BuffClassification.Other, TraitImages.DangerTime),
        new Buff("Time Bomb", TimeBombBuff, Source.Chronomancer, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Debuff, TraitImages.TimeBomb),
        new Buff("Temporal Stasis", TemporalStasis, Source.Chronomancer, BuffClassification.Debuff, BuffImages.Stun),
        new Buff("Chronophantasma", ChronophantasmaBuff, Source.Chronomancer, BuffClassification.Other, TraitImages.Chronophantasma),
        new Buff("Chronophantasma Resummon", ChronophantasmaResummonBuff, Source.Chronomancer, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, TraitImages.Chronophantasma),
    ];

    private static readonly HashSet<int> NonCloneMinions = [];
    internal static bool IsKnownMinionID(int id)
    {
        return NonCloneMinions.Contains(id);
    }

    internal static List<CastEvent> ComputeChronomancerShatters(AgentItem player, CombatData combatData, SkillData skillData, IReadOnlyList<AgentItem> clones)
    {
        var res = new List<CastEvent>();
        if (combatData.TryGetEffectEventsBySrcWithGUIDs(player, [EffectGUIDs.ChronomancerSplitSecond, EffectGUIDs.ChronomancerRewinder, EffectGUIDs.ChronomancerTimeSink], out var shatters))
        {
            if (!combatData.TryGetEffectEventsBySrcWithGUID(player, EffectGUIDs.MesmerThePrestigeDisappear2AndShatterAroundClonesAndChrono, out var chronoShatters))
            {
                return res;
            }
            if (!combatData.TryGetEffectEventsBySrcWithGUID(player, EffectGUIDs.ChronomancerSeizeTheMomentShatter, out var boonGivingShatters))
            {
                boonGivingShatters = [];
            }
            var skillDict = new Dictionary<Guid, SkillItem>()
            {
                { EffectGUIDs.ChronomancerSplitSecond, skillData.Get(SplitSecondOrSplitSecondAmmo)},
                { EffectGUIDs.ChronomancerRewinder, skillData.Get(Rewinder)},
                { EffectGUIDs.ChronomancerTimeSink, skillData.Get(TimeSink)},
            };
            HashSet<long> shatterSkillIDs = [SplitSecond, SplitSecondAmmo, Rewinder, TimeSink];
            skillData.NotAccurate.UnionWith([SplitSecondOrSplitSecondAmmo, SplitSecond, SplitSecondAmmo, Rewinder, TimeSink]);
            shatters.SortByTime();
            shatters.Reverse();
            var pClones = clones
                .Where(player.IsMasterOf)
                .ToList();
            var cloneKillingBlowsDict = pClones
                .Select(clone => (clone, combatData.GetDamageTakenData(clone)
                    .Where(y => y.HasKilled).ToList()))
                .Where(x => x.Item2.Count > 0)
                .ToDictionary(x => x.clone, x => x.Item2);
            // We keep clones with dead events but without killing blows or killing blows with relevant skill ids
            var pClonesDead = pClones
                .Where(x => !cloneKillingBlowsDict.TryGetValue(x, out var killingBlows) || killingBlows.Any(y => shatterSkillIDs.Contains(y.SkillID)))
                .Select(x => combatData.GetDeadEvents(x).LastOrDefault())
                .Where(x => x != null)
                .ToList();
            pClonesDead.Sort((x, y) => x!.Time.CompareTo(y!.Time));
            foreach (var shatter in shatters)
            {
                var boonGivingShattersInFrame = boonGivingShatters
                    .Where(x => Math.Abs(x.Time - shatter.Time) < ServerDelayConstant)
                    .ToList();
                var skill = skillDict[shatter.GUIDEvent.GUID];
                HashSet<long> skillIDs;
                // If split second, determine either normal or shatter storm (ammo)
                if (skill.ID == SplitSecondOrSplitSecondAmmo)
                {
                    skillIDs = [SplitSecond, SplitSecondAmmo];
                    if (combatData.GetDamageData(SplitSecondAmmo).Any(x => x.CreditedFrom.Is(player) && Math.Abs(x.Time - shatter.Time) < 2000))
                    {
                        skill = skillData.Get(SplitSecondAmmo);
                    }
                    else if (combatData.GetDamageData(SplitSecond).Any(x => x.CreditedFrom.Is(player) && Math.Abs(x.Time - shatter.Time) < 2000))
                    {
                        skill = skillData.Get(SplitSecond);
                    }
                }
                else
                {
                    skillIDs = [skill.ID];
                }
                // If boon trait is equipped, we can safely use that, use position equality between the two effects
                if (boonGivingShattersInFrame.Any(x =>
                        (x.Position.XY() - shatter.Position.XY()).LengthSquared() < 1e-6)
                    )
                {
                    res.Add(new InstantCastEvent(shatter.Time, skill, shatter.Src));
                }
                else
                {
                    if (boonGivingShattersInFrame.Count > 0)
                    {
                        continue;
                    }
                    // Find dead clone in window, without killing blow or killing blow with skill id matching the effect
                    var deadClone = pClonesDead.LastOrDefault(x =>
                        x!.Time >= shatter.Time &&
                        x!.Time - shatter.Time < 2 * ServerDelayConstant &&
                        (!cloneKillingBlowsDict.TryGetValue(x.Src, out var killingBlows) ||
                            killingBlows.Any(x => skillIDs.Contains(x.SkillID))
                        )
                    );
                    if (deadClone == null)
                    {
                        // Safety check
                        if (chronoShatters.Any(x =>
                                Math.Abs(x.Time - shatter.Time) < ServerDelayConstant &&
                                (x.Position.XY() - shatter.Position.XY()).LengthSquared() < 1e-6)
                            )
                        {
                            res.Add(new InstantCastEvent(shatter.Time, skill, shatter.Src));
                        }
                    }
                    else
                    {
                        // Consume clone
                        pClonesDead.Remove(deadClone);
                    }
                }
            }
        }
        return res;
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Mesmer;

        // Well of Eternity
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfEternity, out var wellsOfEternity))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfEternity, SkillModeCategory.Heal);
            foreach (EffectEvent effect in wellsOfEternity)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfEternity);
            }
        }
        // Well of Eternity - Pulses
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, [EffectGUIDs.ChronomancerWellOfEternityPulse, EffectGUIDs.ChronomancerWellOfEternityExplosion], out var wellsOfEternityPulses))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfEternity, SkillModeCategory.Heal);
            foreach (EffectEvent effect in wellsOfEternityPulses)
            {
                int effectTimeStart = (int)effect.Time;
                int effectTimeEnd = effectTimeStart + 1000;
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingGrowingEnd(effectTimeEnd).UsingSkillMode(skill));
            }
        }

        // Well of Action
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfAction, out var wellsOfAction))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfAction);
            foreach (EffectEvent effect in wellsOfAction)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectWellOfAction, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));

                // Well pulses - Hard coded because the effects don't have a Src
                int pulseTimeDelay = 0;
                for (int i = 0; i < 4; i++)
                {
                    int effectTimeStart = (int)effect.Time + pulseTimeDelay;
                    int effectTimeEnd = effectTimeStart + 300;
                    if (effectTimeStart > lifespan.Item2) { break; }
                    var circle = (CircleDecoration)new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill);
                    if (i < 3)
                    {
                        // Pulse inwards
                        replay.Decorations.Add(circle.UsingGrowingEnd(effectTimeEnd, true));
                    }
                    else
                    {
                        // Final pulse outwards
                        replay.Decorations.Add(circle.UsingGrowingEnd(effectTimeEnd));
                    }
                    pulseTimeDelay += 1000;
                }
            }
        }

        // Well of Calamity
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfCalamity, out var wellsOfCalamity))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfCalamity);
            foreach (EffectEvent effect in wellsOfCalamity)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfCalamity);
            }
        }
        // Well of Calamity - Pulses
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, [EffectGUIDs.ChronomancerWellOfCalamityPulse, EffectGUIDs.ChronomancerWellOfCalamityExplosion], out var wellsOfCalamityPulses))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfCalamity);
            foreach (EffectEvent effect in wellsOfCalamityPulses)
            {
                int effectTimeStart = (int)effect.Time;
                int effectTimeEnd = effectTimeStart + 1000;
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingGrowingEnd(effectTimeEnd, true).UsingSkillMode(skill));
            }
        }

        // Well of Precognition
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfPrecognition, out var wellsOfPrecognition))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfPrecognition, SkillModeCategory.ImportantBuffs);
            foreach (EffectEvent effect in wellsOfPrecognition)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfPrecognition);
            }
        }
        // Well of Precognition - Pulses
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, [EffectGUIDs.ChronomancerWellOfPrecognitionPulse, EffectGUIDs.ChronomancerWellOfPrecognitionExplosion], out var wellsOfPrecognitionPulses))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfPrecognition, SkillModeCategory.ImportantBuffs);
            foreach (EffectEvent effect in wellsOfPrecognitionPulses)
            {
                int effectTimeStart = (int)effect.Time;
                int effectTimeEnd = effectTimeStart + 1000;
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingGrowingEnd(effectTimeEnd).UsingSkillMode(skill));
            }
        }

        // Well of Senility
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfSenility, out var wellsOfSenility))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfRecall_Senility);
            foreach (EffectEvent effect in wellsOfSenility)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectWellOfSenility, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));

                // Well pulses - Hard coded because the effects don't have a Src
                int pulseTimeDelay = 0;
                for (int i = 0; i < 4; i++)
                {
                    int effectTimeStart = (int)effect.Time + pulseTimeDelay;
                    int effectTimeEnd = effectTimeStart + 300;
                    if (effectTimeStart > lifespan.Item2) { break; }
                    var circle = (CircleDecoration)new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill);
                    if (i < 3)
                    {
                        // Pulse inwards
                        replay.Decorations.Add(circle.UsingGrowingEnd(effectTimeEnd, true));
                    }
                    else
                    {
                        // Final pulse outwards
                        replay.Decorations.Add(circle.UsingGrowingEnd(effectTimeEnd));
                    }
                    pulseTimeDelay += 1000;
                }
            }
        }

        // Gravity Well
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerGravityWell, out var gravityWells))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, GravityWell, SkillModeCategory.CC);
            foreach (EffectEvent effect in gravityWells)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectGravityWell);
            }
        }
        // Gravity Well - Pulses
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, [EffectGUIDs.ChronomancerGravityWellPulse, EffectGUIDs.ChronomancerGravityWellExplosion], out var gravityWellPulses))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, GravityWell, SkillModeCategory.CC);
            foreach (EffectEvent effect in gravityWellPulses)
            {
                int effectTimeStart = (int)effect.Time;
                int effectTimeEnd = effectTimeStart + 1000;
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingGrowingEnd(effectTimeEnd, true).UsingSkillMode(skill));
            }
        }
    }
}
