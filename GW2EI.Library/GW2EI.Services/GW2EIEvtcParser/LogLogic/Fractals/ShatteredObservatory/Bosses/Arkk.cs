using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.MechanicIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Arkk : ShatteredObservatory
{
    internal readonly MechanicGroup Mechanics = new([
            new MechanicGroup(
                [
                    new PlayerDstHealthDamageHitMechanic([ HorizonStrikeArkk1, HorizonStrikeArkk2 ], Mech_HorizonStrikeArkk, new (Symbols.Circle, Colors.LightOrange), new("Horizon Strike.A", "Horizon Strike (turning pizza slices during Arkk)","Horizon Strike (Arkk)"), Sev1),
                    new PlayerDstHealthDamageHitMechanic(HorizonStrikeNormal, Mech_HorizonStrikeArkkNormal, new (Symbols.Circle,Colors.DarkRed), new("Horizon Strike norm", "Horizon Strike (normal during Arkk)","Horizon Strike (normal, Arkk)"), Sev1),
                ]
            ),
            new MechanicGroup(
                [
                    new PlayerDstHealthDamageHitMechanic(SolarFury, Mech_SolarFury, new (Symbols.Circle,Colors.LightRed), new("Ball", "Stood in Red Overhead Ball Field","Red Ball Aoe"), Sev0),
                    new PlayerDstHealthDamageHitMechanic(SolarDischarge, Mech_SolarDischarge, new (Symbols.CircleOpen,Colors.Red), new("Shockwave", "Knockback shockwave after Overhead Balls","Shockwave"), Sev0),
                    new PlayerDstHealthDamageHitMechanic(SolarStomp, Mech_SolarStomp, new (Symbols.TriangleUp,Colors.Magenta), new("Stomp", "Solar Stomp (Evading Stomp)","Evading Jump"), Sev1),
                ]
            ),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([ DiffractiveEdge1, DiffractiveEdge2 ], Mech_DiffractiveEdge, new (Symbols.Star,Colors.Yellow), new("5 Cone", "Diffractive Edge (5 Cone Knockback)","Five Cones"), Sev1),
                new PlayerDstHealthDamageHitMechanic(FocusedRage, Mech_FocusedRage, new (Symbols.TriangleDown,Colors.Orange), new("Cone KB", "Knockback in Cone with overhead crosshair","Knockback Cone"), Sev1),
            ]),
            new PlayerDstHealthDamageHitMechanic([ StarburstCascade1, StarburstCascade2 ], Mech_StarburstCascade, new (Symbols.CircleOpen,Colors.LightOrange), new("Float Ring", "Starburst Cascade (Expanding/Retracting Lifting Ring)","Float Ring"), Sev0, 500),
            new PlayerDstHealthDamageHitMechanic(OverheadSmash, Mech_OverheadSmashArkk, new (Symbols.TriangleLeft,Colors.LightRed), new("Smash", "Overhead Smash","Overhead Smash"), Sev1),
            new PlayerDstHealthDamageHitMechanic(ExplodeArkk, Mech_ExplodeArkk, new (Symbols.Circle,Colors.Yellow), new("Bloom Explode", "Hit by Solar Bloom explosion","Bloom Explosion"), Sev1),
            new PlayerDstBuffApplyMechanic(CosmicMeteor, Mech_CosmicMeteor, new (Symbols.CircleOpen,Colors.Green), new("Green", "Temporal Realignment (Green) application","Green"), Sev0),
            new MechanicGroup(
                [
                    new EnemyCastStartMechanic(ArkkBreakbarCast, Mech_ArkkBreakbarStart, new (Symbols.DiamondTall,Colors.DarkTeal), new("Breakbar", "Start Breakbar","CC"), Sev3),
                    new EnemyDstBuffApplyMechanic(Exposed31589, Mech_ArkkBreakbarFail, new (Symbols.DiamondTall,Colors.Red), new("CC.Fail", "Breakbar (Failed CC)","CC Fail"), Sev0)
                        .UsingChecker((bae,log) => bae.To.IsSpecies(TargetID.Arkk) && !log.CombatData.GetAnimatedCastData(ArkkBreakbarCast).Any(x => bae.To.Is(x.Caster) && x.Time < bae.Time && bae.Time < x.ExpectedEndTime + ServerDelayConstant)),
                    new EnemyDstBuffApplyMechanic(Exposed31589, Mech_ArkkBreakbarSuccess, new (Symbols.DiamondTall,Colors.DarkGreen), new("CCed", "Breakbar broken","CCed"), Sev0)
                        .UsingChecker((bae,log) => bae.To.IsSpecies(TargetID.Arkk) && log.CombatData.GetAnimatedCastData(ArkkBreakbarCast).Any(x => bae.To.Is(x.Caster) && x.Time < bae.Time && bae.Time < x.ExpectedEndTime + ServerDelayConstant)),
                ]
            ),
            new PlayerDstHealthDamageHitMechanic(OverheadSmashArchdiviner, Mech_OverheadSmashArkkArchDiviner, new (Symbols.TriangleLeftOpen,Colors.LightRed), new("A.Smsh", "Overhead Smash (Arcdiviner)","Smash (Add)"), Sev2),
            new PlayerDstHealthDamageHitMechanic(RollingChaos, Mech_RollingChaos, new (Symbols.CircleOpen,Colors.LightRed), new("KD Marble", "Rolling Chaos (Arrow marble)","KD Marble"), Sev1),
            new EnemyCastStartMechanic(CosmicStreaks, Mech_CosmicStreaks, new (Symbols.DiamondOpen,Colors.Pink), new("DDR Beam", "Triple Death Ray Cast (last phase)","Death Ray Cast"), Sev0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(WhirlingDevastation, Mech_WhirlingDevastation, new (Symbols.StarDiamondOpen,Colors.DarkPink), new("Whirl", "Whirling Devastation (Gladiator Spin)","Gladiator Spin"), Sev2, 300),
                new MechanicGroup(
                    [
                        new EnemyCastStartMechanic(PullCharge, Mech_PullArkkGladiatorStart, new (Symbols.Bowtie,Colors.DarkTeal), new("Pull", "Pull Charge (Gladiator Pull)","Gladiator Pull"), Sev1), //
                        new EnemyCastEndMechanic(PullCharge, Mech_PullArkkGladiatorFail, new (Symbols.Bowtie,Colors.Red), new("Pull CC Fail", "Pull Charge CC failed","CC fail (Gladiator)"), Sev1)
                            .UsingChecker((ce,log) => ce.ActualDuration > 3200), //
                        new EnemyCastEndMechanic(PullCharge, Mech_PullArkkGladiatorSuccess, new (Symbols.Bowtie,Colors.DarkGreen), new("Pull CCed", "Pull Charge CCed","CCed (Gladiator)"), Sev1)
                            .UsingChecker((ce, log) => ce.ActualDuration < 3200), //
                    ]
                ),
                new PlayerDstHealthDamageHitMechanic(SpinningCut, Mech_SpinningCut, new (Symbols.StarSquareOpen,Colors.LightPurple), new("Daze", "Spinning Cut (3rd Gladiator Auto->Daze)","Gladiator Daze"), Sev1), //
            ]),
        ]);

    public Arkk(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "arkk";
        Icon = EncounterIconArkk;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000003;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        var crMap = new CombatReplayMap(
                        (914, 914),
                        (-19291, -18274, -16571, -15554));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayArkk, crMap, parentMap);
        return crMap;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        IdentifyGadgets(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        var trashIDs = new List<TargetID>(10 + base.GetTrashMobsIDs().Count);
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
        trashIDs.Add(TargetID.ReactorActiveArkk);
        return trashIDs;
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.CMNoName;
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.Arkk,
            TargetID.Archdiviner,
            TargetID.EliteBrazenGladiator,
            TargetID.TemporalAnomalyArkk,
        ];
    }

    private static void GetMiniBossPhase(TargetID targetID, ParsedEvtcLog log, IReadOnlyList<SingleActor> targets, string phaseName, List<SubPhasePhaseData> phases, EncounterPhaseData encounterPhase)
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

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor arkk, IReadOnlyList<SingleActor> targets, IReadOnlyList<SingleActor> trashMobs, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<SubPhasePhaseData>(11);
        phases.AddRange(GetSubPhasesByInvul(log, Determined762, arkk, false, true, encounterPhase.Start, encounterPhase.End));
        for (int i = 0; i < phases.Count; i++)
        {
            phases[i].Name = "Phase " + (i + 1);
            phases[i].AddParentPhase(encounterPhase);
            phases[i].AddTarget(arkk, log);
        }
        var encounterMiniBosses = targets.Where(x => x.IsAnySpecies([TargetID.Archdiviner, TargetID.EliteBrazenGladiator]) && encounterPhase.InInterval(x.FirstAware)).ToList();
        GetMiniBossPhase(TargetID.Archdiviner, log, encounterMiniBosses, "Archdiviner", phases, encounterPhase);
        GetMiniBossPhase(TargetID.EliteBrazenGladiator, log, encounterMiniBosses, "Brazen Gladiator", phases, encounterPhase);

        var bloomPhases = new List<SubPhasePhaseData>(10);
        var encounterBlooms = trashMobs.Where(x => x.IsSpecies(TargetID.SolarBloom) && encounterPhase.InInterval(x.FirstAware)).OrderBy(x => x.FirstAware);
        foreach (var bloom in encounterBlooms)
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
        var arkk = agentData.GetStableSpeciesByID(TargetID.Arkk).FirstOrDefault() ?? throw new MissingKeyActorsException("Arkk not found");
        CombatItem? startBuffApply = combatData.FirstOrDefault(x => x.SkillID == ArkkStartBuff && x.SrcMatchesAgent(arkk) && x.IsBuffApplyEvent());
        return startBuffApply?.Time ?? GetLogOffsetBySpawn(logData, combatData, arkk);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        base.CheckSuccess(combatData, agentData, logData, playerAgents, successHandler);
        // reward or death worked
        if (successHandler.Success)
        {
            return;
        }
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Arkk)) ?? throw new MissingKeyActorsException("Arkk not found");
        // missing buff apply events fallback, some phases will be missing
        // removes should be present
        if (SetSuccessByBuffCount(combatData, logData, playerAgents, successHandler, target, Determined762, 10))
        {
            var invulsRemoveTarget = combatData.GetBuffDataByIDByDst(Determined762, target.AgentItem).OfType<BuffRemoveAllEvent>();
            if (invulsRemoveTarget.Count() == 5)
            {
                SetSuccessByCombatExit([target], combatData, logData, playerAgents, successHandler);
            }
        }
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
        var encounterPhases = log.LogData.GetEncounterPhases(log, LogID);
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

    internal static void IdentifyGadgets(AgentData agentData, List<CombatItem> combatData)
    {
        var electrocutedPositions = new[] {
            // sides
            new Vector2(-17905.2f, -15904.8f),
            new Vector2(-18931.2f, -16899.7f),
            new Vector2(-17945.8f, -17921.5f),
            new Vector2(-16921.2f, -16908.7f),
            // corners
            // new Vector2(-16914.3f, -15900.4f),
            // new Vector2(-18927.5f, -15900.4f),
            // new Vector2(-18927.5f, -17935.9f),
            // new Vector2(-16914.3f, -17935.9f),
        }; // sides is enough for replay
        foreach (var agent in agentData.GetAgentByType(AgentItem.AgentType.VolatileSpecies).Where(x => x.IsUnamedSpecies()))
        {
            switch (agent.HitboxWidth)
            {
                case 16:
                    var posEvent = combatData.FirstOrDefault(x => x.IsPosition && x.SrcMatchesAgent(agent));
                    if (posEvent != null)
                    {
                        var pos = MovementEvent.GetPoint3D(posEvent).XY();
                        if (electrocutedPositions.Any(x => (x - pos).LengthSquared() < InchDistanceThresholdSquared))
                        {
                            agent.OverrideID(TargetID.ElectrocutedAreaArkk, agentData); // animations "zeropose", "areas" (beware collisions)
                        }
                    }
                    break;
                case 284:
                    agent.OverrideID(TargetID.ReactorArkk, agentData); // animations "rebuild", "demolish"
                    break;
                case 472:
                    agent.OverrideID(TargetID.TileArkk, agentData); // animations "on", "warning", "off"
                    break;
            }
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
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
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
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
            case (int)TargetID.ReactorActiveArkk:
                {
                    // reactor beams
                    var beamEvents = GetBuffApplyRemoveSequence(log.CombatData, ReactorBeamArkk, target, true, true);
                    replay.Decorations.AddTethers(beamEvents, Colors.SkyBlue, 0.5);
                    break;
                }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
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

        // electrocuted areas
        foreach (var reactor in log.AgentData.GetStableSpeciesByID(TargetID.ElectrocutedAreaArkk))
        {
            const uint length = 2050;
            const uint width = 650;
            const float offset = 325f;
            var center = new Vector2(-17931, -16914);
            var areas = new Token("areas");
            if (reactor.TryGetCurrentPosition(log, reactor.LastAware, out var position))
            {
                var isWest = position.Value.X < center.X;
                var isNorthSouth = Math.Abs(position.Value.Y - center.Y) > 100f;
                var connector = new PositionConnector(position.Value).WithOffset((isWest ? -1f : 1f) * offset * Vector3.UnitY, true);
                var rotation = new AngleConnector(isNorthSouth ? 90f : 0f);
                foreach (var anim in log.CombatData.GetGadgetAnimationData(reactor))
                {
                    if (anim.AnimationToken == areas)
                    {
                        var lifespan = (anim.Time, anim.LoopEnd);
                        environmentDecorations.Add(new RectangleDecoration(width, length, lifespan, Colors.Orange, 0.2, connector).UsingRotationConnector(rotation));
                    }
                }
            }
        }

        // disappearing tiles
        foreach (var tile in log.AgentData.GetStableSpeciesByID(TargetID.TileArkk))
        {
            const uint size = 330;
            var warning = new Token("warning");
            var off = new Token("off");
            if (tile.TryGetCurrentPosition(log, tile.LastAware, out var position))
            {
                foreach (var anim in log.CombatData.GetGadgetAnimationData(tile))
                {
                    var lifespan = (anim.Time, anim.LoopEnd);
                    Color color;
                    if (anim.AnimationToken == warning)
                    {
                        color = Colors.Orange;
                    }
                    else if (anim.AnimationToken == off)
                    {
                        color = Colors.Red;
                    }
                    else
                    {
                        continue;
                    }
                    environmentDecorations.Add(new RectangleDecoration(size, size, lifespan, color, 0.15, new PositionConnector(position.Value)));
                }
            }
        }
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, AgentData agentData, SkillData skillData, Dictionary<long, List<AnimatedCastEvent>> animatedCastDataByID)
    {
        List<CastEvent> res = [];
        res.AddRange(ProfHelper.ComputeUnderBuffCastEvents(combatData, skillData, HypernovaLaunchSAK, HypernovaLaunchBuff));
        return res;
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }
}
