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
    internal class Freezie : FestivalStrikeMissionLogic
    {
        public Freezie(int triggerID) : base(triggerID)
        {
            Extension = "freezie";
            Icon = EncounterIconFreezie;
            EncounterID |= 0x000001;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Freezie));
            AbstractSingleActor heartTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TrashID.FreeziesFrozenHeart));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Freezie not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined895, mainTarget, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 1)
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mainTarget);
                }
                else
                {
                    phase.Name = "Heal " + (i) / 2;
                    phase.AddTarget(heartTarget);
                }
            }
            return phases;
        }
        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            RewardEvent reward = combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == 55821);
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Freezie,
                (int)ArcDPSEnums.TrashID.FreeziesFrozenHeart
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Freezie,
                (int)ArcDPSEnums.TrashID.FreeziesFrozenHeart
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.IceStormer,
                ArcDPSEnums.TrashID.IceSpiker,
                ArcDPSEnums.TrashID.IcyProtector
            };
        }
    }
}
