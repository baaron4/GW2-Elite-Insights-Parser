using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class BanditTrio : SalvationPass
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new PlayerDstBuffApplyMechanic(ShellShocked, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "Launched", "Shell-Shocked (Launched from pad)", "Shell-Shocked", 0),
            new PlayerDstBuffApplyMechanic(SlowBurn, new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.LightPurple), "SlowBurn.A", "Received Slow Burn", "Slow Burn Application", 0),
            new PlayerSrcBuffApplyMechanic(SapperBombDamageBuff, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Green), "Hit Cage", "Hit Cage with Sapper Bomb", "Hit Cage (Sapper Bomb)", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.Cage)),
            new MechanicGroup([
                new MechanicGroup([
                    new PlayerSrcBuffApplyMechanic(Targeted, new MechanicPlotlySetting(Symbols.StarSquare, Colors.Pink), "Targeted.B", "Applied Targeted Buff (Berg)", "Targeted Application (Berg)", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.Berg)),
                    new PlayerSrcBuffApplyMechanic(Targeted, new MechanicPlotlySetting(Symbols.StarSquare, Colors.Purple), "Targeted.A", "Applied Targeted Buff (Any)", "Targeted Application (Any)", 0),
                ]),
                new MechanicGroup([
                    new PlayerCastStartMechanic(Beehive, new MechanicPlotlySetting(Symbols.Pentagon, Colors.Yellow), "Beehive.T", "Threw Beehive", "Beehive Throw", 0),
                    new PlayerSrcHealthDamageHitMechanic(Beehive, new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.Yellow), "Beehive.H.B", "Beehive Hits (Berg)", "Beehive Hit (Berg)", 0).UsingChecker((ahde, log) => ahde.To.IsSpecies(TargetID.Berg)),
                    new PlayerSrcHealthDamageHitMechanic(Beehive, new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.LightOrange), "Beehive.H.A", "Beehive Hits (Any)", "Beehive Hit (Any)", 0),
                ]),
                new PlayerDstHealthDamageHitMechanic(OverheadSmashBerg, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Orange), "Smash", "Overhead Smash (CC Attack Berg)","CC Smash", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(Blind, new MechanicPlotlySetting(Symbols.X, Colors.White), "Blinded", "Blinded by Zane", "Blinded", 0).UsingChecker((bae, log) => bae.CreditedBy.IsSpecies(TargetID.Zane)),
                new PlayerDstHealthDamageHitMechanic(HailOfBulletsZane, new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Red), "Zane Cone", "Hail of Bullets (Zane Cone Shot)","Hail of Bullets", 0),
            ]),
            new MechanicGroup([
                new PlayerCastStartMechanic(ThrowOilKeg, new MechanicPlotlySetting(Symbols.Hourglass, Colors.LightRed), "OilKeg.T", "Threw Oil Keg", "Oil Keg Throw", 0),
                new PlayerDstBuffApplyMechanic(Burning, new MechanicPlotlySetting(Symbols.StarOpen, Colors.Red), "Burning", "Burned by Narella", "Burning", 0).UsingChecker((bae, log) => bae.CreditedBy.IsSpecies(TargetID.Narella)),
                new PlayerDstHealthDamageHitMechanic(FieryVortexNarella, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Tornado", "Fiery Vortex (Tornado Narella)","Tornado (Narella)", 250),
                new PlayerDstHealthDamageHitMechanic(FlakShotNarella, new MechanicPlotlySetting(Symbols.Diamond, Colors.LightRed), "Flak", "Flak Shot (Narella)", "Flak Shot Hit", 0),
            ]),
        ]);
    public BanditTrio(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "trio";
        GenericFallBackMethod = FallBackMethod.None;
        ChestID = ChestID.ChestOfPrisonCamp;
        Icon = EncounterIconBanditTrio;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000002;
    }

    protected override IReadOnlyList<TargetID> GetSuccessCheckIDs()
    {
        return [
            TargetID.Berg,
            TargetID.Zane,
            TargetID.Narella
        ];
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.Berg,
            TargetID.Zane,
            TargetID.Narella
        ];
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        return [ TargetID.Cage ];
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (1000, 913),
                        (-2900, -12251, 2561, -7265));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayBanditTrio, crMap);
        return crMap;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericLogOffset(logData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            startToUse = long.MaxValue;
            if (!agentData.TryGetFirstAgentItem(TargetID.Berg, out var berg))
            {
                throw new MissingKeyActorsException("Berg not found");
            }
            
            startToUse = Math.Min(berg.FirstAware, startToUse);
            if (!agentData.TryGetFirstAgentItem(TargetID.Zane, out var zane))
            {
                throw new MissingKeyActorsException("Zane not found");
            }
            
            startToUse = Math.Min(zane.FirstAware, startToUse);
            if (!agentData.TryGetFirstAgentItem(TargetID.Narella, out var narella))
            {
                throw new MissingKeyActorsException("Narella not found");
            }
            
            startToUse = Math.Min(narella.FirstAware, startToUse);
            // Thrash mob start check
            var boxStart = new Vector2(-2200, -11300);
            var boxEnd = new Vector2(1000, -7200);
            HashSet<TargetID> trashMobsToCheck =
            [
                TargetID.BanditAssassin,
                TargetID.BanditAssassin2,
                TargetID.BanditSapperTrio,
                TargetID.BanditDeathsayer,
                TargetID.BanditDeathsayer2,
                TargetID.BanditBrawler,
                TargetID.BanditBrawler2,
                TargetID.BanditBattlemage,
                TargetID.BanditBattlemage2,
                TargetID.BanditCleric,
                TargetID.BanditCleric2,
                TargetID.BanditBombardier,
                TargetID.BanditSniper,
            ];
            var banditPositions = combatData.Where(x => x.IsStateChange == StateChange.Position && agentData.GetAgent(x.SrcAgent, x.Time).IsAnySpecies(trashMobsToCheck))
                .Select(x => new PositionEvent(x, agentData));
            var banditsInBox = banditPositions.Where(x => x.Time < startToUse + 10000 && x.GetPointXY().IsInBoundingBox(boxStart, boxEnd))
                .Select(x => x.Src)
                .ToHashSet();
            if (banditsInBox.Count > 0)
            {
                startToUse = Math.Min(banditsInBox.Min(x => x.FirstAware), startToUse);
            }
        }
        return startToUse;
    }

    internal static void FindCageAndBombs(AgentData agentData, List<CombatItem> combatData)
    {
        // Cage
        AgentItem? cage = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 224100 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 238 && x.HitboxHeight == 300).FirstOrDefault();
        if (cage != null)
        {
            cage.OverrideType(AgentItem.AgentType.NPC, agentData);
            cage.OverrideID(TargetID.Cage, agentData);
        }
        // Bombs
        var bombs = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 0 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 240);
        foreach (AgentItem bomb in bombs)
        {
            bomb.OverrideType(AgentItem.AgentType.NPC, agentData);
            bomb.OverrideID(TargetID.Bombs, agentData);
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindCageAndBombs(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override LogData.LogStartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (TargetHPPercentUnderThreshold(TargetID.Berg, logData.LogStart, combatData, Targets))
        {
            return LogData.LogStartStatus.Late;
        }
        if (agentData.TryGetFirstAgentItem(TargetID.Berg, out var berg) && combatData.GetLogNPCUpdateEvents().Count > 0)
        {
            var firstMovement = combatData.GetMovementData(berg).First(x => x.Time > berg.FirstAware + MinimumInCombatDuration);
            if (firstMovement != null && firstMovement.Time < 120000)
            {
                return LogData.LogStartStatus.Late;
            }
        }
        return LogData.LogStartStatus.Normal;
    }

    private static void SetPhasePerTarget(SingleActor target, List<PhaseData> phases, PhaseData encounterPhase, ParsedEvtcLog log)
    {
        EnterCombatEvent? phaseStart = log.CombatData.GetEnterCombatEvents(target.AgentItem).LastOrDefault(x => x.Time >= encounterPhase.Start);
        if (phaseStart != null)
        {
            long start = phaseStart.Time;
            DeadEvent? phaseEnd = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
            long end = encounterPhase.End;
            if (phaseEnd != null)
            {
                end = phaseEnd.Time;
            }
            var phase = new SubPhasePhaseData(start, Math.Min(end, encounterPhase.End));
            phase.AddParentPhase(encounterPhase);
            phase.AddTarget(target, log);
            phase.Name = target.ID switch
            {
                (int)TargetID.Narella => "Narella",
                (int)TargetID.Berg => "Berg",
                (int)TargetID.Zane => "Zane",
                _ => throw new MissingKeyActorsException("Unknown target in Bandit Trio"),
            };
            phases.Add(phase);
        }
    }

    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor berg, SingleActor zane, SingleActor narella, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<PhaseData>(3);
        SetPhasePerTarget(berg, phases, encounterPhase, log);
        SetPhasePerTarget(zane, phases, encounterPhase, log);
        SetPhasePerTarget(narella, phases, encounterPhase, log);
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor berg = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Berg)) ?? throw new MissingKeyActorsException("Berg not found");
        SingleActor zane = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Zane)) ?? throw new MissingKeyActorsException("Zane not found");
        SingleActor narella = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Narella)) ?? throw new MissingKeyActorsException("Narella not found");
        phases[0].AddTargets(Targets, log);
        phases.AddRange(ComputePhases(log, berg, zane, narella, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.BanditSaboteur,
            TargetID.Warg,
            TargetID.VeteranTorturedWarg,
            TargetID.BanditAssassin,
            TargetID.BanditAssassin2,
            TargetID.BanditSapperTrio,
            TargetID.BanditDeathsayer,
            TargetID.BanditDeathsayer2,
            TargetID.BanditBrawler,
            TargetID.BanditBrawler2,
            TargetID.BanditBattlemage,
            TargetID.BanditBattlemage2,
            TargetID.BanditCleric,
            TargetID.BanditCleric2,
            TargetID.BanditBombardier,
            TargetID.BanditSniper,
            TargetID.NarellaTornado,
            TargetID.OilSlick,
            TargetID.Prisoner1,
            TargetID.Prisoner2,
            TargetID.InsectSwarm,
            TargetID.Bombs,
        ];
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Bandit Trio";
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        long castDuration;
        long growing;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Berg:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Overhead Smash - Cone knock
                        case OverheadSmashBerg:
                            castDuration = 2250;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, growing);
                            if (target.TryGetCurrentFacingDirection(log, lifespan.start + 600, out var facing, lifespan.end))
                            {
                                var cone = (PieDecoration)new PieDecoration(550, 80, lifespan, Colors.Orange, 0.2, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facing));
                                replay.Decorations.AddWithGrowing(cone, growing);
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.Zane:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Hail of Bullets - 3 Cones attack
                        case HailOfBulletsZane:
                            (long start, long end) firstCone = (cast.Time, cast.Time + 400);
                            (long start, long end) secondCone = (cast.Time + 800, cast.Time + 800 + 400);
                            (long start, long end) thirdCone = (cast.Time + 1600, cast.Time + 1600 + 400);
                            uint radius = 1500;
                            if (target.TryGetCurrentFacingDirection(log, firstCone.start, out var facing))
                            {
                                var connector = new AgentConnector(target);
                                var rotationConnector = new AngleConnector(facing);
                                replay.Decorations.Add(new PieDecoration(radius, 28, firstCone, Colors.LightOrange, 0.2, connector).UsingRotationConnector(rotationConnector));
                                replay.Decorations.Add(new PieDecoration(radius, 54, secondCone, Colors.LightOrange, 0.2, connector).UsingRotationConnector(rotationConnector));
                                replay.Decorations.Add(new PieDecoration(radius, 81, thirdCone, Colors.LightOrange, 0.2, connector).UsingRotationConnector(rotationConnector));
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.Narella:
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(player, log, replay);
        }
        // Sapper bombs
        var sapperBombs = player.GetBuffStatus(log, SapperBombBuff).Where(x => x.Value > 0);
        foreach (var seg in sapperBombs)
        {
            var circle = new CircleDecoration(180, seg, Colors.Lime, 0.5, new AgentConnector(player));
            replay.Decorations.AddWithFilledWithGrowing(circle.UsingFilled(false), true, seg.Start + 5000);
            replay.Decorations.AddOverheadIcon(seg, player, ParserIcons.BombOverhead);
        }
    }
    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
    }
    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
        if (log.CombatData.GetBuffData(EnvironmentallyFriendly).Any())
        {
            var encounterPhases = log.LogData.GetPhases(log).OfType<EncounterPhaseData>().Where(x => x.LogID == LogID);
            foreach (var encounterPhase in encounterPhases)
            {
                if (encounterPhase.Success)
                {
                    instanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, encounterPhase, EnvironmentallyFriendly));
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
