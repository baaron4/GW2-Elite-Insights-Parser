using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class LonelyTowerCerusAndDeimos : LonelyTower
    {
        public LonelyTowerCerusAndDeimos(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            });
            Extension = "cerdei";
            //Icon = EncounterIconMAMA;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Cerus and Deimos";
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = base.GetFightOffset(evtcVersion, fightData, agentData, combatData);
            if (evtcVersion >= ArcDPSBuilds.NewLogStart)
            {
                AgentItem cerus = agentData.GetNPCsByID(TargetID.CerusLonelyTower).FirstOrDefault();
                AgentItem deimos = agentData.GetNPCsByID(TargetID.DeimosLonelyTower).FirstOrDefault();
                if (cerus != null && deimos != null)
                {
                    CombatItem initialDamageToPlayers = combatData.Where(x => x.IsDamagingDamage() && agentData.GetAgent(x.DstAgent, x.Time).IsPlayer && (
                          agentData.GetAgent(x.SrcAgent, x.Time) == cerus || agentData.GetAgent(x.SrcAgent, x.Time) == deimos)).FirstOrDefault();
                    long initialDamageTimeToTargets = Math.Min(GetFirstDamageEventTime(fightData, agentData, combatData, cerus),GetFirstDamageEventTime(fightData, agentData, combatData, deimos));
                    if (initialDamageToPlayers != null)
                    {
                        return Math.Min(initialDamageToPlayers.Time, initialDamageTimeToTargets);
                    }
                    return initialDamageTimeToTargets;
                }
                throw new MissingKeyActorsException("Cerus or Deimos not found");
            }
            return startToUse;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower));
            if (deimos == null)
            {
                throw new MissingKeyActorsException("Deimos not found");
            }
            AbstractSingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower));
            if (cerus == null)
            {
                throw new MissingKeyActorsException("Cerus not found");
            }
            BuffApplyEvent determinedApplyCerus = combatData.GetBuffDataByIDByDst(Determined762, cerus.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
            BuffApplyEvent determinedApplyDeimos = combatData.GetBuffDataByIDByDst(Determined762, deimos.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
            if (determinedApplyCerus != null && determinedApplyDeimos != null)
            {
                fightData.SetSuccess(true, Math.Max(determinedApplyCerus.Time, determinedApplyDeimos.Time));
            }
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (TargetHPPercentUnderThreshold(TargetID.CerusLonelyTower, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
            if (TargetHPPercentUnderThreshold(TargetID.DeimosLonelyTower, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
            return FightData.EncounterStartStatus.Normal;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor cerus = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CerusLonelyTower));
            if (cerus == null)
            {
                throw new MissingKeyActorsException("Cerus not found");
            }
            AbstractSingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DeimosLonelyTower));
            if (deimos == null)
            {
                throw new MissingKeyActorsException("Deimos not found");
            }
            phases[0].AddTarget(cerus);
            phases[0].AddTarget(deimos);
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)TargetID.CerusLonelyTower,
                (int)TargetID.DeimosLonelyTower,
            };
        }
        protected override Dictionary<int, int> GetTargetsSortIDs()
        {
            return new Dictionary<int, int>()
            {
                {(int)TargetID.CerusLonelyTower, 0 },
                {(int)TargetID.DeimosLonelyTower, 0 },
            };
        }
    }
}
