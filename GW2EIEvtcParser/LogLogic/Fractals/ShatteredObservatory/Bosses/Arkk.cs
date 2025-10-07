using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Arkk : ShatteredObservatory
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup(
                [
                    new PlayerDstHealthDamageHitMechanic([ HorizonStrikeArkk1, HorizonStrikeArkk2 ], new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "Horizon Strike", "Horizon Strike (turning pizza slices during Arkk)","Horizon Strike (Arkk)", 0),
                    new PlayerDstHealthDamageHitMechanic(HorizonStrikeNormal, new MechanicPlotlySetting(Symbols.Circle,Colors.DarkRed), "Horizon Strike norm", "Horizon Strike (normal during Arkk)","Horizon Strike (normal, Arkk)", 0),
                ]
            ),
            new MechanicGroup(
                [
                    new PlayerDstHealthDamageHitMechanic(SolarFury, new MechanicPlotlySetting(Symbols.Circle,Colors.LightRed), "Ball", "Stood in Red Overhead Ball Field","Red Ball Aoe", 0),
                    new PlayerDstHealthDamageHitMechanic(SolarDischarge, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Shockwave", "Knockback shockwave after Overhead Balls","Shockwave", 0),
                    new PlayerDstHealthDamageHitMechanic(SolarStomp, new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Magenta), "Stomp", "Solar Stomp (Evading Stomp)","Evading Jump", 0),
                ]
            ),
            new PlayerDstHealthDamageHitMechanic([ DiffractiveEdge1, DiffractiveEdge2 ], new MechanicPlotlySetting(Symbols.Star,Colors.Yellow), "5 Cone", "Diffractive Edge (5 Cone Knockback)","Five Cones", 0),
            new PlayerDstHealthDamageHitMechanic(FocusedRage, new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Orange), "Cone KB", "Knockback in Cone with overhead crosshair","Knockback Cone", 0),
            new PlayerDstHealthDamageHitMechanic([ StarbustCascade1, StarbustCascade2 ], new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Float Ring", "Starburst Cascade (Expanding/Retracting Lifting Ring)","Float Ring", 500),
            new PlayerDstHealthDamageHitMechanic(OverheadSmash, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.LightRed), "Smash", "Overhead Smash","Overhead Smash",0),
            new PlayerDstHealthDamageHitMechanic(ExplodeArkk, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Bloom Explode", "Hit by Solar Bloom explosion","Bloom Explosion", 0),
            new PlayerDstBuffApplyMechanic(CosmicMeteor, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "Green", "Temporal Realignment (Green) application","Green", 0),
            new MechanicGroup(
                [
                    new EnemyCastStartMechanic(ArkkBreakbarCast, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "Breakbar", "Start Breakbar","CC", 0),
                    new EnemyDstBuffApplyMechanic(Exposed31589, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC.Fail", "Breakbar (Failed CC)","CC Fail", 0)
                        .UsingChecker((bae,log) => bae.To.IsSpecies(TargetID.Arkk) && !log.CombatData.GetAnimatedCastData(ArkkBreakbarCast).Any(x => bae.To.Is(x.Caster) && x.Time < bae.Time && bae.Time < x.ExpectedEndTime + ServerDelayConstant)),
                    new EnemyDstBuffApplyMechanic(Exposed31589, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed", "Breakbar broken","CCed", 0)
                        .UsingChecker((bae,log) => bae.To.IsSpecies(TargetID.Arkk) && log.CombatData.GetAnimatedCastData(ArkkBreakbarCast).Any(x => bae.To.Is(x.Caster) && x.Time < bae.Time && bae.Time < x.ExpectedEndTime + ServerDelayConstant)),
                ]
            ),
            new PlayerDstHealthDamageHitMechanic(OverheadSmashArchdiviner, new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.LightRed), "A.Smsh", "Overhead Smash (Arcdiviner)","Smash (Add)", 0),
            new PlayerDstHealthDamageHitMechanic(RollingChaos, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightRed), "KD Marble", "Rolling Chaos (Arrow marble)","KD Marble", 0),
            new EnemyCastStartMechanic(CosmicStreaks, new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Pink), "DDR Beam", "Triple Death Ray Cast (last phase)","Death Ray Cast", 0),
            new PlayerDstHealthDamageHitMechanic(WhirlingDevastation, new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.DarkPink), "Whirl", "Whirling Devastation (Gladiator Spin)","Gladiator Spin", 300),
            new MechanicGroup(
                [
                    new EnemyCastStartMechanic(PullCharge, new MechanicPlotlySetting(Symbols.Bowtie,Colors.DarkTeal), "Pull", "Pull Charge (Gladiator Pull)","Gladiator Pull", 0), //
                    new EnemyCastEndMechanic(PullCharge, new MechanicPlotlySetting(Symbols.Bowtie,Colors.Red), "Pull CC Fail", "Pull Charge CC failed","CC fail (Gladiator)", 0)
                        .UsingChecker((ce,log) => ce.ActualDuration > 3200), //
                    new EnemyCastEndMechanic(PullCharge, new MechanicPlotlySetting(Symbols.Bowtie,Colors.DarkGreen), "Pull CCed", "Pull Charge CCed","CCed (Gladiator)", 0)
                        .UsingChecker((ce, log) => ce.ActualDuration < 3200), //
                ]
            ),
            new PlayerDstHealthDamageHitMechanic(SpinningCut, new MechanicPlotlySetting(Symbols.StarSquareOpen,Colors.LightPurple), "Daze", "Spinning Cut (3rd Gladiator Auto->Daze)","Gladiator Daze", 0), //
        ]);
    public Arkk(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "arkk";
        Icon = EncounterIconArkk;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000003;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (914, 914),
                        (-19231, -18137, -16591, -15677)/*,
                        (-6144, -6144, 9216, 9216),
                        (11804, 4414, 12444, 5054)*/);
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayArkk, crMap);
        return crMap;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        var trashIDs = new List<TargetID>(9 + base.GetTrashMobsIDs().Count);
        trashIDs.AddRange(base.GetTrashMobsIDs());
        trashIDs.Add(TargetID.FanaticDagger2);
        trashIDs.Add(TargetID.FanaticDagger1);
        trashIDs.Add(TargetID.FanaticBow);
        trashIDs.Add(TargetID.SolarBloom);
        trashIDs.Add(TargetID.BLIGHT);
        trashIDs.Add(TargetID.PLINK);
        trashIDs.Add(TargetID.DOC);
        trashIDs.Add(TargetID.CHOP);
        trashIDs.Add(TargetID.ProjectionArkk);
        return trashIDs;
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.LogMode.CMNoName;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Arkk,
            TargetID.Archdiviner,
            TargetID.EliteBrazenGladiator,
            TargetID.TemporalAnomalyArkk,
        ];
    }

    private static void GetMiniBossPhase(TargetID targetID, ParsedEvtcLog log, IReadOnlyList<SingleActor> targets, string phaseName, List<PhaseData> phases, EncounterPhaseData encounterPhase)
    {
        SingleActor? target = targets.FirstOrDefault(x => x.IsSpecies(targetID));
        if (target == null)
        {
            return;
        }
        var phaseData = new SubPhasePhaseData(Math.Max(target.FirstAware, log.LogData.LogStart), Math.Min(target.LastAware, log.LogData.LogEnd), phaseName);
        AddTargetsToPhaseAndFit(phaseData, targets, [targetID], log);
        phases.Add(phaseData);
        phaseData.AddParentPhase(encounterPhase);
    }

    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor arkk, IReadOnlyList<SingleActor> targets, IReadOnlyList<SingleActor> trashMobs, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<PhaseData>(11);
        phases.AddRange(GetPhasesByInvul(log, Determined762, arkk, false, true, encounterPhase.Start, encounterPhase.End));
        for (int i = 0; i < phases.Count; i++)
        {
            phases[i].Name = "Phase " + (i + 1);
            phases[i].AddParentPhase(encounterPhase);
            phases[i].AddTarget(arkk, log);
        }
        var encounterMiniBosses = targets.Where(x => x.IsAnySpecies([TargetID.Archdiviner, TargetID.EliteBrazenGladiator]) && encounterPhase.InInterval(x.FirstAware)).ToList();
        GetMiniBossPhase(TargetID.Archdiviner, log, encounterMiniBosses, "Archdiviner", phases, encounterPhase);
        GetMiniBossPhase(TargetID.EliteBrazenGladiator, log, encounterMiniBosses, "Brazen Gladiator", phases, encounterPhase);

        var bloomPhases = new List<PhaseData>(10);
        var encounterBlooms = trashMobs.Where(x => x.IsSpecies(TargetID.SolarBloom) && encounterPhase.InInterval(x.FirstAware)).OrderBy(x => x.FirstAware);
        foreach (NPC bloom in encounterBlooms)
        {
            long start = bloom.FirstAware;
            long end = bloom.LastAware;
            var phase = bloomPhases.FirstOrDefault(x => Math.Abs(x.Start - start) < 100); // some blooms can be delayed
            if (phase != null)
            {
                phase.OverrideStart(Math.Min(phase.Start, start));
                phase.OverrideEnd(Math.Max(phase.End, end));
            }
            else
            {
                bloomPhases.Add(new SubPhasePhaseData(start, end));
            }
        }
        var invuls = arkk.GetBuffStatus(log, Determined762);
        for (int i = 0; i < bloomPhases.Count; i++)
        {
            PhaseData phase = bloomPhases[i];
            phase.AddParentPhase(encounterPhase);
            phase.Name = $"Blooms {i + 1}";
            phase.AddTarget(arkk, log);
            var invulLoss = invuls.FirstOrNull((in Segment x) => x.Start > phase.Start && x.Value == 0);
            phase.OverrideEnd(Math.Min(phase.End, invulLoss?.Start ?? log.LogData.LogEnd));
        }
        phases.AddRange(bloomPhases);

        // Add anomalies as secondary target to the phases
        var anomalies = targets.Where(x => x.IsSpecies(TargetID.TemporalAnomalyArkk));
        for (int i = 0; i < phases.Count; i++)
        {
            phases[i].AddTargets(anomalies, log, PhaseData.TargetPriority.Blocking);
        }

        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor arkk = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Arkk)) ?? throw new MissingKeyActorsException("Arkk not found");
        phases[0].AddTarget(arkk, log);
        phases[0].AddTargets(Targets.Where(x => x.IsSpecies(TargetID.Archdiviner) || x.IsSpecies(TargetID.EliteBrazenGladiator)), log, PhaseData.TargetPriority.Blocking);
        phases.AddRange(ComputePhases(log, arkk, Targets, TrashMobs, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        var arkk = agentData.GetNPCsByID(TargetID.Arkk).FirstOrDefault() ?? throw new MissingKeyActorsException("Arkk not found");
        CombatItem? startBuffApply = combatData.FirstOrDefault(x => x.SkillID == ArkkStartBuff && x.SrcMatchesAgent(arkk) && x.IsBuffApply());
        return startBuffApply?.Time ?? GetLogOffsetBySpawn(logData, combatData, arkk);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, logData, playerAgents);
        // reward or death worked
        if (logData.Success)
        {
            return;
        }
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Arkk)) ?? throw new MissingKeyActorsException("Arkk not found");
        // missing buff apply events fallback, some phases will be missing
        // removes should be present
        if (SetSuccessByBuffCount(combatData, logData, playerAgents, target, Determined762, 10))
        {
            var invulsRemoveTarget = combatData.GetBuffDataByIDByDst(Determined762, target.AgentItem).OfType<BuffRemoveAllEvent>();
            if (invulsRemoveTarget.Count() == 5)
            {
                SetSuccessByCombatExit([target], combatData, logData, playerAgents);
            }
        }
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IsInstance)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
        var encounterPhases = log.LogData.GetPhases(log).OfType<EncounterPhaseData>().Where(x => x.LogID == LogID);
        var finalEncounter = encounterPhases.LastOrDefault();
        if (finalEncounter != null && finalEncounter.Success)
        {
            IReadOnlyList<BuffEvent> beDynamic = log.CombatData.GetBuffData(AchievementEligibilityBeDynamic);
            int counter = 0;

            if (beDynamic.Any() && finalEncounter.Success)
            {
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, AchievementEligibilityBeDynamic, finalEncounter.End - ServerDelayConstant))
                    {
                        counter++;
                    }
                }
            }
            // The party must have 5 players to be eligible
            if (counter == 5)
            {
                instanceBuffs.Add(new(log.Buffs.BuffsByIDs[AchievementEligibilityBeDynamic], 1, log.LogData.GetMainPhase(log)));
            }
        }

    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }

        // Cosmic Meteor (green)
        IEnumerable<Segment> cosmicMeteors = p.GetBuffStatus(log, CosmicMeteor).Where(x => x.Value > 0);
        foreach (Segment cosmicMeteor in cosmicMeteors)
        {
            int start = (int)cosmicMeteor.Start;
            int end = (int)cosmicMeteor.End;
            replay.Decorations.AddWithGrowing(new CircleDecoration(180, (start, end), Colors.DarkGreen, 0.4, new AgentConnector(p)), end);
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }

        switch (target.ID)
        {
            case (int)TargetID.Arkk:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        case SolarBlastArkk1:
                            replay.Decorations.AddOverheadIcon(new Segment((int)cast.Time, cast.EndTime, 1), target, ParserIcons.EyeOverhead, 30);
                            break;
                        case SupernovaArkk:
                            // TODO: add growing square
                            break;
                        case HorizonStrikeArkk1:
                        case HorizonStrikeArkk2:
                            if (log.CombatData.HasEffectData)
                            {
                                break;
                            }

                            int offset = 520; // ~520ms at the start and between
                            int castDuration = 2600;
                            var connector = new AgentConnector(target);
                            var rotation = replay.PolledRotations.FirstOrNull((in ParametricPoint3D x) => x.Time >= cast.Time);
                            if (!rotation.HasValue)
                            {
                                break;
                            }

                            var applies = log.CombatData.GetBuffApplyDataByDst(target.AgentItem).OfType<BuffApplyEvent>().Where(x => x.Time > cast.Time);
                            BuffApplyEvent? nextInvul = applies.FirstOrDefault(x => x.BuffID == Determined762);
                            BuffApplyEvent? nextStun = applies.FirstOrDefault(x => x.BuffID == Stun);
                            long cap = Math.Min(nextInvul?.Time ?? log.LogData.LogEnd, nextStun?.Time ?? log.LogData.LogEnd);
                            long actualEndCast = ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration);
                            float facing = rotation.Value.XYZ.GetRoundedZRotationDeg();
                            for (int i = 0; i < 5; i++)
                            {
                                long start = cast.Time + offset * (i + 1);
                                long end = start + castDuration;
                                if (cast.SkillID == HorizonStrikeArkk1)
                                {
                                    float angle = facing + 180 / 5 * i;
                                    if (start >= cap)
                                    {
                                        break;
                                    }
                                    replay.Decorations.Add(new PieDecoration(1500, 30, (start, end), Colors.Orange, 0.2, connector).UsingRotationConnector(new AngleConnector(angle + 180)));
                                    replay.Decorations.Add(new PieDecoration(1500, 30, (end, end + 300), Colors.Red, 0.2, connector).UsingRotationConnector(new AngleConnector(angle + 180)));
                                    replay.Decorations.Add(new PieDecoration(1500, 30, (start, end), Colors.Orange, 0.2, connector).UsingRotationConnector(new AngleConnector(angle)));
                                    replay.Decorations.Add(new PieDecoration(1500, 30, (end, end + 300), Colors.Red, 0.2, connector).UsingRotationConnector(new AngleConnector(angle)));
                                }
                                else if (cast.SkillID == HorizonStrikeArkk2)
                                {
                                    float angle = facing + 90 - 180 / 5 * i;
                                    if (start >= cap)
                                    {
                                        break;
                                    }
                                    replay.Decorations.Add(new PieDecoration(1500, 30, (start, end), Colors.Orange, 0.2, connector).UsingRotationConnector(new AngleConnector(angle)));
                                    replay.Decorations.Add(new PieDecoration(1500, 30, (end, end + 300), Colors.Red, 0.2, connector).UsingRotationConnector(new AngleConnector(angle)));
                                    replay.Decorations.Add(new PieDecoration(1500, 30, (start, end), Colors.Orange, 0.2, connector).UsingRotationConnector(new AngleConnector(angle + 180)));
                                    replay.Decorations.Add(new PieDecoration(1500, 30, (end, end + 300), Colors.Red, 0.2, connector).UsingRotationConnector(new AngleConnector(angle + 180)));
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
                // case (int)TargetID.TemporalAnomalyArkk:
                //     if (!log.CombatData.HasEffectData)
                //     {
                //         foreach (ExitCombatEvent exitCombat in log.CombatData.GetExitCombatEvents(target.AgentItem))
                //         {
                //             int start = (int)exitCombat.Time;
                //             BuffRemoveAllEvent skullRemove = log.CombatData.GetBuffRemoveAllData(CorporealReassignmentBuff).FirstOrDefault(x => x.Time >= exitCombat.Time + ServerDelayConstant);
                //             int end = Math.Min((int?)skullRemove?.Time ?? int.MaxValue, start + 11000); // cap at 11s spawn to explosion
                //             ParametricPoint3D anomalyPos = replay.PolledPositions.LastOrDefault(x => x.Time <= exitCombat.Time + ServerDelayConstant);
                //             if (anomalyPos != null)
                //             {
                //                 replay.Decorations.Add(new CircleDecoration(false, 0, 220, (start, end), Colors.LightBlue, 0.4, new PositionConnector(anomalyPos)));
                //             }
                //         }
                //     }
                //     break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        // Horizon Strike
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HorizonStrikeArkk, out var strikes))
        {
            foreach (EffectEvent effect in strikes)
            {
                int start = (int)effect.Time;
                int end = start + 2600; // effect has 3833ms duration for some reason
                var rotation = new AngleConnector(effect.Rotation.Z + 90);
                environmentDecorations.Add(new PieDecoration(1400, 30, (start, end), Colors.Orange, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(rotation));
                environmentDecorations.Add(new PieDecoration(1400, 30, (end, end + 300), Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(rotation));
            }
        }
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        List<CastEvent> res = [];
        res.AddRange(ProfHelper.ComputeUnderBuffCastEvents(combatData, skillData, HypernovaLaunchSAK, HypernovaLaunchBuff));
        return res;
    }
}
