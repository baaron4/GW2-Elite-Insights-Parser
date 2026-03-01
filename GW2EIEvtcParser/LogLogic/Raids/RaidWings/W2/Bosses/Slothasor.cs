using System;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Slothasor : SalvationPass
{
    internal readonly MechanicGroup Mechanics = new([
            new PlayerDstHealthDamageHitMechanic(TantrumDamage, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Tantrum", "Tantrum (Triple Circles after Ground slamming)","Tantrum", 5000),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(VolatilePoisonBuff, new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Poison", "Volatile Poison Application (Special Action Key)","Poison (Action Key)", 0),
                new PlayerDstHealthDamageHitMechanic(VolatilePoisonSkill, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Poison dmg", "Stood in Volatile Poison","Poison dmg", 0),
            ]),
            new PlayerDstHealthDamageHitMechanic(Halitosis, new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.LightOrange), "Breath", "Halitosis (Flame Breath)","Flame Breath", 0),
            new PlayerDstHealthDamageHitMechanic(SporeRelease, new MechanicPlotlySetting(Symbols.Pentagon,Colors.Red), "Shake", "Spore Release (Coconut Shake)","Shake", 0),
            new PlayerDstBuffApplyMechanic(MagicTransformation, new MechanicPlotlySetting(Symbols.Hexagram,Colors.Teal), "Slub", "Magic Transformation (Ate Magic Mushroom)","Slub Transform", 0)
                    .UsingTimeClamper((time, log, encounterPhase) => Math.Max(encounterPhase.Start, time)), 
            //new Mechanic(Nauseated, "Nauseated", ParseEnum.BossIDS.Slothasor, new MechanicPlotlySetting("diamond-tall-open",Colors.LightPurple), "Slub CD",0), //can be skipped imho, identical person and timestamp as Slub Transform
            new PlayerDstBuffApplyMechanic(FixatedSlothasor, new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixate", "Fixated by Slothasor","Fixated", 0),
            new PlayerDstHealthDamageHitMechanic([ToxicCloud1, ToxicCloud2], new MechanicPlotlySetting(Symbols.PentagonOpen,Colors.DarkGreen), "Floor", "Toxic Cloud (stood in green floor poison)","Toxic Floor", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(Fear, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Fear", "Hit by fear after breakbar","Feared", 0)
                    .UsingChecker((ba,log) => ba.AppliedDuration == 8000),
                new EnemyDstBuffApplyMechanic(NarcolepsyBuff, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC.Slth", "Narcolepsy (Breakbar)","Breakbar", 0),
                new EnemyDstBuffRemoveMechanic(NarcolepsyBuff, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC.Slth Fail", "Narcolepsy (Failed CC)","CC Fail", 0)
                    .UsingChecker((br,log) => br.RemovedDuration > 120000),
                new EnemyDstBuffRemoveMechanic(NarcolepsyBuff, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed.Slth", "Narcolepsy (Breakbar broken)","CCed", 0)
                    .UsingChecker( (br,log) => br.RemovedDuration <= 120000),
            ]),
            new PlayerDstBuffApplyMechanic(SlipperySlubling, new MechanicPlotlySetting(Symbols.Star,Colors.Yellow), "Slppr.Slb", "Slippery Slubling","Slippery Slubling", 0),
        ]);
    public Slothasor(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "sloth";
        Icon = EncounterIconSlothasor;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000001;
        ChestID = ChestID.SlothasorChest;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (654, 1000),
                        (5822, -3491, 9549, 2205));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplaySlothasor, crMap);
        return crMap;
    }

    internal override void UpdatePlayersSpecAndGroup(IReadOnlyList<Player> players, CombatData combatData, LogData logData)
    {
        base.UpdatePlayersSpecAndGroup(players, combatData, logData);
        var slubTransformApplyAtStart = combatData.GetBuffApplyData(MagicTransformation).FirstOrDefault(x => x.Time <= logData.LogStart + 5000);
        if (slubTransformApplyAtStart != null)
        {
            var transformedPlayer = players.FirstOrDefault(x => x.AgentItem.Is(slubTransformApplyAtStart.To));
            if (transformedPlayer != null)
            {
                var transfoExit = combatData.GetBuffRemoveAllDataByIDBySrc(MagicTransformation, transformedPlayer.AgentItem).FirstOrDefault(x => x.Time >= slubTransformApplyAtStart.Time);
                if (transfoExit != null)
                {
                    var enterCombat = combatData.GetEnterCombatEvents(transformedPlayer.AgentItem).Where(x => x.Time <= transfoExit.Time + 1000).LastOrDefault(x => x.Spec != Spec.Unknown && x.Subgroup != 0);
                    if (enterCombat != null)
                    {
                        transformedPlayer.AgentItem.OverrideSpec(enterCombat.Spec);
                        transformedPlayer.OverrideGroup(enterCombat.Subgroup);
                    }
                }
            }
        }
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
        if (log.CombatData.GetBuffData(SlipperySlubling).Any())
        {
            var encounterPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID);
            foreach (var encounterPhase in encounterPhases)
            {
                if (encounterPhase.Success)
                {
                    instanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, encounterPhase, SlipperySlubling));
                }
            }
        }
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.Slubling1,
            TargetID.Slubling2,
            TargetID.Slubling3,
            TargetID.Slubling4,
            TargetID.PoisonMushroom
        ];
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.Slothasor,
            TargetID.PlayerSlubling
        ];
    }
    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [(int)TargetID.PlayerSlubling];
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(VolatileAura, VolatileAura),
            new BuffLossCastFinder(PurgeSlothasor, VolatilePoisonBuff),
        ];
    }

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor slothasor, IEnumerable<SingleActor> slublings, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<SubPhasePhaseData>(5);
        long encounterStart = encounterPhase.Start;
        long encounterEnd = encounterPhase.End;
        var sleepy = slothasor.GetAnimatedCastEvents(log, encounterStart, encounterEnd).Where(x => x.SkillID == NarcolepsySkill);
        long start = encounterStart;
        int i = 1;
        foreach (CastEvent c in sleepy)
        {
            var phase = new SubPhasePhaseData(start, Math.Min(c.Time, encounterEnd), "Phase " + i++);
            phase.AddParentPhase(encounterPhase);
            phase.AddTarget(slothasor, log);
            phase.AddTargets(slublings, log, PhaseData.TargetPriority.NonBlocking);
            start = c.EndTime;
            phases.Add(phase);
        }
        var lastPhase = new SubPhasePhaseData(start, encounterEnd, "Phase " + i++);
        lastPhase.AddParentPhase(encounterPhase);
        lastPhase.AddTarget(slothasor, log);
        lastPhase.AddTargets(slublings, log, PhaseData.TargetPriority.NonBlocking);
        phases.Add(lastPhase);
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Slothasor)) ?? throw new MissingKeyActorsException("Slothasor not found");
        var slublings = Targets.Where(x => x.IsSpecies(TargetID.PlayerSlubling));
        phases[0].AddTarget(mainTarget, log);
        phases[0].AddTargets(slublings, log, PhaseData.TargetPriority.NonBlocking);
        phases.AddRange(ComputePhases(log, mainTarget, slublings, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal static void FindMushrooms(LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var mushroomAgents = combatData
            .Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate)
            .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => x.Type == AgentItem.AgentType.Gadget && (x.HitboxWidth == 146 || x.HitboxWidth == 210) && x.HitboxHeight == 300)
            .ToList();
        if (mushroomAgents.Count > 0)
        {
            int idToKeep = mushroomAgents.GroupBy(x => x.ID).Select(x => (x.Key, x.Count())).MaxBy(x => x.Item2).Key;
            foreach (AgentItem mushroom in mushroomAgents)
            {
                if (!mushroom.IsSpecies(idToKeep))
                {
                    continue;
                }
                var hpUpdates = combatData.Where(x => x.SrcMatchesAgent(mushroom) && x.IsStateChange == StateChange.HealthUpdate);
                var aliveUpdates = hpUpdates.Where(x => HealthUpdateEvent.GetHealthPercent(x) == 100).ToList();
                var deadUpdates = hpUpdates.Where(x => HealthUpdateEvent.GetHealthPercent(x) == 0).ToList();
                long lastDeadTime = long.MinValue;
                foreach (CombatItem aliveEvent in aliveUpdates)
                {
                    CombatItem? deadEvent = deadUpdates.FirstOrDefault(x => x.Time > lastDeadTime && x.Time > aliveEvent.Time);
                    if (deadEvent == null)
                    {
                        lastDeadTime = Math.Min(logData.EvtcLogEnd, mushroom.LastAware);
                    }
                    else
                    {
                        lastDeadTime = deadEvent.Time;
                    }
                    AgentItem aliveMushroom = agentData.AddCustomNPCAgent(aliveEvent.Time, lastDeadTime, mushroom.Name, mushroom.Spec, TargetID.PoisonMushroom, false, mushroom.Toughness, mushroom.Healing, mushroom.Condition, mushroom.Concentration, mushroom.HitboxWidth, mushroom.HitboxHeight);
                    aliveMushroom.SetEnglobingAgentItem(mushroom, agentData);
                }
            }
        }
    }

    internal static void FindSlublingTransformations(LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var slubTransformList = combatData.Where(x => x.SkillID == MagicTransformation && !x.IsExtension && (x.IsBuffRemove == BuffRemove.All || x.IsBuffApply()));
        var transformStart = slubTransformList.Where(x => x.IsBuffRemove == BuffRemove.None).ToList();
        var transformEnd = slubTransformList.Where(x => x.IsBuffRemove == BuffRemove.All).ToList();
        var copies = new List<CombatItem>();
        for (int i = 0; i < transformStart.Count; i++)
        {
            //
            long transformStartTime = transformStart[i].Time;
            long transformEndTime = i < transformEnd.Count ? transformEnd[i].Time : logData.LogEnd;
            //
            AgentItem? transformedPlayer = agentData.GetAgentByType(AgentItem.AgentType.Player).FirstOrDefault(x => x.Is(agentData.GetAgent(transformStart[i].DstAgent, transformStart[i].Time)));
            if (transformedPlayer == null)
            {
                continue;
            }
            transformedPlayer = transformedPlayer.EnglobingAgentItem;
            AgentItem slubling = agentData.AddCustomNPCAgent(transformStartTime, transformEndTime + 100, "Slubling " + (i + 1) + " " + transformedPlayer.Name.Split('\0')[0], Spec.NPC, TargetID.PlayerSlubling, false, 0, 0, 0, 0, transformedPlayer.HitboxWidth, transformedPlayer.HitboxHeight);
            AgentManipulationHelper.RedirectDamageAndCopyRemainingFromSrcToDst(slubling, transformedPlayer, copies, combatData, extensions);
        }
        if (copies.Count != 0)
        {
            combatData.AddRange(copies);
            combatData.SortByTime();
        }
    }


    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindMushrooms(logData, agentData, combatData, extensions);
        FindSlublingTransformations(logData, agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        long castDuration;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Slothasor:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Halitosis - Fire breath
                        case Halitosis:
                            long preCastTime = 1000;
                            castDuration = 2000;
                            (long start, long end) lifespanPrecast = (cast.Time, cast.Time + preCastTime);
                            lifespan = (lifespanPrecast.end, lifespanPrecast.end + castDuration);
                            uint range = 600;
                            if (target.TryGetCurrentFacingDirection(log, lifespanPrecast.start, out var facingHalitosis))
                            {
                                var connector = new AgentConnector(target);
                                var rotationConnector = new AngleConnector(facingHalitosis);
                                var openingAngle = 60;
                                replay.Decorations.Add(new PieDecoration(range, openingAngle, lifespanPrecast, Colors.Orange, 0.1, connector).UsingRotationConnector(rotationConnector));
                                replay.Decorations.Add(new PieDecoration(range, openingAngle, lifespan, Colors.Orange, 0.4, connector).UsingRotationConnector(rotationConnector));
                            }
                            break;
                        // Tantrum - 3 sets ground AoEs
                        case TantrumSkill:
                            // Generic indicator of casting
                            if (!log.CombatData.HasEffectData)
                            {
                                lifespan = (cast.Time, cast.EndTime);
                                var tantrum = new CircleDecoration(300, lifespan, Colors.LightOrange, 0.4, new AgentConnector(target));
                                replay.Decorations.AddWithFilledWithGrowing(tantrum.UsingFilled(false), true, lifespan.end);
                            }
                            break;
                        // Spore Release - Shake
                        case SporeRelease:
                            // Generic indicator of casting
                            if (!log.CombatData.HasEffectData)
                            {
                                lifespan = (cast.Time, cast.EndTime);
                                var sporeRelease = new CircleDecoration(700, lifespan, Colors.Red, 0.4, new AgentConnector(target)).UsingFilled(false);
                                replay.Decorations.AddWithFilledWithGrowing(sporeRelease, true, lifespan.end);
                            }
                            break;
                        default:
                            break;
                    }
                }

                // Narcolepsy - Invulnerability
                var narcolepsies = target.GetBuffStatus(log, NarcolepsyBuff).Where(x => x.Value > 0);
                foreach (var narcolepsy in narcolepsies)
                {
                    replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, (narcolepsy.Start, narcolepsy.End), Colors.LightBlue, 0.6, Colors.Black, 0.2, [(narcolepsy.Start, 0), (narcolepsy.Start + 120000, 100)], new AgentConnector(target))
                        .UsingRotationConnector(new AngleConnector(180)));
                }

                // Tantrum - Knockdown AoEs
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.SlothasorTantrum, out var tantrums))
                {
                    foreach (EffectEvent effect in tantrums)
                    {
                        lifespan = effect.ComputeLifespan(log, 2000);
                        var circle = new CircleDecoration(100, lifespan, Colors.LightOrange, 0.1, new PositionConnector(effect.Position));
                        replay.Decorations.Add(circle);
                    }
                }
                break;
            case (int)TargetID.PoisonMushroom:
                break;
            default:
                break;
        }

    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        // Poison
        var poisonToDrop = p.GetBuffStatus(log, VolatilePoisonBuff).Where(x => x.Value > 0);
        foreach (Segment seg in poisonToDrop)
        {
            int toDropStart = (int)seg.Start;
            int toDropEnd = (int)seg.End;
            var circle = new CircleDecoration(180, seg, "rgba(255, 255, 100, 0.5)", new AgentConnector(p));
            replay.Decorations.AddWithFilledWithGrowing(circle.UsingFilled(false), true, toDropStart + 8000);
            if (!log.CombatData.HasEffectData && p.TryGetCurrentInterpolatedPosition(log, toDropEnd, out var position))
            {
                replay.Decorations.Add(new CircleDecoration(900, 180, (toDropEnd, toDropStart + 90000), Colors.GreenishYellow, 0.3, new PositionConnector(position)).UsingGrowingEnd(toDropEnd + 82000));
            }
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.VolatilePoisonOverhead);
        }
        // Transformation
        var slubTrans = p.GetBuffStatus(log, MagicTransformation).Where(x => x.Value > 0);
        foreach (Segment seg in slubTrans)
        {
            replay.Decorations.Add(new CircleDecoration(180, seg, "rgba(0, 80, 255, 0.3)", new AgentConnector(p)));
        }
        // Fixated
        var fixatedSloth = p.GetBuffStatus(log, FixatedSlothasor).Where(x => x.Value > 0);
        foreach (Segment seg in fixatedSloth)
        {
            replay.Decorations.Add(new CircleDecoration(120, seg, Colors.FixationPurple.WithAlpha(0.3).ToString(), new AgentConnector(p)));
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SlothasorSporeReleaseProjectileImpacts, out var sporeReleaseImpacts))
        {
            foreach (var sporeReleaseImpact in sporeReleaseImpacts)
            {
                var lifespan = sporeReleaseImpact.ComputeLifespan(log, 1000);
                var circle = new CircleDecoration(100, lifespan, Colors.Red, 0.2, new PositionConnector(sporeReleaseImpact.Position));
                environmentDecorations.Add(circle);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SlothasorGrowingVolatilePoison, out var growingVolatilePoisons))
        {
            foreach (var growingVolatilePoison in growingVolatilePoisons)
            {
                var volatilePoisonApply = log.CombatData.GetBuffApplyData(VolatilePoisonBuff).LastOrDefault(x => x.Time <= growingVolatilePoison.Time);
                if (volatilePoisonApply != null)
                {
                    // Compute life span not reliable, has a dynamic end, which cuts the AoE short when encounter ends, use the expected durations
                    environmentDecorations.Add(new CircleDecoration(900, 180, (growingVolatilePoison.Time, volatilePoisonApply.Time + 90000), Colors.GreenishYellow, 0.3, new PositionConnector(growingVolatilePoison.Position)).UsingGrowingEnd(growingVolatilePoison.Time + 82000));
                }
                
            }       
        }
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }
}
