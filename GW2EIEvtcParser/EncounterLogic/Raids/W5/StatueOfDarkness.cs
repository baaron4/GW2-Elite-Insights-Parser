using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class StatueOfDarkness : HallOfChains
    {
        // TODO - add CR icons and some mechanics
        public StatueOfDarkness(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.StarSquare,Colors.Black), "Feared","Feared by Eye Teleport Skill", "Feared",0),
            new PlayerDstBuffApplyMechanic(LightCarrier, "Light Carrier", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Light Orb","Light Carrier (picked up a light orb)", "Picked up orb",0),
            new PlayerCastStartMechanic(Flare, "Flare", new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "Detonate","Flare (detonate light orb to incapacitate eye)", "Detonate orb",0).UsingChecker( (evt, log) => evt.Status != AbstractCastEvent.AnimationStatus.Interrupted),
            new PlayerDstHitMechanic(PiercingShadow, "Piercing Shadow", new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Blue), "Spin","Piercing Shadow (damaging spin to all players in sight)", "Eye Spin",0),
            new PlayerDstHitMechanic(DeepAbyss, "Deep Abyss", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Red), "Beam","Deep Abyss (ticking eye beam)", "Eye Beam",0),
            new PlayerSrcBuffApplyMechanic(new long[] { Daze, Fear, Knockdown }, "Hard CC Eye of Fate", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Red), "Hard CC Fate","Applied Daze/Fear/Knockdown on Eye of Fate", "CC Fate",50).UsingChecker((ba, log) => ba.To.IsSpecies(ArcDPSEnums.TargetID.EyeOfFate)),
            new PlayerSrcBuffApplyMechanic(new long[] { Daze, Fear, Knockdown }, "Hard CC Eye of Judge", new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Hard CC Judge","Applied Daze/Fear/Knockdown on Eye of Judgement", "CC Judge",50).UsingChecker((ba, log) => ba.To.IsSpecies(ArcDPSEnums.TargetID.EyeOfJudgement)),
            //47857 <- teleport + fear skill? 
            }
            );
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

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.LightThieves,
                ArcDPSEnums.TrashID.MazeMinotaur,
            };
        }


        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.EyeOfFate,
                (int)ArcDPSEnums.TargetID.EyeOfJudgement
            };
        }

        protected override List<int> GetSuccessCheckIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.EyeOfFate,
                (int)ArcDPSEnums.TargetID.EyeOfJudgement
            };
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.EyeOfFate,
                (int)ArcDPSEnums.TargetID.EyeOfJudgement
            };
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                IReadOnlyList<AgentItem> lightThieves = agentData.GetNPCsByID(ArcDPSEnums.TrashID.LightThieves);
                if (lightThieves.Any())
                {
                    startToUse = lightThieves.Min(x => x.FirstAware);
                }
            }
            return startToUse;
        }

        private static List<PhaseData> GetSubPhases(AbstractSingleActor eye, ParsedEvtcLog log)
        {
            var res = new List<PhaseData>();
            BuffRemoveAllEvent det762Loss = log.CombatData.GetBuffData(Determined762).OfType<BuffRemoveAllEvent>().Where(x => x.To == eye.AgentItem).FirstOrDefault();
            if (det762Loss != null)
            {
                int count = 0;
                long start = det762Loss.Time;
                List<AbstractBuffEvent> det895s = GetFilteredList(log.CombatData, Determined895, eye, true, true);
                foreach (AbstractBuffEvent abe in det895s)
                {
                    if (abe is BuffApplyEvent)
                    {
                        var phase = new PhaseData(start, Math.Min(abe.Time, log.FightData.FightEnd))
                        {
                            Name = eye.Character + " " + (++count)
                        };
                        phase.AddTarget(eye);
                        res.Add(phase);
                    } else
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
                    phase.AddTarget(eye);
                    res.Add(phase);
                }
            }
            return res;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor eyeFate = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.EyeOfFate));
            AbstractSingleActor eyeJudgement = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.EyeOfJudgement));
            if (eyeJudgement == null || eyeFate == null)
            {
                throw new MissingKeyActorsException("Eyes not found");
            }
            phases[0].AddTarget(eyeJudgement);
            phases[0].AddTarget(eyeFate);
            phases.AddRange(GetSubPhases(eyeFate, log));
            phases.AddRange(GetSubPhases(eyeJudgement, log));
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor eyeFate = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.EyeOfFate));
                AbstractSingleActor eyeJudgement = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.EyeOfJudgement));
                if (eyeJudgement == null || eyeFate == null)
                {
                    throw new MissingKeyActorsException("Eyes not found");
                }
                //
                List<AbstractBuffEvent> lastGraspsJudgement = GetFilteredList(combatData, LastGraspJudgment, eyeJudgement, true, true);
                var lastGraspsJudgementSegments = new List<Segment>();
                for (int i = 0; i < lastGraspsJudgement.Count; i += 2)
                {
                    lastGraspsJudgementSegments.Add(new Segment(lastGraspsJudgement[i].Time, lastGraspsJudgement[i + 1].Time, 1));
                }
                List<AbstractBuffEvent> lastGraspsFate = GetFilteredList(combatData, LastGraspFate, eyeFate, true, true);
                var lastGraspsFateSegments = new List<Segment>();
                for (int i = 0; i < lastGraspsFate.Count; i += 2)
                {
                    lastGraspsFateSegments.Add(new Segment(lastGraspsFate[i].Time, lastGraspsFate[i + 1].Time, 1));
                }
                //
                Segment lastJudge = lastGraspsJudgementSegments.LastOrDefault();
                Segment lastFate = lastGraspsFateSegments.LastOrDefault();
                if (lastFate == null || lastJudge == null)
                {
                    return;
                }
                if (lastFate.IntersectSegment(lastJudge))
                {
                    fightData.SetSuccess(true, Math.Max(lastJudge.Start, lastFate.Start));
                }
            }
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Statue of Darkness";
        }
    }
}
