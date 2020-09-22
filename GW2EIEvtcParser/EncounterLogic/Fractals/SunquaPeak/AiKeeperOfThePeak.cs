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
            Extension = "ai";
            Icon = "https://i.imgur.com/3mlCdI9.png";
        }

        internal override string GetLogicName(ParsedEvtcLog log)
        {
            if (_hasDarkMode && _hasElementalMode)
            {
                return "Ai, Keeper of the Peak";
            } 
            else if (_hasDarkMode)
            {
                return "Dark Ai, Keeper of the Peak";
            } 
            else
            {
                return "Elemental Ai, Keeper of the Peak";
            }
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
                if (_hasElementalMode)
                {
                    long darkModeStart = combatData.FirstOrDefault(x => x.SkillID == 61277 && x.Time >= darkModePhaseEvent.Time).Time;
                    CombatItem invul895Loss = combatData.FirstOrDefault(x => x.Time <= darkModeStart && x.SkillID == 895 && x.IsBuffRemove == ArcDPSEnums.BuffRemove.All);
                    long lastAwareTime = (invul895Loss != null ? invul895Loss.Time : darkModeStart);
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
                    Extension = "drkai";
                    targetAgent.OverrideName("Dark Ai");
                    targetAgent.OverrideID((int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2);
                    agentData.Refresh();
                }
            } 
            else
            {
                Extension = "elai";
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
            NPC elementalAi = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak);
            if (elementalAi == null)
            {
                if (_hasElementalMode)
                {
                    throw new InvalidOperationException("Ai not found");
                }
            } 
            else
            {
                phases[0].Targets.Add(elementalAi);
            }
            NPC darkAi = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2);
            if (darkAi == null)
            {
                if (_hasDarkMode)
                {
                    throw new InvalidOperationException("Ai not found");
                }
            } 
            else
            {
                phases[0].Targets.Add(darkAi);
            }
            if (!requirePhases)
            {
                return phases;
            }
            if (_hasElementalMode)
            {
                BuffApplyEvent invul895Gain = log.CombatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == elementalAi.AgentItem).FirstOrDefault();
                long eleStart = Math.Max(elementalAi.FirstAware, 0);
                long eleEnd = invul895Gain != null ? invul895Gain.Time : log.FightData.FightEnd;
                if (_hasDarkMode)
                {
                    var elePhase = new PhaseData(eleStart, eleEnd, "Elemental Phase");
                    elePhase.Targets.Add(elementalAi);
                    phases.Add(elePhase);
                }          
                //
                var invul762Gains = log.CombatData.GetBuffData(762).OfType<BuffApplyEvent>().Where(x => x.To == elementalAi.AgentItem).ToList();
                var invul762Losses = log.CombatData.GetBuffData(762).OfType<BuffRemoveAllEvent>().Where(x => x.To == elementalAi.AgentItem).ToList();
                // sub phases
                string[] eleNames = { "Air", "Fire", "Water" };
                long subStart = eleStart;
                long subEnd = 0;
                for (int i = 0; i < invul762Gains.Count; i++)
                {
                    subEnd = invul762Gains[i].Time;
                    if (i < invul762Losses.Count)
                    {
                        var subPhase = new PhaseData(subStart, subEnd, eleNames[i]);
                        subPhase.Targets.Add(elementalAi);
                        phases.Add(subPhase);
                        long invul762Loss = invul762Losses[i].Time;
                        AbstractCastEvent castEvt = elementalAi.GetCastLogs(log, eleStart, eleEnd).FirstOrDefault(x => x.SkillId == 61385 && x.Time >= invul762Loss);
                        if (castEvt == null)
                        {
                            break;
                        }
                        subStart = castEvt.Time;
                    } 
                    else
                    {
                        var subPhase = new PhaseData(subStart, subEnd, eleNames[i]);
                        subPhase.Targets.Add(elementalAi);
                        phases.Add(subPhase);
                        break;
                    }
                    
                }
            }
            if (_hasDarkMode)
            {
                BuffApplyEvent invul895Gain = log.CombatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == darkAi.AgentItem).FirstOrDefault();
                long darkStart = Math.Max(darkAi.FirstAware, 0);
                long darkEnd = invul895Gain != null ? invul895Gain.Time : log.FightData.FightEnd;
                if (_hasElementalMode)
                {
                    var darkPhase = new PhaseData(darkStart, darkEnd, "Dark Phase");
                    darkPhase.Targets.Add(darkAi);
                    phases.Add(darkPhase);
                }             
                // sub phases
                AbstractCastEvent fearToSorrow = darkAi.GetCastLogs(log, darkStart, darkEnd).FirstOrDefault(x => x.SkillId == 61606);
                if (fearToSorrow != null)
                {
                    var fearPhase = new PhaseData(darkStart + 1, fearToSorrow.Time, "Fear");
                    fearPhase.Targets.Add(darkAi);
                    phases.Add(fearPhase);
                    AbstractCastEvent sorrowToGuilt = darkAi.GetCastLogs(log, darkStart, darkEnd).FirstOrDefault(x => x.SkillId == 61602);
                    if (sorrowToGuilt != null)
                    {
                        var sorrowPhase = new PhaseData(fearToSorrow.Time + 1, sorrowToGuilt.Time, "Sorrow");
                        sorrowPhase.Targets.Add(darkAi);
                        phases.Add(sorrowPhase);
                        var guiltPhase = new PhaseData(sorrowToGuilt.Time + 1, darkEnd, "Guilt");
                        guiltPhase.Targets.Add(darkAi);
                        phases.Add(guiltPhase);
                    }
                    else
                    {
                        var sorrowPhase = new PhaseData(fearToSorrow.Time + 1, darkEnd, "Sorrow");
                        sorrowPhase.Targets.Add(darkAi);
                        phases.Add(sorrowPhase);
                    }
                }
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
