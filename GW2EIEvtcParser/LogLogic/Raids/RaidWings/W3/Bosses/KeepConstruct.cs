using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Trigonometry;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class KeepConstruct : StrongholdOfTheFaithful
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new PlayerDstBuffApplyMechanic([StatueFixated1, StatueFixated2], new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixate", "Fixated by Statue","Fixated", 0),
            new PlayerDstHealthDamageHitMechanic(HailOfFury, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Debris", "Hail of Fury (Falling Debris)","Debris", 0),
            new EnemyDstBuffApplyMechanic(Compromised, new MechanicPlotlySetting(Symbols.Hexagon,Colors.Blue), "Rift#", "Compromised (Pushed Orb through Rifts)","Compromised", 0),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(MagicBlast, new MechanicPlotlySetting(Symbols.Star,Colors.Teal), "M.B.# 33%", "Magic Blast (Orbs eaten by KC) at 33%","Magic Blast 33%", 0)
                    .UsingChecker( (de, log) => de.To.GetCurrentHealthPercent(log, de.Time) <= 40),
                new EnemyDstBuffApplyMechanic(MagicBlast, new MechanicPlotlySetting(Symbols.Star,Colors.DarkTeal), "M.B.# 66%", "Magic Blast (Orbs eaten by KC) at 66%","Magic Blast 66%", 0)
                    .UsingChecker( (de, log) => {
                            var curHP = de.To.GetCurrentHealthPercent(log, de.Time);
                            return curHP <= 70 &&  curHP >= 60;
                        }
                    ),
            ]),
            new SpawnMechanic((int) TargetID.InsidiousProjection, new MechanicPlotlySetting(Symbols.Bowtie,Colors.Red), "Merge", "Insidious Projection spawn (2 Statue merge)","Merged Statues", 0),
            new PlayerDstHealthDamageHitMechanic([PhantasmalBlades2,PhantasmalBlades3, PhantasmalBlades1], new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Magenta), "Pizza", "Phantasmal Blades (rotating Attack)","Phantasmal Blades", 0),
            new PlayerDstHealthDamageHitMechanic(TowerDrop, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Jump", "Tower Drop (KC Jump)","Tower Drop", 0),
            new PlayerDstBuffApplyMechanic(XerasFury, new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Bomb", "Xera's Fury (Large Bombs) application","Bombs", 0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(RadiantEnergyWhiteOrb, new MechanicPlotlySetting(Symbols.Circle,Colors.White), "GW.Orb", "Good White Orb","Good White Orb", 0)
                    .UsingChecker((de,log) => de.To.HasBuff(log, RadiantAttunementOrb, de.Time)),
                new PlayerDstHealthDamageHitMechanic(CrimsonEnergyRedOrb, new MechanicPlotlySetting(Symbols.Circle,Colors.DarkRed), "GR.Orb", "Good Red Orb","Good Red Orb", 0)
                    .UsingChecker((de,log) => de.To.HasBuff(log, CrimsonAttunementOrb, de.Time)),
                new PlayerDstHealthDamageHitMechanic(RadiantEnergyWhiteOrb, new MechanicPlotlySetting(Symbols.Circle,Colors.Grey), "BW.Orb", "Bad White Orb","Bad White Orb", 0)
                    .UsingChecker((de,log) => !de.To.HasBuff(log, RadiantAttunementOrb, de.Time)),
                new PlayerDstHealthDamageHitMechanic(CrimsonEnergyRedOrb, new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "BR.Orb", "Bad Red Orb","Bad Red Orb", 0)
                    .UsingChecker((de,log) => !de.To.HasBuff(log, CrimsonAttunementOrb, de.Time)),
            ]),
            new PlayerSrcAllHealthDamageHitsMechanic(new MechanicPlotlySetting(Symbols.StarOpen,Colors.LightOrange), "Core Hit","Core was Hit by Player", "Core Hit",1000)
                .UsingChecker((de, log) => de.To.IsSpecies(TargetID.KeepConstructCore) && de is DirectHealthDamageEvent)
        ]);
    public KeepConstruct(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "kc";
        Icon = EncounterIconKeepConstruct;
        LogCategoryInformation.InSubCategoryOrder = 1;
        LogID |= 0x000002;
        ChestID = ChestID.KeepConstructChest;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (987, 1000),
                        (-5467, 8069, -2282, 11297));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayKeepConstruct, crMap);
        return crMap;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(ConstructAura, ConstructAura),
        ];
    }

    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor keepConstruct, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        long encounterStart = encounterPhase.Start;
        long encounterEnd = encounterPhase.End;
        var phases = new List<PhaseData>(20);
        // Main phases 35025
        var kcPhaseInvuls = keepConstruct.GetBuffStatus(log, XerasBoon);
        var mainPhases = new List<PhaseData>();
        var mainPhaseCount = 1;
        foreach (var c in kcPhaseInvuls)
        {
            if (c.Value == 0)
            {
                var mainPhase = new SubPhasePhaseData(Math.Max(c.Start, encounterStart), Math.Min(c.End, encounterEnd), "Phase " + (mainPhaseCount++));
                mainPhase.AddParentPhase(encounterPhase);
                mainPhase.AddTarget(keepConstruct, log);
                AddTargetsToPhase(mainPhase, targets, KCStatues, log, PhaseData.TargetPriority.NonBlocking);
                mainPhases.Add(mainPhase);
            }
        }
        phases.AddRange(mainPhases);
        // add burn phases
        int offset = phases.Count;
        IReadOnlyList<BuffEvent> orbItems = log.CombatData.GetBuffDataByIDByDst(Compromised, keepConstruct.AgentItem);
        // Get number of orbs and filter the list
        long orbStart = 0;
        int orbCount = 0;
        var segments = new List<Segment>();
        foreach (BuffEvent c in orbItems)
        {
            if (c is BuffApplyEvent)
            {
                if (orbStart == 0)
                {
                    orbStart = c.Time;
                }
                orbCount++;
            }
            else if (orbStart != 0)
            {
                segments.Add(new Segment(orbStart, Math.Min(c.Time, encounterEnd), orbCount));
                orbCount = 0;
                orbStart = 0;
            }
        }
        int burnCount = 1;
        var burnPhases = new List<PhaseData>(5);
        foreach (Segment seg in segments)
        {
            var phase = new SubPhasePhaseData(seg.Start, seg.End, "Burn " + burnCount++ + " (" + seg.Value + " orbs)");
            phase.AddTarget(keepConstruct, log);
            phase.AddParentPhases(mainPhases);
            phases.Add(phase);
            burnPhases.Add(phase);
        }
        phases.Sort((x, y) => x.Start.CompareTo(y.Start));
        // pre burn phases
        int preBurnCount = 1;
        var preBurnPhases = new List<PhaseData>(5);
        var kcInvuls = keepConstruct.GetBuffStatus(log, Determined762);
        foreach (var invul in kcInvuls)
        {
            if (invul.Value > 0)
            {
                long preBurnEnd = invul.Start;
                PhaseData? prevPhase = phases.LastOrDefault(x => x.Start <= preBurnEnd || x.End <= preBurnEnd);
                if (prevPhase != null)
                {
                    long preBurnStart = (prevPhase.End >= preBurnEnd ? prevPhase.Start : prevPhase.End) + 1;
                    if (preBurnEnd - preBurnStart > PhaseTimeLimit)
                    {
                        var phase = new SubPhasePhaseData(preBurnStart, preBurnEnd, "Pre-Burn " + preBurnCount++);
                        phase.AddParentPhases(mainPhases);
                        phase.AddTarget(keepConstruct, log);
                        AddTargetsToPhase(phase, targets, KCStatues, log, PhaseData.TargetPriority.NonBlocking);
                        preBurnPhases.Add(phase);
                    }
                }
            }
        }
        phases.AddRange(preBurnPhases);
        phases.Sort((x, y) => x.Start.CompareTo(y.Start));
        // add leftover phases
        PhaseData? cur = null;
        var leftOverPhases = new List<PhaseData>(5);
        for (int i = 0; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            if (mainPhases.Contains(phase))
            {
                cur = phase;
            }
            else if (burnPhases.Contains(phase))
            {
                if (cur != null)
                {
                    if (cur.End >= phase.End + PhaseTimeLimit && (i == phases.Count - 1 || mainPhases.Contains(phases[i + 1])))
                    {
                        var burnIndex = burnPhases.IndexOf(phase) + 1;
                        var leftOverPhase = new SubPhasePhaseData(phase.End, cur.End, "Leftover " + burnIndex);
                        leftOverPhase.AddParentPhases(mainPhases);
                        leftOverPhase.AddTarget(keepConstruct, log);
                        leftOverPhases.Add(leftOverPhase);
                    }
                }
            }
        }
        phases.AddRange(leftOverPhases);
        return phases;
    }

    internal static readonly IReadOnlyList<TargetID> KCStatues = 
    [
        TargetID.Jessica,
        TargetID.Olson,
        TargetID.Engul,
        TargetID.Faerla,
        TargetID.Caulle,
        TargetID.Henley,
        TargetID.Galletta,
        TargetID.Ianim,
    ];
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KeepConstruct)) ?? throw new MissingKeyActorsException("Keep Construct not found");
        phases[0].AddTarget(mainTarget, log);
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(KCStatues)), log, PhaseData.TargetPriority.NonBlocking);
        phases.AddRange(ComputePhases(log, mainTarget, Targets, (EncounterPhaseData)phases[0], requirePhases));
       
        return phases;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.KeepConstruct,
            ..KCStatues
        ];
    }
    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.KeepConstruct, 0 },
            {TargetID.Jessica, 1 },
            {TargetID.Olson, 1 },
            {TargetID.Engul, 1 },
            {TargetID.Faerla, 1 },
            {TargetID.Caulle, 1 },
            {TargetID.Henley, 1 },
            {TargetID.Galletta, 1 },
            {TargetID.Ianim, 1 },
        };
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.KeepConstructCore,
            TargetID.GreenPhantasm,
            TargetID.InsidiousProjection,
            TargetID.UnstableLeyRift,
            TargetID.RadiantPhantasm,
            TargetID.CrimsonPhantasm,
            TargetID.RetrieverProjection
        ];
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        long start = replay.TimeOffsets.start;
        long end = replay.TimeOffsets.end;
        switch (target.ID)
        {
            case (int)TargetID.KeepConstruct:
                // Phantasmal Blades
                int bladeDelay = 150;
                int duration = 1000;
                float bladeOpeningAngle = 360 * 3 / 32;
                uint bladeRadius = 1600;

                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        case TowerDrop:
                            {
                                start = cast.Time;
                                end = cast.EndTime;
                                long skillCast = end - 1000;
                                if (target.TryGetCurrentInterpolatedPosition(log, end, out var position))
                                {
                                    replay.Decorations.AddWithFilledWithGrowing(new CircleDecoration(400, (start, skillCast), Colors.LightOrange, 0.5, new PositionConnector(position)).UsingFilled(false), true, skillCast);
                                }
                            }
                            break;
                        case PhantasmalBlades1:
                            {
                                int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((cast.ActualDuration - 1150) / 1000.0), 9));
                                start = cast.Time + bladeDelay;
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing))
                                {
                                    var connector = new AgentConnector(target);
                                    replay.Decorations.Add(new CircleDecoration(200, (start, start + (ticks + 1) * 1000), Colors.Red, 0.4, connector));
                                    float initialAngle = facing.GetRoundedZRotationDeg();
                                    replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle))); // First blade lasts twice as long
                                    for (int i = 1; i < ticks; i++)
                                    {
                                        float angle = initialAngle + i * 360 / 8;
                                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle))); // First blade lasts longer
                                    }
                                }
                            }
                            break;
                        case PhantasmalBlades2:
                            {
                                int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((cast.ActualDuration - 1150) / 1000.0), 9));
                                start = cast.Time + bladeDelay;
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing))
                                {
                                    var connector = new AgentConnector(target);
                                    replay.Decorations.Add(new CircleDecoration(200, (start, start + (ticks + 1) * 1000), Colors.Red, 0.4, connector));
                                    float initialAngle1 = facing.GetRoundedZRotationDeg();
                                    replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle1))); // First blade lasts twice as long
                                    float initialAngle2 = RadianToDegreeF(Math.Atan2(-facing.Y, -facing.X));
                                    replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle2))); // First blade lasts twice as long
                                    for (int i = 1; i < ticks; i++)
                                    {
                                        float angle1 = initialAngle1 + i * 360 / 8;
                                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle1))); // First blade lasts longer
                                        float angle2 = initialAngle2 + i * 360 / 8;
                                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle2))); // First blade lasts longer
                                    }
                                }
                            }
                            break;
                        case PhantasmalBlades3:
                            {
                                int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((cast.ActualDuration - 1150) / 1000.0), 9));
                                start = cast.Time + bladeDelay;
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing))
                                {
                                    var connector = new AgentConnector(target);
                                    replay.Decorations.Add(new CircleDecoration(200, (start, start + (ticks + 1) * 1000), Colors.Red, 0.4, connector));
                                    float initialAngle1 = RadianToDegreeF(Math.Atan2(-facing.Y, -facing.X));
                                    replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle1))); // First blade lasts twice as long
                                    float initialAngle2 = initialAngle1 + 120;
                                    replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle2))); // First blade lasts twice as long
                                    float initialAngle3 = initialAngle1 - 120;
                                    replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle3))); // First blade lasts twice as long
                                    for (int i = 1; i < ticks; i++)
                                    {
                                        float angle1 = initialAngle1 + i * 360 / 8;
                                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle1))); // First blade lasts longer
                                        float angle2 = initialAngle2 + i * 360 / 8;
                                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle2))); // First blade lasts longer
                                        float angle3 = initialAngle3 + i * 360 / 8;
                                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle3))); // First blade lasts longer
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                var kcOrbCollect = target.GetBuffStatus(log, XerasBoon).Where(x => x.Value > 0);
                foreach (Segment seg in kcOrbCollect)
                {
                    replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, (seg.Start, seg.End), Colors.Red, 0.6, Colors.Black, 0.2, [(seg.Start, 0), (seg.End, 100)], new AgentConnector(target))
                        .UsingRotationConnector(new AngleConnector(180)));
                }
                break;
            case (int)TargetID.KeepConstructCore:
                break;
            case (int)TargetID.Jessica:
            case (int)TargetID.Olson:
            case (int)TargetID.Engul:
            case (int)TargetID.Faerla:
            case (int)TargetID.Caulle:
            case (int)TargetID.Henley:
            case (int)TargetID.Galletta:
            case (int)TargetID.Ianim:
                replay.Decorations.Add(new CircleDecoration(600, (start, end), Colors.Red, 0.5, new AgentConnector(target)).UsingFilled(false));
                replay.Decorations.Add(new CircleDecoration(400, (start, end), Colors.GreenishYellow, 0.2, new AgentConnector(target)));
                if (replay.PolledPositions.Count > 0)
                {
                    replay.Decorations.AddWithGrowing(new CircleDecoration(300, (start - 5000, start), Colors.LightBlue, 0.3, new PositionConnector(replay.PolledPositions[0].XYZ)), start);
                }
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.KeepConstructStatueDeathEmbraceRemoveTrigger, out var embraceRemoveAreas))
                {
                    foreach (var embraceRemoveArea in embraceRemoveAreas)
                    {
                        replay.Decorations.Add(new CircleDecoration(400, embraceRemoveArea.ComputeLifespan(log, 1000), Colors.GreenishYellow, 0.4, new PositionConnector(embraceRemoveArea.Position)));
                    }
                }
                break;
            case (int)TargetID.GreenPhantasm:
                int lifetime = 8000;
                replay.Decorations.AddWithGrowing(new CircleDecoration(210, (start, start + lifetime), Colors.Green, 0.2, new AgentConnector(target)), start + lifetime);
                break;
            case (int)TargetID.RetrieverProjection:
            case (int)TargetID.InsidiousProjection:
            case (int)TargetID.UnstableLeyRift:
            case (int)TargetID.RadiantPhantasm:
            case (int)TargetID.CrimsonPhantasm:
                break;
            default:
                break;
        }

    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return combatData.GetBuffApplyData(AchievementEligibilityDownDownDowned).Any(x => x.Time > -100) ? LogData.LogMode.CM : LogData.LogMode.Normal;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        // Bombs
        var xeraFury = p.GetBuffStatus(log, XerasFury).Where(x => x.Value > 0);
        foreach (Segment seg in xeraFury)
        {
            replay.Decorations.AddWithGrowing(new CircleDecoration(550, seg, Colors.Orange, 0.2, new AgentConnector(p)), seg.End);

        }
        // Fixated Statue tether to Player
        var fixatedStatue = GetBuffApplyRemoveSequence(log.CombatData, [StatueFixated1, StatueFixated2], p, true, true);
        replay.Decorations.AddTether(fixatedStatue, Colors.Magenta, 0.5);
        // Fixation Overhead
        IEnumerable<Segment> fixations = p.GetBuffStatus(log, [StatueFixated1, StatueFixated2]).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(fixations, p, ParserIcons.FixationPurpleOverhead);
        // Attunements Overhead
        IEnumerable<Segment> crimsonAttunements = p.GetBuffStatus(log, [CrimsonAttunementPhantasm, CrimsonAttunementOrb]).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(crimsonAttunements, p, ParserIcons.CrimsonAttunementOverhead);
        IEnumerable<Segment> radiantAttunements = p.GetBuffStatus(log, [RadiantAttunementPhantasm, RadiantAttunementOrb]).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(radiantAttunements, p, ParserIcons.RadiantAttunementOverhead);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KeepConstructHailOfFuryWarning, out var debrisWarnings))
        {
            foreach (var debris in debrisWarnings)
            {
                var debrisLifespan = debris.ComputeLifespan(log, 3000);
                var growingEnd = debrisLifespan.end;
                // One second for the actual drop, we have the drop effect but server delay can create visual flickering
                debrisLifespan.end += 1000;
                var debrisDecoration = new CircleDecoration(150, debrisLifespan, Colors.Orange, 0.2, new PositionConnector(debris.Position));
                environmentDecorations.AddWithGrowing(debrisDecoration, growingEnd);
            }
        }
        // Crimson Energy (red) and Radiant Energy (white) orbs
        var radiantOrbs = log.CombatData.GetMissileEventsBySkillID(RadiantEnergyWhiteOrb);
        var crimsonOrbs = log.CombatData.GetMissileEventsBySkillID(CrimsonEnergyRedOrb);
        environmentDecorations.AddNonHomingMissiles(log, radiantOrbs, Colors.White, 0.4, 25);
        environmentDecorations.AddNonHomingMissiles(log, crimsonOrbs, Colors.Red, 0.4, 25);
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }

        var encounterPhases = log.LogData.GetPhases(log).OfType<EncounterPhaseData>().Where(x => x.LogID == LogID);
        foreach (var encounterPhase in encounterPhases)
        {
            if (encounterPhase.Success && encounterPhase.IsCM)
            {
                int hasHitKc = 0;
                var kc = encounterPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.KeepConstruct));
                foreach (Player p in log.PlayerList)
                {
                    if (p.GetDamageEvents(kc, log).Any())
                    {
                        hasHitKc++;
                    }
                }
                if (hasHitKc == log.PlayerList.Count)
                {
                    instanceBuffs.Add(new(log.Buffs.BuffsByIDs[AchievementEligibilityDownDownDowned], 1, encounterPhase));
                }
            }
        }
    }
}
