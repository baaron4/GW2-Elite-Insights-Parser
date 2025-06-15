using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class SoullessHorror : HallOfChains
{
    public SoullessHorror(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new MechanicGroup([
                new PlayerDstHitMechanic(InnerVortexSlash, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Donut In", "Vortex Slash (Inner Donut hit)","Inner Donut", 0),
                new PlayerDstHitMechanic(OuterVortexSlash, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Donut Out", "Vortex Slash (Outer Donut hit)","Outer Donut", 0),
                new PlayerDstHitMechanic([ InnerVortexSlash, OuterVortexSlash ], new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightOrange), "NecDancer.Achiv", "Achievement Eligibility: Necro Dancer", "Necro Dancer", 0)
                    .UsingAchievementEligibility(),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(QuadSlashFirstSet, new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.LightOrange), "Slice1", "Quad Slash (4 Slices, First hit)","4 Slices 1", 0),
                new PlayerDstHitMechanic(QuadSlashSecondSet, new MechanicPlotlySetting(Symbols.StarSquareOpen,Colors.LightOrange), "Slice2", "Quad Slash (4 Slices, Second hit)","4 Slices 2", 0),
                new PlayerDstHitMechanic(DeathBloom, new MechanicPlotlySetting(Symbols.Octagon,Colors.LightOrange), "8Slice", "Death Bloom (8 Slices)","8 Slices", 0),
            ]),
            new PlayerDstHitMechanic(SpinningSlash, new MechanicPlotlySetting(Symbols.StarTriangleUpOpen,Colors.DarkRed), "Scythe", "Spinning Slash (hit by Scythe)","Scythe", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(FixatedSH, new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixate", "Fixated (Special Action Key)","Fixated", 0),
                new PlayerDstBuffApplyMechanic(Necrosis, new MechanicPlotlySetting(Symbols.StarOpen,Colors.Magenta), "Necrosis", "Necrosis (Tanking Debuff)","Necrosis Debuff", 50),
            ]),
            new PlayerDstHitMechanic(CorruptTheLiving, new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Spin", "Corrupt the Living (Torment+Poison Spin)","Torment+Poison Spin", 0),
            new PlayerDstHitMechanic(WurmSpit, new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.DarkTeal), "Spit", "Wurm Spit","Wurm Spit", 0),
            new MechanicGroup([
                new EnemyCastStartMechanic(HowlingDeath, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC", "Howling Death (Breakbar)","Breakbar", 0),
                new EnemyCastEndMechanic(HowlingDeath, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed", "Howling Death (Breakbar) broken","CCed", 0)
                    .UsingChecker((ce, log) => ce.ActualDuration <= 6800),
                new EnemyCastEndMechanic(HowlingDeath, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail", "Howling Death (Breakbar failed) ","CC Fail", 0)
                    .UsingChecker((ce,log) => ce.ActualDuration > 6800),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(SoulRift, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Golem", "Soul Rift (stood in Golem Aoe)","Golem Aoe", 0),
                new PlayerSrcBuffApplyMechanic(Immobile, new MechanicPlotlySetting(Symbols.X,Colors.Red), "Immob.Golem", "Immobilized Golem","Immobilized Golem", 50).UsingChecker((ce, log) => ce.To.IsSpecies(TargetID.TormentedDead)),
            ]),
        ]));
        Extension = "sh";
        ChestID = ChestID.ChestOfDesmina;
        Icon = EncounterIconSoullessHorror;
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000001;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(ChillingAura, ChillingAura),
            new BuffGainCastFinder(IssueChallengeSAK, FixatedSH),
        ];
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplaySoullessHorror,
                        (1000, 1000),
                        (-12223, -771, -8932, 2420)/*,
                        (-21504, -12288, 24576, 12288),
                        (19072, 15484, 20992, 16508)*/);
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.SoullessHorror,
            TargetID.TormentedDead,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.Scythe,
            TargetID.SurgingSoul,
            TargetID.FleshWurm
        ];
    }

    internal override IEnumerable<ErrorEvent> GetCustomWarningMessages(FightData fightData, EvtcVersionEvent evtcVersion)
    {
        return base.GetCustomWarningMessages(fightData, evtcVersion)
            .Concat(GetConfusionDamageMissingMessage(evtcVersion).ToEnumerable());
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, fightData, playerAgents);
        if (!fightData.Success)
        {
            SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.SoullessHorror)) ?? throw new MissingKeyActorsException("Soulless Horror not found");
            BuffEvent? buffOnDeath = combatData.GetBuffDataByIDByDst(Determined895, mainTarget.AgentItem).Where(x => x is BuffApplyEvent).LastOrDefault();
            if (buffOnDeath != null)
            {
                if (agentData.GetNPCsByID(TargetID.Desmina).Any(x => x.FirstAware <= buffOnDeath.Time + ServerDelayConstant && x.LastAware >= buffOnDeath.Time))
                {
                    fightData.SetSuccess(true, buffOnDeath.Time);
                }
            }
        }
    }
    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadget(ChestID, agentData, combatData, ChestOfDesminaPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        // discard hp update events after determined apply
        SingleActor soullessHorror = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.SoullessHorror)) ?? throw new MissingKeyActorsException("Soulless Horror not found");
        CombatItem? determined895Apply = combatData.LastOrDefault(x => x.SkillID == Determined895 && x.IsBuffApply() && x.DstMatchesAgent(soullessHorror.AgentItem));
        if (determined895Apply != null)
        {
            foreach (var combatEvent in combatData.Where(x => x.IsStateChange == StateChange.HealthUpdate && x.SrcMatchesAgent(soullessHorror.AgentItem) && x.Time >= determined895Apply.Time))
            {
                combatEvent.OverrideSrcAgent(0);
            }
        }
    }

    /*internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
    {
        var res = new List<AbstractBuffEvent>();
        // Necrosis adjust
        AdjustTimeRefreshBuff(buffsByDst, buffsById, 47414);
        return res;
    }*/

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        long fightEnd = log.FightData.FightEnd;
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.SoullessHorror)) ?? throw new MissingKeyActorsException("Soulless Horror not found");
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }
        var tormentedDeads = Targets.Where(x => x.IsSpecies(TargetID.TormentedDead));
        var howling = mainTarget.GetCastEvents(log, log.FightData.FightStart, fightEnd).Where(x => x.SkillId == HowlingDeath);
        long start = 0;
        int i = 1;
        foreach (CastEvent c in howling)
        {
            var phase = new PhaseData(start, Math.Min(c.Time, fightEnd), "Pre-Breakbar " + i++);
            phase.AddTarget(mainTarget, log);
            phase.AddTargets(tormentedDeads, log, PhaseData.TargetPriority.NonBlocking);
            phase.AddParentPhase(phases[0]);
            start = c.EndTime;
            phases.Add(phase);
        }
        if (fightEnd - start > PhaseTimeLimit)
        {
            var lastPhase = new PhaseData(start, fightEnd, "Final");
            lastPhase.AddTarget(mainTarget, log);
            lastPhase.AddTargets(tormentedDeads, log, PhaseData.TargetPriority.NonBlocking);
            lastPhase.AddParentPhase(phases[0]);
            phases.Add(lastPhase);
        }

        return phases;
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        (long start, long end) lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);

        switch (target.ID)
        {
            case (int)TargetID.SoullessHorror:
                var cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
                // arena reduction
                var center = new Vector3(-10581, 825, -817);
                List<(double, uint, uint)> destroyedRings;
                if (log.FightData.IsCM)
                {
                    destroyedRings =
                        [
                            (100, 1330, 1550),
                            (90, 1120, 1330),
                            (66, 910, 1120),
                            (33, 720, 910)
                        ];
                }
                else
                {
                    destroyedRings =
                        [
                            (90, 1330, 1550),
                            (66, 1120, 1330),
                            (33, 910, 1120),
                        ];
                }
                foreach ((double hpVal, uint innerRadius, uint outerRadius) in destroyedRings)
                {
                    Segment? hpUpdate = target.GetHealthUpdates(log).FirstOrNull((in Segment x) => x.Value <= hpVal);
                    if (hpUpdate != null)
                    {
                        var doughnut = new DoughnutDecoration(innerRadius, outerRadius, (hpUpdate.Value.Start, log.FightData.FightEnd), Colors.Orange, 0.3, new PositionConnector(center));
                        replay.Decorations.AddWithGrowing(doughnut, hpUpdate.Value.Start + 3000);
                    }
                    else
                    {
                        break;
                    }
                }

                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        case HowlingDeath:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, lifespan, Colors.Red, 0.6, Colors.Black, 0.2, [(cast.Time, 0), (cast.ExpectedEndTime, 100)], new AgentConnector(target))
                            .UsingRotationConnector(new AngleConnector(180)));
                            break;
                        case InnerVortexSlash:
                            castDuration = 4000;
                            long doughnutDuration = 1000;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            (long start, long end) lifespanDoughnut = (lifespan.end, lifespan.end + doughnutDuration);
                            if (target.TryGetCurrentInterpolatedPosition(log, lifespan.start, out var position))
                            {
                                var innerCircle = new CircleDecoration(380, lifespan, Colors.LightOrange, 0.2, new PositionConnector(position));
                                replay.Decorations.AddWithFilledWithGrowing(innerCircle, true, lifespan.end);
                                var outerDoughnut = new DoughnutDecoration(380, 760, lifespanDoughnut, Colors.LightOrange, 0.2, new PositionConnector(position));
                                replay.Decorations.AddWithFilledWithGrowing(outerDoughnut, true, lifespanDoughnut.end);
                            }
                            break;
                        case DeathBloom:
                            {
                                lifespan = (cast.Time, cast.EndTime);
                                if (target.TryGetCurrentFacingDirection(log, lifespan.start + 500, out var facing))
                                {
                                    float initialAngle = facing.GetRoundedZRotationDeg();
                                    var connector = new AgentConnector(target);
                                    for (int i = 0; i < 8; i++)
                                    {
                                        var rotationConnector = new AngleConnector(initialAngle + (i * 360 / 8));
                                        replay.Decorations.Add(new PieDecoration(3500, 360 / 12, lifespan, Colors.Yellow, 0.5, connector).UsingRotationConnector(rotationConnector));
                                    }
                                }
                            }
                            break;
                        case QuadSlashFirstSet:
                            {
                                lifespan = (cast.Time, cast.EndTime);
                                if (target.TryGetCurrentFacingDirection(log, lifespan.start + 500, out var facing))
                                {
                                    float initialAngle = facing.GetRoundedZRotationDeg();
                                    var connector = new AgentConnector(target);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        var rotationConnector = new AngleConnector(initialAngle + (i * 360 / 4));
                                        replay.Decorations.Add(new PieDecoration(3500, 360 / 12, lifespan, Colors.Yellow, 0.5, connector).UsingRotationConnector(rotationConnector));
                                    }
                                }
                            }
                            break;
                        case QuadSlashSecondSet:
                            {
                                lifespan = (cast.Time, cast.EndTime);
                                if (target.TryGetCurrentFacingDirection(log, lifespan.start + 500, out var facing))
                                {
                                    float initialAngle = facing.GetRoundedZRotationDeg();
                                    var connector = new AgentConnector(target);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        var rotationConnector = new AngleConnector(initialAngle + 45 + (i * 360 / 4));
                                        replay.Decorations.Add(new PieDecoration(3500, 360 / 12, lifespan, Colors.Yellow, 0.5, connector).UsingRotationConnector(rotationConnector));
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.Scythe:
                replay.Decorations.Add(new CircleDecoration(80, lifespan, Colors.Red, 0.5, new AgentConnector(target)));
                break;

            case (int)TargetID.TormentedDead:
                if (!log.CombatData.HasEffectData)
                {
                    if (replay.Positions.Count == 0)
                    {
                        break;
                    }
                    replay.Decorations.Add(new CircleDecoration(400, (lifespan.end, lifespan.end + 60000), Colors.Red, 0.5, new PositionConnector(replay.Positions.Last().XYZ)));
                }
                break;

            case (int)TargetID.SurgingSoul:
                if (replay.Positions.Count < 2)
                {
                    break;
                }

                var firstPos = replay.Positions[0].XYZ;
                if (firstPos.X < -12000 || firstPos.X > -9250)
                {
                    replay.Decorations.Add(new RectangleDecoration(240, 660, lifespan, Colors.Orange, 0.5, new AgentConnector(target)));
                    break;
                }
                else if (firstPos.Y < -525 || firstPos.Y > 2275)
                {
                    replay.Decorations.Add(new RectangleDecoration(645, 238, lifespan, Colors.Orange, 0.5, new AgentConnector(target)));
                    break;
                }
                break;

            case (int)TargetID.FleshWurm:
                break;

            default:
                break;
        }

    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);
        IEnumerable<Segment> fixations = player.GetBuffStatus(log, FixatedSH, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(fixations, player, ParserIcons.FixationPurpleOverhead);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        (long start, long end) lifespan;

        // Soul Rift - Tormented Dead death AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SoullessHorrorSoulRift, out var soulRifts))
        {
            foreach (EffectEvent effect in soulRifts)
            {
                lifespan = effect.ComputeLifespan(log, 57000);
                environmentDecorations.Add(new CircleDecoration(400, lifespan, Colors.Red, 0.5, new PositionConnector(effect.Position)));
            }
        }
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        // split necrosis
        var splitNecrosis = new Dictionary<AgentItem, List<BuffEvent>>();
        foreach (BuffEvent c in combatData.GetBuffData(Necrosis))
        {
            if (c is not BuffApplyEvent)
            {
                continue;
            }

            AgentItem tank = c.To;
            if (!splitNecrosis.TryGetValue(tank, out var value))
            {
                value = [];
                splitNecrosis.Add(tank, value);
            }

            value.Add(c);
        }

        if (splitNecrosis.Count == 0)
        {
            return 0;
        }

        List<BuffEvent> longestNecrosis = splitNecrosis.Values.OrderByDescending(x => x.Count).First();
        long minDiff = long.MaxValue;
        for (int i = 0; i < longestNecrosis.Count - 1; i++)
        {
            BuffEvent cur = longestNecrosis[i];
            BuffEvent next = longestNecrosis[i + 1];
            long timeDiff = next.Time - cur.Time;
            if (timeDiff > 1000 && minDiff > timeDiff)
            {
                minDiff = timeDiff;
            }
        }
        return (minDiff < 11000) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }
}
