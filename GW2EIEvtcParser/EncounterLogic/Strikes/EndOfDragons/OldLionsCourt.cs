using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class OldLionsCourt : EndOfDragonsStrike
    {
        public OldLionsCourt(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Icon = "https://i.imgur.com/Q2g9aLD.png";
            Extension = "lioncourt";
            EncounterCategoryInformation.InSubCategoryOrder = 4;
            EncounterID |= 0x000005;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/s1R1CRq.png",
                            (1008, 1008),
                            (-1420, 3010, 1580, 6010));
        }
        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.PrototypeVermilion,
                (int)ArcDPSEnums.TargetID.PrototypeIndigo,
                (int)ArcDPSEnums.TargetID.PrototypeArsenite,
                (int)ArcDPSEnums.TargetID.PrototypeVermilionCM,
                (int)ArcDPSEnums.TargetID.PrototypeIndigoCM,
                (int)ArcDPSEnums.TargetID.PrototypeArseniteCM,
            };
        }

        protected override List<int> GetSuccessCheckIDs()
        {
            return new List<int>
            {
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                List<int> idsToCheck;
                if (fightData.IsCM)
                {
                    idsToCheck = new List<int>
                    {
                        (int)ArcDPSEnums.TargetID.PrototypeVermilionCM,
                        (int)ArcDPSEnums.TargetID.PrototypeIndigoCM,
                        (int)ArcDPSEnums.TargetID.PrototypeArseniteCM,
                    };
                } 
                else
                {
                    idsToCheck = new List<int>
                    {
                        (int)ArcDPSEnums.TargetID.PrototypeVermilion,
                        (int)ArcDPSEnums.TargetID.PrototypeIndigo,
                        (int)ArcDPSEnums.TargetID.PrototypeArsenite,
                    };
                }
                SetSuccessByDeath(Targets, combatData, fightData, playerAgents, true, idsToCheck);
            }
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Old Lion's Court";
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.PrototypeVermilion,
                (int)ArcDPSEnums.TargetID.PrototypeIndigo,
                (int)ArcDPSEnums.TargetID.PrototypeArsenite,
                (int)ArcDPSEnums.TargetID.PrototypeVermilionCM,
                (int)ArcDPSEnums.TargetID.PrototypeIndigoCM,
                (int)ArcDPSEnums.TargetID.PrototypeArseniteCM,
            };
        }

        private AbstractSingleActor Vermillion()
        {
            return Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PrototypeVermilionCM) ?? Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PrototypeVermilion);
        }
        private AbstractSingleActor Indigo()
        {
            return Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PrototypeIndigoCM) ?? Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PrototypeIndigo);
        }
        private AbstractSingleActor Arsenite()
        {
            return Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PrototypeArseniteCM) ?? Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PrototypeArsenite);
        }

        private static List<PhaseData> GetSubPhases(AbstractSingleActor target, ParsedEvtcLog log, string phaseName)
        {
            DeadEvent dead = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
            long end = log.FightData.FightEnd;
            long start = log.FightData.FightStart;
            if (dead != null && dead.Time < end)
            {
                end = dead.Time;
            } 
            List<PhaseData> subPhases = GetPhasesByInvul(log, new[] { LeyWovenShielding, MalfunctioningLeyWovenShielding }, target, false, true, start, end);
            string[] phaseNames;
            if (log.FightData.IsCM)
            {
                if (subPhases.Count > 3)
                {
                    return new List<PhaseData>();
                }
                phaseNames = new[]
                {
                    phaseName + " 100% - 60%",
                    phaseName + " 60% - 20%",
                    phaseName + " 20% - 0%",
                };
            }
            else
            {
                if (subPhases.Count > 4)
                {
                    return new List<PhaseData>();
                }
                phaseNames = new[]
                {
                    phaseName + " 100% - 80%",
                    phaseName + " 80% - 40%",
                    phaseName + " 40% - 10%",
                    phaseName + " 10% - 0%",
                };
            }
            for (int i = 0; i < subPhases.Count; i++)
            {
                subPhases[i].Name = phaseNames[i];
                subPhases[i].AddTarget(target);
            }
            return subPhases;
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = base.GetFightOffset(fightData, agentData, combatData);
            AgentItem vermillion = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.PrototypeVermilionCM).FirstOrDefault() ?? agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.PrototypeVermilion).FirstOrDefault();
            if (vermillion != null)
            {
                CombatItem breakbarStateActive = combatData.FirstOrDefault(x => x.SrcMatchesAgent(vermillion) && x.IsStateChange == ArcDPSEnums.StateChange.BreakbarState && x.Value == 0);
                if (breakbarStateActive != null)
                {
                    startToUse = breakbarStateActive.Time;
                }
            }
            return startToUse;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor vermilion = Vermillion();
            var canComputePhases = vermilion != null && vermilion.HasBuff(log, LeyWovenShielding, 500); // check that vermilion is present and starts shielded, otherwise clearly incomplete log
            if (vermilion != null)
            {
                phases[0].AddTarget(vermilion);
                if (canComputePhases)
                {
                    phases.AddRange(GetSubPhases(vermilion, log, "Vermilion"));
                }
            }
            AbstractSingleActor indigo = Indigo();
            if (indigo != null)
            {
                phases[0].AddTarget(indigo);
                if (canComputePhases)
                {
                    phases.AddRange(GetSubPhases(indigo, log, "Indigo"));
                }
            }
            AbstractSingleActor arsenite = Arsenite();
            if (arsenite != null)
            {
                phases[0].AddTarget(arsenite);
                if (canComputePhases)
                {
                    phases.AddRange(GetSubPhases(arsenite, log, "Arsenite"));
                }
            }
            return phases;
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Vermillion() ?? Indigo() ?? Arsenite();
            if (target == null)
            {
                throw new MissingKeyActorsException("Main target not found");
            }
            return target.GetHealth(combatData) > 20e6 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }
    }
}
