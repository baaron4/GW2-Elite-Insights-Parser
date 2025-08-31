using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Kanaxai : SilentSurf
{
    public Kanaxai(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup(
        [
            new MechanicGroup(
                [
                    new PlayerDstHealthDamageHitMechanic(RendingStormSkill, new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Red), "RendStm.H", "Hit by Rending Storm (Axe AoE)", "Rending Storm Hit", 0),
                    new PlayerDstBuffApplyMechanic([RendingStormAxeTargetBuff1, RendingStormAxeTargetBuff2], new MechanicPlotlySetting(Symbols.CircleX, Colors.LightPurple), "RendStm.T", "Targeted by Rending Storm (Axe Throw)", "Rending Storm Target", 150),
                ]
            ),
            new PlayerDstHealthDamageHitMechanic([HarrowshotDeath, HarrowshotExposure, HarrowshotFear, HarrowshotLethargy, HarrowshotTorment], new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "Harrowshot.H", "Harrowshot (Lost all boons)", "Harrowshot (Boonstrip)", 0),
            new MechanicGroup(
                [
                    new PlayerDstBuffApplyMechanic(ExtremeVulnerability, new MechanicPlotlySetting(Symbols.X, Colors.DarkRed), "ExtVuln.A", "Applied Extreme Vulnerability", "Extreme Vulnerability Application", 150),
                    new PlayerDstBuffRemoveMechanic(ExtremeVulnerability, new MechanicPlotlySetting(Symbols.Bowtie, Colors.DarkRed), "Eye.D", "Died to Dread Visage (Eye)", "Dread Visage Death", 150)
                        .UsingChecker((remove, log) =>
                        {
                            // 5s extreme vulnerability from dread visage
                            const int duration = 5000;
                            // find last apply
                            BuffApplyEvent apply = log.CombatData.GetBuffApplyDataByIDByDst(ExtremeVulnerability, remove.To)
                                .OfType<BuffApplyEvent>()
                                .Where(e => e.Time <= remove.Time)
                                .MaxBy(e => e.Time);
                            // check for removed duration, applied duration & death within 1s after
                            return remove.RemovedDuration > ServerDelayConstant
                                && Math.Abs(apply.AppliedDuration - duration) < ServerDelayConstant
                                && log.CombatData.GetDeadEvents(remove.To).Any(dead =>
                                {
                                    long diff = dead.Time - remove.Time;
                                    return diff > -ServerDelayConstant && diff <= 1000;
                                });
                        }
                    ),
                    new PlayerDstBuffRemoveMechanic(ExtremeVulnerability, new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Numbers.D", "Died to Frightening Speed (Numbers)", "Frightening Speed Death", 150)
                        .UsingChecker((remove, log) =>
                        {
                            // 60s extreme vulnerability from frightening speed
                            const int duration = 60000;
                            // find last apply
                            BuffApplyEvent apply = log.CombatData.GetBuffApplyDataByIDByDst(ExtremeVulnerability, remove.To)
                                .OfType<BuffApplyEvent>()
                                .Where(e => e.Time <= remove.Time)
                                .MaxBy(e => e.Time);
                            // check for removed duration, applied duration & death within 1s after
                            return remove.RemovedDuration > ServerDelayConstant
                                && Math.Abs(apply.AppliedDuration - duration) < ServerDelayConstant
                                && log.CombatData.GetDeadEvents(remove.To).Any(dead =>
                                {
                                    long diff = dead.Time - remove.Time;
                                    return diff > -ServerDelayConstant && diff <= 1000;
                                });
                        }
                    ),
                ]
            ),
            new PlayerDstBuffApplyMechanic(ExposedPlayer, new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Pink), "Expo.A", "Applied Exposed", "Exposed Application (Player)", 0),
            new PlayerDstBuffApplyMechanic(Fear, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Yellow), "Fear.A", "Fear Applied", "Fear Application", 150),
            new PlayerDstBuffApplyMechanic(Phantasmagoria, new MechanicPlotlySetting(Symbols.Diamond, Colors.Pink), "Phant.A", "Phantasmagoria Applied (Aspect visible on Island)", "Phantasmagoria Application", 150),
            new EnemyDstBuffApplyMechanic(Exposed31589, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Pink), "Expo.A", "Applied Exposed to Kanaxai", "Exposed Application (Kanaxai)", 150),
        ]));
        Extension = "kanaxai";
        Icon = EncounterIconKanaxai;
        LogID |= 0x000001;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                       (334, 370),
                       (-6195, -295, -799, 5685));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayKanaxai, crMap);
        return crMap;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.KanaxaiScytheOfHouseAurkusCM,
            TargetID.AspectOfTorment,
            TargetID.AspectOfLethargy,
            TargetID.AspectOfExposure,
            TargetID.AspectOfDeath,
            TargetID.AspectOfFear,
        ];
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.KanaxaiScytheOfHouseAurkusCM, 0 },
            {TargetID.AspectOfTorment, 1 },
            {TargetID.AspectOfLethargy, 1 },
            {TargetID.AspectOfExposure, 1 },
            {TargetID.AspectOfDeath, 1 },
            {TargetID.AspectOfFear, 1 },
        };
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.LogMode.CMNoName;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        // kanaxai spawns with invulnerability
        var kanaxai = agentData.GetNPCsByID(TargetID.KanaxaiScytheOfHouseAurkusCM).FirstOrDefault() ?? throw new MissingKeyActorsException("Kanaxai not found");
        return GetLogOffsetByInvulnStart(logData, combatData, kanaxai, Determined762);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KanaxaiScytheOfHouseAurkusCM)) ?? throw new MissingKeyActorsException("Kanaxai not found");
        phases[0].AddTarget(kanaxai, log);
        if (!requirePhases)
        {
            return phases;
        }
        // Phases
        List<PhaseData> encounterPhases = GetPhasesByInvul(log, DeterminedToDestroy, kanaxai, true, true);

        var worldCleaverPhaseStarts = log.CombatData.GetBuffApplyDataByIDByDst(DeterminedToDestroy, kanaxai.AgentItem).OfType<BuffApplyEvent
            >().Select(x => x.Time);
        int worldCleaverCount = 0;
        int repeatedCount = 0;
        var isRepeatedWorldCleaverPhase = new List<bool>();
        for (int i = 0; i < encounterPhases.Count; i++)
        {
            PhaseData curPhase = encounterPhases[i];
            curPhase.AddParentPhase(phases[0]);
            if (worldCleaverPhaseStarts.Any(x => curPhase.Start == x))
            {
                var baseName = "World Cleaver ";
                long midPhase = (curPhase.Start + curPhase.End) / 2;
                if (kanaxai.GetCurrentHealthPercent(log, midPhase) > 50)
                {
                    if (repeatedCount == 0)
                    {
                        isRepeatedWorldCleaverPhase.Add(false);
                        curPhase.Name = baseName + (++worldCleaverCount);
                    }
                    else
                    {
                        isRepeatedWorldCleaverPhase.Add(true);
                        curPhase.Name = baseName + (worldCleaverCount) + " Repeated " + repeatedCount;
                    }
                    repeatedCount++;
                }
                else if (kanaxai.GetCurrentHealthPercent(log, midPhase) > 25)
                {
                    if (worldCleaverCount == 1)
                    {
                        repeatedCount = 0;
                    }
                    if (repeatedCount == 0)
                    {
                        isRepeatedWorldCleaverPhase.Add(false);
                        curPhase.Name = baseName + (++worldCleaverCount);
                    }
                    else
                    {
                        isRepeatedWorldCleaverPhase.Add(true);
                        curPhase.Name = baseName + (worldCleaverCount) + " Repeated " + repeatedCount;
                    }
                    repeatedCount++;
                }
                else
                {
                    // No hp update events, buggy log
                    return phases;
                }
                foreach (SingleActor aspect in Targets)
                {
                    switch (aspect.ID)
                    {
                        case (int)TargetID.AspectOfTorment:
                        case (int)TargetID.AspectOfLethargy:
                        case (int)TargetID.AspectOfExposure:
                        case (int)TargetID.AspectOfDeath:
                        case (int)TargetID.AspectOfFear:
                            if (log.CombatData.GetBuffRemoveAllDataByDst(Determined762, aspect.AgentItem).Any(x => x.Time >= curPhase.Start && x.Time <= curPhase.End))
                            {
                                curPhase.AddTarget(aspect, log);
                            }
                            break;
                    }
                }
                curPhase.AddTarget(kanaxai, log);
            }
            else
            {
                isRepeatedWorldCleaverPhase.Add(false);
            }
        }
        // Handle main phases after world cleave phases as we need to know if it is a repeated phase
        int phaseCount = 0;
        for (int i = 0; i < encounterPhases.Count; i++)
        {
            PhaseData curPhase = encounterPhases[i];
            if (!worldCleaverPhaseStarts.Any(x => curPhase.Start == x))
            {
                var baseName = "Phase ";
                if (i < isRepeatedWorldCleaverPhase.Count - 1)
                {
                    if (isRepeatedWorldCleaverPhase[i + 1])
                    {
                        curPhase.Name = baseName + (phaseCount) + " Repeated " + (++repeatedCount);
                    }
                    else
                    {
                        curPhase.Name = baseName + (++phaseCount);
                        repeatedCount = 0;
                    }
                }
                else
                {
                    curPhase.Name = baseName + (++phaseCount);
                }
                curPhase.AddTarget(kanaxai, log);
            }
        }
        phases.AddRange(encounterPhases);

        return phases;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        SingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KanaxaiScytheOfHouseAurkusCM)) ?? throw new MissingKeyActorsException("Kanaxai not found");
        BuffApplyEvent? invul762Gain = combatData.GetBuffApplyDataByIDByDst(Determined762, kanaxai.AgentItem).OfType<BuffApplyEvent>().FirstOrDefault(x => x.Time > 0);
        if (invul762Gain != null && !combatData.GetDespawnEvents(kanaxai.AgentItem).Any(x => Math.Abs(x.Time - invul762Gain.Time) < ServerDelayConstant))
        {
            logData.SetSuccess(true, invul762Gain.Time);
        }
        else
        {
            logData.SetSuccess(false, kanaxai.LastAware);
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);
        long maxEnd = log.LogData.LogEnd;

        // Orange Tether from Aspect to player
        IEnumerable<BuffEvent> tethers = log.CombatData.GetBuffDataByIDByDst(AspectTetherBuff, player.AgentItem);
        IEnumerable<BuffApplyEvent> tetherApplies = tethers.OfType<BuffApplyEvent>();
        IEnumerable<BuffRemoveAllEvent> tetherRemoves = tethers.OfType<BuffRemoveAllEvent>();
        AgentItem tetherAspect = _unknownAgent;
        foreach (BuffApplyEvent apply in tetherApplies)
        {
            tetherAspect = apply.By.IsUnknown ? tetherAspect : apply.By;
            int start = (int)apply.Time;
            BuffApplyEvent? replace = tetherApplies.FirstOrDefault(x => x.Time >= apply.Time && !x.By.Is(tetherAspect));
            BuffRemoveAllEvent? remove = tetherRemoves.FirstOrDefault(x => x.Time >= apply.Time);
            long end = Math.Min(replace?.Time ?? maxEnd, remove?.Time ?? maxEnd);
            replay.Decorations.Add(new LineDecoration((start, (int)end), Colors.Yellow, 0.5, new AgentConnector(tetherAspect), new AgentConnector(player)));
        }

        // Blue tether from Aspect to player, appears when the player gains Phantasmagoria
        // Custom decoration not visible in game
        var phantasmagorias = GetBuffApplyRemoveSequence(log.CombatData, Phantasmagoria, player, true, true);
        replay.Decorations.AddTether(phantasmagorias, Colors.LightBlue, 0.5);

        // Rending Storm - Axe AoE attached to players - There are 2 buffs for the targetting
        IEnumerable<Segment> axes = player.GetBuffStatus(log, [RendingStormAxeTargetBuff1, RendingStormAxeTargetBuff2]).Where(x => x.Value > 0);
        foreach (Segment segment in axes)
        {
            replay.Decorations.AddWithGrowing(new CircleDecoration(180, segment, Colors.Orange, 0.2, new AgentConnector(player)), segment.End);
        }

        // Frightening Speed - Numbers spread AoEs
        IEnumerable<Segment> spreads = player.GetBuffStatus(log, KanaxaiSpreadOrangeAoEBuff).Where(x => x.Value > 0);
        foreach (Segment spreadSegment in spreads)
        {
            replay.Decorations.Add(new CircleDecoration(380, spreadSegment, Colors.Orange, 0.2, new AgentConnector(player)));
        }

        // Target Order Overhead
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder1).Where(x => x.Value > 0), player, ParserIcons.TargetOrder1Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder2).Where(x => x.Value > 0), player, ParserIcons.TargetOrder2Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder3).Where(x => x.Value > 0), player, ParserIcons.TargetOrder3Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder4).Where(x => x.Value > 0), player, ParserIcons.TargetOrder4Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder5).Where(x => x.Value > 0), player, ParserIcons.TargetOrder5Overhead);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        long growing;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.KanaxaiScytheOfHouseAurkusCM:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // World Cleaver - 66 & 33% Attack
                        case WorldCleaver:
                            castDuration = 26320;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            var hits = log.CombatData.GetDamageData(WorldCleaver).Where(x => x.Time > cast.Time);
                            var firstHit = hits.FirstOrDefault(x => x.Time > cast.Time);
                            if (firstHit != null)
                            {
                                (long start, long end) lifespanHit = (cast.Time, firstHit.Time);
                                AddWorldCleaverDecoration(target, replay, lifespanHit, lifespan.end);
                            }
                            else
                            {
                                AddWorldCleaverDecoration(target, replay, lifespan, lifespan.end);
                            }
                            break;
                        // Dread Visage - Eye
                        case DreadVisageKanaxaiSkill:
                        case DreadVisageKanaxaiSkillIsland:
                            castDuration = 5400;
                            growing = (int)cast.Time + castDuration;
                            double actualDuration = ComputeCastTimeWithQuickness(log, target, cast.Time, castDuration);
                            if (actualDuration > 0)
                            {
                                replay.Decorations.AddOverheadIcon(new Segment(cast.Time, cast.Time + (long)Math.Ceiling(actualDuration), 1), target, ParserIcons.EyeOverhead, 30);
                            }
                            else
                            {
                                replay.Decorations.AddOverheadIcon(new Segment(cast.Time, growing, 1), target, ParserIcons.EyeOverhead, 30);
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.AspectOfTorment:
            case (int)TargetID.AspectOfLethargy:
            case (int)TargetID.AspectOfExposure:
            case (int)TargetID.AspectOfDeath:
            case (int)TargetID.AspectOfFear:
                // Check if the log contains Sugar Rush
                bool hasSugarRush = log.CombatData.GetBuffData(MistlockInstabilitySugarRush).Any(x => x.To.IsPlayer);

                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Tether casts performed by Aspects
                        case AspectTetherSkill:
                            lifespan = (cast.Time, cast.ExpectedEndTime); // actual end is often much later, just use expected end for short highlight
                            replay.Decorations.Add(new CircleDecoration(180, 20, lifespan, Colors.LightBlue, 0.5, new AgentConnector(target)).UsingFilled(false));
                            break;
                        // Dread Visage - Eye
                        case DreadVisageAspectSkill:
                            castDuration = 5400;
                            growing = (int)cast.Time + castDuration;

                            Segment? quickness = target.GetBuffStatus(log, Quickness, cast.Time, growing).Where(x => x.Value == 1).FirstOrNull();

                            // If the aspect has Sugar Rush AND Quickness
                            if (hasSugarRush && quickness != null)
                            {
                                double actualDuration = ComputeCastTimeWithQuicknessAndSugarRush(log, target, cast.Time, castDuration);
                                var duration = new Segment(cast.Time, cast.Time + (int)Math.Ceiling(actualDuration), 1);
                                replay.Decorations.AddOverheadIcon(duration, target, ParserIcons.EyeOverhead, 30);
                            }

                            // If the aspect has Sugar rush AND NOT Quickness
                            if (hasSugarRush && quickness == null)
                            {
                                var actualDuration = ComputeCastTimeWithSugarRush(castDuration);
                                var duration = new Segment(cast.Time, cast.Time + (int)Math.Ceiling(actualDuration), 1);
                                replay.Decorations.AddOverheadIcon(duration, target, ParserIcons.EyeOverhead, 30);
                            }

                            // If the aspect DOESN'T have Sugar rush but HAS Quickness
                            if (!hasSugarRush && quickness != null)
                            {
                                double actualDuration = ComputeCastTimeWithQuickness(log, target, cast.Time, castDuration);
                                var duration = new Segment(cast.Time, cast.Time + (int)Math.Ceiling(actualDuration), 1);
                                replay.Decorations.AddOverheadIcon(duration, target, ParserIcons.EyeOverhead, 30);
                            }

                            // If the aspect DOESN'T have Sugar Rush and Quickness
                            if (!hasSugarRush && quickness == null)
                            {
                                replay.Decorations.AddOverheadIcon(new Segment((int)cast.Time, growing, 1), target, ParserIcons.EyeOverhead, 30);
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        // Frightening Speed - Red AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.FrighteningSpeedRedAoE, out var frighteningSpeedRedAoEs))
        {
            foreach (EffectEvent aoe in frighteningSpeedRedAoEs)
            {
                (long start, long end) lifespan = (aoe.Time, aoe.Time + 1500);
                var circle = new CircleDecoration(380, lifespan, Colors.Red, 0.2, new PositionConnector(aoe.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.Copy().UsingFilled(false));
            }
        }

        // Rending Storm - Red Axe AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AxeGroundAoE, out var axeAoEs))
        {
            // Get World Cleaver casts
            SingleActor? kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KanaxaiScytheOfHouseAurkusCM));
            if (kanaxai != null)
            {
                var casts = kanaxai.GetCastEvents(log);

                // Get Axe AoE Buffs
                //TODO(Rennorb) @perf: find average complexity
                var axes = new List<BuffEvent>(50);
                axes.AddRange(log.CombatData.GetBuffData(RendingStormAxeTargetBuff1).OfType<BuffRemoveAllEvent>());
                axes.AddRange(log.CombatData.GetBuffData(RendingStormAxeTargetBuff2).OfType<BuffRemoveAllEvent>());
                axes.SortByTime();

                foreach (EffectEvent aoe in axeAoEs)
                {
                    // Find the first cast time event present after the AoE effect time
                    var cast = casts.Where(x => x.SkillID == WorldCleaver).FirstOrDefault(x => x.Time > aoe.Time);
                    long worldCleaverTime = cast?.Time ?? 0;

                    // Find the first BuffRemoveAllEvent after the AoE effect Time or next World Cleaver cast time
                    // World Cleaver is the time-limit of when the AoEs reset, in third phase we use LogEnd
                    if (worldCleaverTime != 0)
                    {
                        var axeBuffRemoval = axes.FirstOrDefault(buff => buff.Time > aoe.Time && buff.Time < worldCleaverTime);
                        AddAxeAoeDecoration(aoe, axeBuffRemoval, worldCleaverTime, environmentDecorations);
                    }
                    else
                    {
                        var axeBuffRemoval = axes.FirstOrDefault(buff => buff.Time > aoe.Time);
                        AddAxeAoeDecoration(aoe, axeBuffRemoval, log.LogData.LogEnd, environmentDecorations);
                    }
                }
            }
        }

        // Harrowshot - Boonstrip AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarrowshotAoE, out var harrowshots))
        {
            foreach (EffectEvent harrowshot in harrowshots)
            {
                (long start, long end) lifespan = harrowshot.ComputeLifespan(log, 3000);
                var circle = new CircleDecoration(280, lifespan, Colors.Orange, 0.2, new PositionConnector(harrowshot.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.end));
            }
        }
    }

    /// <summary>
    /// Adds the Axe AoE decoration.<br></br>
    /// If the next orange AoE <see cref="BuffRemoveAllEvent"/> on players is after <see cref="WorldCleaver"/> cast time or not present,<br></br>
    /// utilise the <see cref="WorldCleaver"/> cast time or <see cref="LogData.EvtcLogEnd"/>.
    /// </summary>
    /// <param name="aoe">Effect of the AoE.</param>
    /// <param name="axeBuffRemoval">Buff removal of the orange AoE.</param>
    /// <param name="time">Last time possible.</param>
    private static void AddAxeAoeDecoration(EffectEvent aoe, BuffEvent? axeBuffRemoval, long time, CombatReplayDecorationContainer environmentDecorations)
    {
        int duration;
        if (axeBuffRemoval != null)
        {
            duration = (int)(axeBuffRemoval.Time - aoe.Time);
        }
        else
        {
            duration = (int)(time - aoe.Time);
        }
        int start = (int)aoe.Time;
        int effectEnd = start + duration;
        var circle = new CircleDecoration(180, (start, effectEnd), Colors.Red, 0.2, new PositionConnector(aoe.Position));
        environmentDecorations.Add(circle);
        environmentDecorations.Add(circle.Copy().UsingFilled(false));
    }

    /// <summary>
    /// Adds the World Cleaver decoration.
    /// </summary>
    /// <param name="target">Kanaxai.</param>
    /// <param name="replay">Combat Replay.</param>
    /// <param name="lifespan">Start and End of the cast.</param>
    /// <param name="growing">Duration of the channel.</param>
    private static void AddWorldCleaverDecoration(NPC target, CombatReplay replay, (long start, long end) lifespan, long growing)
    {
        replay.Decorations.AddWithGrowing(new CircleDecoration(1100, lifespan, Colors.Red, 0.2, new AgentConnector(target)), growing);
    }
}
