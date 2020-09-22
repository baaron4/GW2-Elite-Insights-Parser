using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class AiKeeperOfThePeak : FractalLogic
    {
        private bool _hasDarkMode = false;
        private bool _hasElementalMode = false;
        public AiKeeperOfThePeak(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {       
            });
            Extension = "keeperpeak";
            Icon = "https://i.imgur.com/3mlCdI9.png";
        }

        internal override string GetLogicName(ParsedEvtcLog log)
        {
            return "Ai, Keeper of the Peak";
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak,
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2,
            };
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak,
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2,
            };
        }

        internal override void EIEvtcParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, List<Player> playerList)
        {
            AgentItem targetAgent = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.AiKeeperOfThePeak).FirstOrDefault();
            if (targetAgent == null)
            {
                throw new InvalidOperationException("Ai not found");
            }
            CombatItem darkModePhaseEvent = combatData.FirstOrDefault(x => x.SkillID == 53569);
            _hasDarkMode = combatData.Exists(x => x.SkillID == 61356);
            _hasElementalMode = !_hasDarkMode || darkModePhaseEvent != null;
            targetAgent.OverrideName("Elemental Ai");
            if (_hasDarkMode)
            {
                CombatItem darkModeStartEvent = combatData.FirstOrDefault(x => x.SkillID == 61277 && x.Time >= darkModePhaseEvent.Time);
                if (_hasElementalMode)
                {
                    CombatItem invul895Loss = combatData.FirstOrDefault(x => x.Time <= darkModeStartEvent.Time && x.SkillID == 895 && x.IsBuffRemove == ArcDPSEnums.BuffRemove.All);
                    long lastAwareTime = (invul895Loss != null ? invul895Loss.Time : darkModeStartEvent.Time);
                    AgentItem targetAgent2 = agentData.AddCustomAgent(lastAwareTime + 1, targetAgent.LastAware, AgentItem.AgentType.NPC, "Dark Ai", "", (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2, targetAgent.Toughness, targetAgent.Healing, targetAgent.Condition, targetAgent.Concentration, targetAgent.HitboxWidth, targetAgent.HitboxHeight);
                    targetAgent.OverrideAwareTimes(targetAgent.FirstAware, lastAwareTime);
                    // Redirect combat events
                    foreach (CombatItem evt in combatData)
                    {
                        if (evt.Time >= targetAgent2.FirstAware && evt.Time <= targetAgent2.LastAware)
                        {
                            if (evt.IsStateChange.SrcIsAgent() && evt.SrcAgent == targetAgent.Agent)
                            {
                                evt.OverrideSrcAgent(targetAgent2.Agent);
                            }
                            if (evt.IsStateChange.DstIsAgent() && evt.DstAgent == targetAgent.Agent)
                            {
                                evt.OverrideDstAgent(targetAgent2.Agent);
                            }
                        }
                    }
                    // Redirect NPC masters
                    foreach (AgentItem ag in agentData.GetAgentByType(AgentItem.AgentType.NPC))
                    {
                        if (ag.Master == targetAgent && ag.FirstAware >= targetAgent.LastAware)
                        {
                            ag.SetMaster(targetAgent2);
                        }
                    }
                    // Redirect Gadget masters
                    foreach (AgentItem ag in agentData.GetAgentByType(AgentItem.AgentType.Gadget))
                    {
                        if (ag.Master == targetAgent && ag.FirstAware >= targetAgent.LastAware)
                        {
                            ag.SetMaster(targetAgent2);
                        }
                    }
                }
                else
                {
                    targetAgent.OverrideName("Dark Ai");
                    targetAgent.OverrideID((int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2);
                    agentData.Refresh();
                }
            } 
            else
            {
                agentData.Refresh();
            }
            base.EIEvtcParse(fightData, agentData, combatData, playerList);
            // Manually set HP
            if (_hasElementalMode && _hasDarkMode)
            {
                CombatItem aiMaxHP = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && x.SrcAgent == targetAgent.Agent);
                if (aiMaxHP != null)
                {
                    Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2).SetManualHealth((int)aiMaxHP.DstAgent);
                }
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.CMnoName;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC elementalAI = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak);
            if (elementalAI == null)
            {
                if (_hasElementalMode)
                {
                    throw new InvalidOperationException("Ai not found");
                }
            } 
            else
            {
                phases[0].Targets.Add(elementalAI);
            }
            NPC darkAI = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2);
            if (darkAI == null)
            {
                if (_hasDarkMode)
                {
                    throw new InvalidOperationException("Ai not found");
                }
            } 
            else
            {
                phases[0].Targets.Add(darkAI);
            }
            if (!requirePhases)
            {
                return phases;
            }
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            int status = 0;
            if (_hasElementalMode)
            {
                status |= 1;
            }
            if (_hasDarkMode)
            {
                status |= 2;
            }
            switch (status)
            {
                case 1:
                case 2:
                    BuffApplyEvent invul895Gain = combatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == Targets[0].AgentItem).FirstOrDefault();
                    if (invul895Gain != null)
                    {
                        fightData.SetSuccess(true, invul895Gain.Time);
                    }
                    break;
                case 3:
                    BuffApplyEvent darkInvul895Gain = combatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == Targets.Find(y => y.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2).AgentItem).FirstOrDefault();
                    if (darkInvul895Gain != null)
                    {
                        fightData.SetSuccess(true, darkInvul895Gain.Time);
                    }
                    break;
                case 0:
                default:
                    throw new InvalidOperationException("Ai not found");
            }
        }
    }
}
