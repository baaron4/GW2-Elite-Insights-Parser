using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class StatueOfDarkness : HallOfChains
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([

            new PlayerDstBuffApplyMechanic(Fear, new MechanicPlotlySetting(Symbols.StarSquare,Colors.Black), "Feared", "Feared by Eye Teleport Skill","Feared", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(LightCarrier, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Light Orb", "Light Carrier (picked up a light orb)","Picked up orb", 0),
                new PlayerCastStartMechanic(Flare, new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "Detonate", "Flare (detonate light orb to incapacitate eye)","Detonate orb", 0)
                    .UsingChecker((evt, log) => !evt.IsInterrupted),
            ]),
            new PlayerDstHealthDamageHitMechanic(PiercingShadow, new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Blue), "Spin", "Piercing Shadow (damaging spin to all players in sight)","Eye Spin", 0),
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
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000005;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayStatueOfDarkness,
                        (809, 1000),
                        (11664, -2108, 16724, 4152)/*,
                        (-21504, -12288, 24576, 12288),
                        (19072, 15484, 20992, 16508)*/);
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.LightThieves,
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

    protected override List<TargetID> GetSuccessCheckIDs()
    {
        return
        [
            TargetID.EyeOfFate,
            TargetID.EyeOfJudgement
        ];
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        if (TargetHPPercentUnderThreshold(TargetID.EyeOfJudgement, fightData.FightStart, combatData, Targets) ||
            TargetHPPercentUnderThreshold(TargetID.EyeOfFate, fightData.FightStart, combatData, Targets))
        {
            return FightData.EncounterStartStatus.Late;
        }
        return FightData.EncounterStartStatus.Normal;
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericFightOffset(fightData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            IReadOnlyList<AgentItem> lightThieves = agentData.GetNPCsByID(TargetID.LightThieves);
            if (lightThieves.Any())
            {
                startToUse = lightThieves.Min(x => x.FirstAware);
            }
        }
        return startToUse;
    }

    private static List<PhaseData> GetSubPhases(SingleActor eye, ParsedEvtcLog log)
    {
        var res = new List<PhaseData>();
        BuffRemoveAllEvent? det762Loss = log.CombatData.GetBuffDataByIDByDst(Determined762, eye.AgentItem).OfType<BuffRemoveAllEvent>().FirstOrDefault();
        if (det762Loss != null)
        {
            int count = 0;
            long start = det762Loss.Time;
            var det895s = GetFilteredList(log.CombatData, Determined895, eye, true, true);
            foreach (BuffEvent abe in det895s)
            {
                if (abe is BuffApplyEvent)
                {
                    var phase = new PhaseData(start, Math.Min(abe.Time, log.FightData.FightEnd))
                    {
                        Name = eye.Character + " " + (++count)
                    };
                    phase.AddTarget(eye, log);
                    res.Add(phase);
                }
                else
                {
                    start = Math.Min(abe.Time, log.FightData.FightEnd);
                }
            }

            if (start < log.FightData.FightEnd)
            {
                var phase = new PhaseData(start, log.FightData.FightEnd)
                {
                    Name = eye.Character + " " + (++count)
                };
                phase.AddTarget(eye, log);
                res.Add(phase);
            }
        }
        return res;
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
        phases.AddRange(GetSubPhases(eyeFate, log));
        phases.AddRange(GetSubPhases(eyeJudgement, log));
        return phases;
    }

    internal static bool HasIntersectingLastGrasps(CombatData combatData, SingleActor eyeFate, SingleActor eyeJudgement, out long intersectTime)
    {
        var lastGraspsJudgement = GetFilteredList(combatData, LastGraspJudgment, eyeJudgement, true, true).ToList(); //TODO(Rennorb) @perf
        var lastGraspsJudgementSegments = new List<Segment>(lastGraspsJudgement.Count / 2);
        for (int i = 0; i < lastGraspsJudgement.Count; i += 2)
        {
            lastGraspsJudgementSegments.Add(new Segment(lastGraspsJudgement[i].Time, lastGraspsJudgement[i + 1].Time, 1));
        }
        var lastGraspsFate = GetFilteredList(combatData, LastGraspFate, eyeFate, true, true).ToList(); //TODO(Rennorb) @perf
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

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
        if (!fightData.Success)
        {
            SingleActor? eyeFate = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EyeOfFate));
            SingleActor? eyeJudgement = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EyeOfJudgement));
            if (eyeJudgement == null || eyeFate == null)
            {
                throw new MissingKeyActorsException("Eyes not found");
            }
            if (HasIntersectingLastGrasps(combatData, eyeFate, eyeJudgement, out var intersectTime))
            {
                fightData.SetSuccess(true, intersectTime);
            }
        }
    }
    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.EyeOfFate, 0 },
            {TargetID.EyeOfJudgement, 0 },
        };
    }
    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Statue of Darkness";
    }
}
