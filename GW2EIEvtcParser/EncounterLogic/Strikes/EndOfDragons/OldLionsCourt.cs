using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

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
            Icon = "https://i.imgur.com/UZmW8Sd.png";
            Extension = "lioncourt";
            EncounterCategoryInformation.InSubCategoryOrder = 4;
            EncounterID |= 0x000005;
        }

        /*protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/Qhnhrvp.png",
                            (838, 639),
                            (1165, 540, 4194, 2850));
        }*/
        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.PrototypeVermilion,
                (int)ArcDPSEnums.TargetID.PrototypeIndigo,
                (int)ArcDPSEnums.TargetID.PrototypeArsenite,
            };
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.PrototypeVermilion,
                (int)ArcDPSEnums.TargetID.PrototypeIndigo,
                (int)ArcDPSEnums.TargetID.PrototypeArsenite,
            };
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
            };
        }

        private List<PhaseData> GetSubPhases(AbstractSingleActor target, ParsedEvtcLog log, string phaseName)
        {
            var phaseNames = new[]
            {
                phaseName + " 100% - 80%",
                phaseName + " 80% - 40%",
                phaseName + " 40% - 10%",
                phaseName + " 10% - 0%",
            };
            DeadEvent dead = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
            List<PhaseData> subPhases = GetPhasesByInvul(log, new[] { LeyWovenShielding, MalfunctioningLeyWovenShielding }, target, false, true, log.FightData.FightStart, dead != null ? dead.Time : log.FightData.FightEnd);
            if (subPhases.Count > 4)
            {
                return new List<PhaseData>();
            }
            for (int i = 0; i < subPhases.Count; i++)
            {
                subPhases[i].Name = phaseNames[i];
                subPhases[i].AddTarget(target);
            }
            return subPhases;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor vermilion = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PrototypeVermilion);
            var canComputePhases = vermilion != null && vermilion.HasBuff(log, LeyWovenShielding, 500); // check that vermilion is present and starts shielded, otherwise clearly incomplete log
            if (vermilion != null)
            {
                phases[0].AddTarget(vermilion);
                if (canComputePhases)
                {
                    phases.AddRange(GetSubPhases(vermilion, log, "Vermilion"));
                }
            } 
            AbstractSingleActor indigo = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PrototypeIndigo);
            if (indigo != null)
            {
                phases[0].AddTarget(indigo);
                if (canComputePhases)
                {
                    phases.AddRange(GetSubPhases(indigo, log, "Indigo"));
                }
            }
            AbstractSingleActor arsenite = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PrototypeArsenite);
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
    }
}
