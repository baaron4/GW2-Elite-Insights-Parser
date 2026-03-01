using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class StatueOfDarkness : HallOfChains
{
    internal readonly MechanicGroup Mechanics = new([

            new PlayerDstBuffApplyMechanic(Fear, new MechanicPlotlySetting(Symbols.StarSquare,Colors.Black), "Feared", "Feared by Eye Teleport Skill","Feared", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(LightCarrier, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Light Orb", "Light Carrier (picked up a light orb)","Picked up orb", 0),
                new PlayerCastStartMechanic(Flare, new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "Detonate", "Flare (detonate light orb to incapacitate eye)","Detonate orb", 0)
                    .UsingChecker((evt, log) => !evt.IsInterrupted),
            ]),
            new PlayerDstHealthDamageHitMechanic(PiercingShadow, new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Blue), "Spin.SoD", "Piercing Shadow (damaging spin to all players in sight)","Eye Spin", 0),
            new PlayerDstHealthDamageHitMechanic(DeepAbyss, new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Red), "Beam", "Deep Abyss (ticking eye beam)","Eye Beam", 0),
            new MechanicGroup([
                new PlayerSrcBuffApplyMechanic([Daze, Fear, Knockdown], new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Red), "Hard CC Fate", "Applied Daze/Fear/Knockdown on Eye of Fate","CC Fate", 50)
                    .UsingChecker((ba, log) => ba.To.IsSpecies(TargetID.EyeOfFate)),
                new PlayerSrcBuffApplyMechanic([Daze, Fear, Knockdown], new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Hard CC Judge", "Applied Daze/Fear/Knockdown on Eye of Judgement","CC Judge", 50)
                    .UsingChecker((ba, log) => ba.To.IsSpecies(TargetID.EyeOfJudgement)),
            ]),
        //47857 <- teleport + fear skill? 
        ]);
    public StatueOfDarkness(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "eyes";
        Icon = EncounterIconStatueOfDarkness;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000005;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (809, 1000),
                        (11664, -2108, 16724, 4152));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayStatueOfDarkness, crMap);
        return crMap;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.LightThief,
            TargetID.MazeMinotaur,
        ];
    }


    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.EyeOfFate,
            TargetID.EyeOfJudgement
        ];
    }

    protected override IReadOnlyList<TargetID> GetSuccessCheckIDs()
    {
        return
        [
            TargetID.EyeOfFate,
            TargetID.EyeOfJudgement
        ];
    }

    internal override LogData.StartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (TargetHPPercentUnderThreshold(TargetID.EyeOfJudgement, logData.LogStart, combatData, Targets) ||
            TargetHPPercentUnderThreshold(TargetID.EyeOfFate, logData.LogStart, combatData, Targets))
        {
            return LogData.StartStatus.Late;
        }
        return LogData.StartStatus.Normal;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericLogOffset(logData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            IReadOnlyList<AgentItem> lightThieves = agentData.GetNPCsByID(TargetID.LightThief);
            if (lightThieves.Any())
            {
                startToUse = lightThieves.Min(x => x.FirstAware);
            }
        }
        return startToUse;
    }

    private static List<SubPhasePhaseData> GetSubPhases(SingleActor eye, ParsedEvtcLog log, EncounterPhaseData encounterPhase)
    {
        var res = new List<SubPhasePhaseData>();
        BuffRemoveAllEvent? det762Loss = log.CombatData.GetBuffDataByIDByDst(Determined762, eye.AgentItem).OfType<BuffRemoveAllEvent>().FirstOrDefault();
        if (det762Loss != null)
        {
            int count = 0;
            long det762LossTime = det762Loss.Time;
            var det895s = eye.GetBuffStatus(log, Determined895);
            foreach (var abe in det895s)
            {
                if (abe.Value == 0)
                {
                    string name = eye.IsSpecies(TargetID.EyeOfJudgement) ? "Eye of Judgement" : "Eye of Fate";
                    var phase = new SubPhasePhaseData(Math.Max(det762LossTime, abe.Start), Math.Min(abe.End, encounterPhase.End))
                    {
                        Name = name + " " + (++count)
                    };
                    phase.AddParentPhase(encounterPhase);
                    phase.AddTarget(eye, log);
                    res.Add(phase);
                }
            }
        }
        return res;
    }
    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor eyeFate, SingleActor eyeJudgement, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<SubPhasePhaseData>(4);
        phases.AddRange(GetSubPhases(eyeFate, log, encounterPhase));
        phases.AddRange(GetSubPhases(eyeJudgement, log, encounterPhase));
        return phases;
    }
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor? eyeFate = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EyeOfFate));
        SingleActor? eyeJudgement = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EyeOfJudgement));
        if (eyeJudgement == null || eyeFate == null)
        {
            throw new MissingKeyActorsException("Eyes not found");
        }
        phases[0].AddTarget(eyeJudgement, log);
        phases[0].AddTarget(eyeFate, log);
        phases.AddRange(ComputePhases(log, eyeFate, eyeJudgement, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal static bool HasIntersectingLastGrasps(CombatData combatData, SingleActor eyeFate, SingleActor eyeJudgement, out long intersectTime)
    {
        var lastGraspsJudgement = GetBuffApplyRemoveSequence(combatData, LastGraspJudgment, eyeJudgement, true, true).ToList(); //TODO_PERF(Rennorb)
        var lastGraspsJudgementSegments = new List<Segment>(lastGraspsJudgement.Count / 2);
        for (int i = 0; i < lastGraspsJudgement.Count; i += 2)
        {
            lastGraspsJudgementSegments.Add(new Segment(lastGraspsJudgement[i].Time, lastGraspsJudgement[i + 1].Time, 1));
        }
        var lastGraspsFate = GetBuffApplyRemoveSequence(combatData, LastGraspFate, eyeFate, true, true).ToList(); //TODO_PERF(Rennorb)
        var lastGraspsFateSegments = new List<Segment>(lastGraspsFate.Count / 2);
        for (int i = 0; i < lastGraspsFate.Count; i += 2)
        {
            lastGraspsFateSegments.Add(new Segment(lastGraspsFate[i].Time, lastGraspsFate[i + 1].Time, 1));
        }
        //
        if (lastGraspsJudgementSegments.LastOrNull() is Segment lastJudge && lastGraspsFateSegments.LastOrNull() is Segment lastFate)
        {
            if (lastFate.Intersects(lastJudge))
            {
                intersectTime = Math.Max(lastJudge.Start, lastFate.Start);
                return true;
            }
        }
        intersectTime = -1;
        return false;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        NoBouncyChestGenericCheckSucess(combatData, agentData, logData, playerAgents, successHandler);
        if (!successHandler.Success)
        {
            SingleActor? eyeFate = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EyeOfFate));
            SingleActor? eyeJudgement = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EyeOfJudgement));
            if (eyeJudgement == null || eyeFate == null)
            {
                throw new MissingKeyActorsException("Eyes not found");
            }
            if (HasIntersectingLastGrasps(combatData, eyeFate, eyeJudgement, out var intersectTime))
            {
                successHandler.SetSuccess(true, intersectTime);
            }
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors (target, log, replay);
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
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }
    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>
        {
            {TargetID.EyeOfFate, 0 },
            {TargetID.EyeOfJudgement, 0 },
        };
    }
    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Statue of Darkness";
    }
}
