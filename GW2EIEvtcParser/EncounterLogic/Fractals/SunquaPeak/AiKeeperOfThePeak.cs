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
                // General
                new HitOnPlayerMechanic(61463, "Elemental Whirl", new MechanicPlotlySetting("square","rgb(255,125,125)"), "Ele.Whrl.","Elemental Whirl", "Elemental Whirl",0),
                // Air
            new HitOnPlayerMechanic(61574, "Elemental Manipulation (Air)", new MechanicPlotlySetting("square","rgb(255,0,255)"), "Air Manip.","Elemental Manipulation (Air)", "Elemental Manipulation (Air)",0),
            new HitOnPlayerMechanic(61534, "Elemental Manipulation (Air)", new MechanicPlotlySetting("square","rgb(255,0,255)"), "Air Manip.","Elemental Manipulation (Air)", "Elemental Manipulation (Air)",0),
            new HitOnPlayerMechanic(61196, "Elemental Manipulation (Air)", new MechanicPlotlySetting("square","rgb(255,0,255)"), "Air Manip.","Elemental Manipulation (Air)", "Elemental Manipulation (Air)",0),
            new HitOnPlayerMechanic(61487, "Fulgor Sphere", new MechanicPlotlySetting("circle","rgb(255,0,255)"), "Flg.Sph.","Fulgor Sphere", "Fulgor Sphere",0),
            new HitOnPlayerMechanic(61565, "Fulgor Sphere", new MechanicPlotlySetting("circle","rgb(255,0,255)"), "Flg.Sph.","Fulgor Sphere", "Fulgor Sphere",0),
            new HitOnPlayerMechanic(61470, "Volatile Wind", new MechanicPlotlySetting("triangle-left","rgb(255,0,255)"), "Vlt.Wnd.","Volatile Wind", "Volatile Wind",0),
            new HitOnPlayerMechanic(61205, "Wind Burst", new MechanicPlotlySetting("triangle-down-open","rgb(255,0,255)"), "Wnd.Burst","Wind Burst", "Wind Burst",0),
            new HitOnPlayerMechanic(61205, "Wind Burst Launch", new MechanicPlotlySetting("triangle-down","rgb(255,0,255)"), "L.Wnd.Burst","Launched up by Wind Burst", "Wind Burst Launch",0,(de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(61190 , "Call of Storms", new MechanicPlotlySetting("triangle-up","rgb(255,0,255)"), "Call Strs","Call of Storms", "Call of Storms",0),
            new EnemyBuffApplyMechanic(61224, "Whirlwind Shield",new MechanicPlotlySetting("asterisk-open","rgb(255,0,255)"), "W.Shield" ,"Whirlwind Shield","Whirlwind Shield",0),
            // Fire
            new HitOnPlayerMechanic(61279, "Elemental Manipulation (Fire)", new MechanicPlotlySetting("square","rgb(255,125,0)"), "Fire Manip.","Elemental Manipulation (Fire)", "Elemental Manipulation (Fire)",0),
            new HitOnPlayerMechanic(61256, "Elemental Manipulation (Fire)", new MechanicPlotlySetting("square","rgb(255,125,0)"), "Fire Manip.","Elemental Manipulation (Fire)", "Elemental Manipulation (Fire)",0),
            new HitOnPlayerMechanic(61271, "Elemental Manipulation (Fire)", new MechanicPlotlySetting("square","rgb(255,125,0)"), "Fire Manip.","Elemental Manipulation (Fire)", "Elemental Manipulation (Fire)",0),
            new HitOnPlayerMechanic(61273, "Roiling Flames", new MechanicPlotlySetting("circle","rgb(255,125,0)"), "Rlng.Flms.","Roiling Flames", "Roiling Flames",0),
            new HitOnPlayerMechanic(61582, "Roiling Flames", new MechanicPlotlySetting("circle","rgb(255,125,0)"), "Rlng.Flms.","Roiling Flames", "Roiling Flames",0),      
            new HitOnPlayerMechanic(61548, "Volatile Fire", new MechanicPlotlySetting("triangle-left","rgb(255,125,0)"), "Vlt.Fr.","Volatile Fire", "Volatile Fire",0),
            new SkillOnPlayerMechanic(61348, "Call Meteor", new MechanicPlotlySetting("hexagram","rgb(255,125,0)"), "Meteor.H","Hit by Meteor", "Meteor Hit",1000, (evt, log) => evt.HasDowned || evt.HasKilled),
            new HitOnPlayerMechanic(61248, "Flame Burst", new MechanicPlotlySetting("triangle-down","rgb(255,125,0)"), "Flm.Burst","Flame Burst", "Flame Burst",0),
            new HitOnPlayerMechanic(61445, "Firestorm", new MechanicPlotlySetting("triangle-up","rgb(255,125,0)"), "Fr.Strm","Firestorm", "Firestorm",0),
            new EnemyCastStartMechanic(61439, "Call Meteor", new MechanicPlotlySetting("asterisk-open","rgb(255,125,0)"), "Smn.Meteor", "Summoned Meteor", "Summon Meteor", 0),
            // Water
            new HitOnPlayerMechanic(61172, "Elemental Manipulation (Water)", new MechanicPlotlySetting("square","rgb(0,125,255)"), "Water Manip.","Elemental Manipulation (Water)", "Elemental Manipulation (Water)",0),
            new HitOnPlayerMechanic(61207, "Elemental Manipulation (Water)", new MechanicPlotlySetting("square","rgb(0,125,255)"), "Water Manip.","Elemental Manipulation (Water)", "Elemental Manipulation (Water)",0),
            new HitOnPlayerMechanic(61556, "Elemental Manipulation (Water)", new MechanicPlotlySetting("square","rgb(0,125,255)"), "Water Manip.","Elemental Manipulation (Water)", "Elemental Manipulation (Water)",0),
            new HitOnPlayerMechanic(61556, "Torrential Bolt", new MechanicPlotlySetting("circle","rgb(0,125,255)"), "Tor.Bolt","Torrential Bolt", "Torrential Bolt",0),
            new HitOnPlayerMechanic(61177, "Torrential Bolt", new MechanicPlotlySetting("circle","rgb(0,125,255)"), "Tor.Bolt","Torrential Bolt", "Torrential Bolt",0),   
            new HitOnPlayerMechanic(61419, "Volatile Water", new MechanicPlotlySetting("triangle-left","rgb(0,125,255)"), "Vlt.Wtr.","Volatile Water", "Volatile Water",0),
            new HitOnPlayerMechanic(61251, "Aquatic Burst", new MechanicPlotlySetting("triangle-down","rgb(0,125,255)"), "Aq.Burst","Aquatic Burst", "Aquatic Burst",0),
            new EnemyBuffApplyMechanic(61402, "Tidal Barrier", new MechanicPlotlySetting("asterisk-open","rgb(0,125,255)"), "Tid.Bar.", "Tidal Barrier", "Tidal Barrier", 0),
            // Dark
            new HitOnPlayerMechanic(61602, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61606, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61604, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61508, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61217, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61529, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61260, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61600, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61527, "Empathic Manipulation", new MechanicPlotlySetting("square","rgb(150,125,255)"), "Emp.Manip.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(61344, "Focused Wrath", new MechanicPlotlySetting("circle","rgb(150,125,255)"), "Fcsd.Wrth.","Focused Wrath", "Focused Wrath",0),
            new HitOnPlayerMechanic(61499, "Focused Wrath", new MechanicPlotlySetting("circle","rgb(150,125,255)"), "Fcsd.Wrth.","Focused Wrath", "Focused Wrath",0),
            new EnemyCastStartMechanic(61508, "Empathic Manipulation (Fear)", new MechanicPlotlySetting("triangle-up","rgb(150,125,255)"), "Fear Manip.", "Empathic Manipulation (Fear)", "Empathic Manipulation (Fear)", 0),
            new EnemyCastEndMechanic(61508, "Empathic Manipulation (Fear) Interrupt", new MechanicPlotlySetting("triangle-up-open","rgb(150,125,255)"), "IntFear Manip.", "Empathic Manipulation (Fear) Interrupt", "Empathic Manipulation (Fear) Interrupt", 0, (evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyCastStartMechanic(61606, "Empathic Manipulation (Sorrow)", new MechanicPlotlySetting("triangle-left","rgb(150,125,255)"), "Sor.Manip.", "Empathic Manipulation (Sorrow)", "Empathic Manipulation (Sorrow)", 0),
            new EnemyCastEndMechanic(61606, "Empathic Manipulation (Sorrow) Interrupt", new MechanicPlotlySetting("triangle-left-open","rgb(150,125,255)"), "IntSor.Manip.", "Empathic Manipulation (Sorrow) Interrupt", "Empathic Manipulation (Sorrow) Interrupt", 0, (evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyCastStartMechanic(61602, "Empathic Manipulation (Guilt)", new MechanicPlotlySetting("triangle-right","rgb(150,125,255)"), "Glt.Manip.", "Empathic Manipulation (Guilt)", "Empathic Manipulation (Guilt)", 0),
            new EnemyCastEndMechanic(61602, "Empathic Manipulation (Guilt) Interrupt", new MechanicPlotlySetting("triangle-right-open","rgb(150,125,255)"), "Int.Glt.Manip.", "Empathic Manipulation (Guilt) Interrupt", "Empathic Manipulation (Guilt) Interrupt", 0, (evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
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

        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            // Tidal Bargain, Cacophonous Mind and Crushing Guilt adjust
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 61512);
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 61208);
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 61435);
            return res;
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak,
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.FearDemon,
                ArcDPSEnums.TrashID.GuiltDemon,
                ArcDPSEnums.TrashID.EnrageWaterSprite,
                ArcDPSEnums.TrashID.SorrowDemon1,
                ArcDPSEnums.TrashID.SorrowDemon2,
                ArcDPSEnums.TrashID.SorrowDemon3,
                ArcDPSEnums.TrashID.SorrowDemon4,
                ArcDPSEnums.TrashID.SorrowDemon5,
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
