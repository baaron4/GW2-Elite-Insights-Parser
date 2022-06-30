using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class AiKeeperOfThePeak : FractalLogic
    {
        private bool _hasDarkMode = false;
        private bool _hasElementalMode = false;
        private bool _china = false;
        public AiKeeperOfThePeak(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                // General
                new HitOnPlayerMechanic(61463, "Elemental Whirl", new MechanicPlotlySetting(Symbols.Square,Colors.LightRed), "Ele.Whrl.","Elemental Whirl", "Elemental Whirl",0),
                // Air
            new HitOnPlayerMechanic(new long[] {61574, 61534, 61196 }, "Elemental Manipulation (Air)", new MechanicPlotlySetting(Symbols.Square,Colors.Magenta), "Ar.Mnp.","Elemental Manipulation (Air)", "Elemental Manipulation (Air)",0),
            new HitOnPlayerMechanic(new long[] {61487, 61565 }, "Fulgor Sphere", new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "Flg.Sph.","Fulgor Sphere", "Fulgor Sphere",0),
            new HitOnPlayerMechanic(61470, "Volatile Wind", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Magenta), "Vlt.Wnd.","Volatile Wind", "Volatile Wind",0),
            new HitOnPlayerMechanic(61205, "Wind Burst", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Magenta), "Wnd.Brst.","Wind Burst", "Wind Burst",0),
            new HitOnPlayerMechanic(61205, "Wind Burst Launch", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Magenta), "L.Wnd.Burst","Launched up by Wind Burst", "Wind Burst Launch",0,(de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new HitOnPlayerMechanic(61190 , "Call of Storms", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Magenta), "Call.Strs","Call of Storms", "Call of Storms",0),
            new EnemyBuffApplyMechanic(WhirlwindShield, "Whirlwind Shield",new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Magenta), "W.Shield" ,"Whirlwind Shield","Whirlwind Shield",0),
            // Fire
            new HitOnPlayerMechanic(new long[] {61279, 61256, 61271 }, "Elemental Manipulation (Fire)", new MechanicPlotlySetting(Symbols.Square,Colors.Orange), "Fr.Mnp.","Elemental Manipulation (Fire)", "Elemental Manipulation (Fire)",0),
            new HitOnPlayerMechanic(new long[] {61273, 61582 }, "Roiling Flames", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Rlng.Flms.","Roiling Flames", "Roiling Flames",0),
            new HitOnPlayerMechanic(61548, "Volatile Fire", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Orange), "Vlt.Fr.","Volatile Fire", "Volatile Fire",0),
            new SkillOnPlayerMechanic(61348, "Call Meteor", new MechanicPlotlySetting(Symbols.Hexagram,Colors.Orange), "Mtr.H","Hit by Meteor", "Meteor Hit",1000, (evt, log) => evt.HasDowned || evt.HasKilled),
            new HitOnPlayerMechanic(61248, "Flame Burst", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Orange), "Flm.Brst.","Flame Burst", "Flame Burst",0),
            new HitOnPlayerMechanic(61445, "Firestorm", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Orange), "Fr.Strm","Firestorm", "Firestorm",0),
            new EnemyCastStartMechanic(61439, "Call Meteor", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Orange), "Smn.Meteor", "Summoned Meteor", "Summon Meteor", 0),
            // Water
            new HitOnPlayerMechanic(new long[] {61172, 61207, 61556 }, "Elemental Manipulation (Water)", new MechanicPlotlySetting(Symbols.Square,Colors.LightBlue), "Wtr.Mnp.","Elemental Manipulation (Water)", "Elemental Manipulation (Water)",0),
            new HitOnPlayerMechanic(new long[] {61556, 61177 }, "Torrential Bolt", new MechanicPlotlySetting(Symbols.Circle,Colors.LightBlue), "Tr.Blt.","Torrential Bolt", "Torrential Bolt",0),
            new HitOnPlayerMechanic(61419, "Volatile Water", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.LightBlue), "Vlt.Wtr.","Volatile Water", "Volatile Water",0),
            new HitOnPlayerMechanic(61251, "Aquatic Burst", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.LightBlue), "Aq.Brst.","Aquatic Burst", "Aquatic Burst",0),
            new EnemyBuffApplyMechanic(TidalBarrier, "Tidal Barrier", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.LightBlue), "Tdl.Bar.", "Tidal Barrier", "Tidal Barrier", 0),
            new PlayerBuffApplyMechanic(TidalBargain, "Tidal Bargain", new MechanicPlotlySetting(Symbols.StarOpen,Colors.LightBlue), "Tdl.Brgn.","Downed by Tidal Bargain", "Tidal Bargain",0),
            new PlayerBuffRemoveMechanic(TidalBargain, "Tidal Bargain Downed", new MechanicPlotlySetting(Symbols.Star,Colors.LightBlue), "Tdl.Brgn.Dwn.","Downed by Tidal Bargain", "Tidal Bargain Downed",0, (evt, log) => evt.RemovedStacks == 10 && Math.Abs(evt.RemovedDuration - 90000) < 10 * ParserHelper.ServerDelayConstant && log.CombatData.GetBuffData(Downed).Any(x => Math.Abs(x.Time - evt.Time) < 50 && x is BuffApplyEvent bae && bae.To == evt.To)),
            // Dark
            new HitOnPlayerMechanic(new long[] {61602, 61606, 61604, 61508, 61217, 61529, 61260, 61600, 61527 }, "Empathic Manipulation", new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "Emp.Mnp.","Empathic Manipulation", "Empathic Manipulation",0),
            new HitOnPlayerMechanic(new long[] {61344, 61499 }, "Focused Wrath", new MechanicPlotlySetting(Symbols.Circle,Colors.LightPurple), "Fcsd.Wrth.","Focused Wrath", "Focused Wrath",0),
            new HitOnPlayerMechanic(61289, "Negative Burst", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.LightPurple), "N.Brst.","Negative Burst", "Negative Burst",500),
            new HitOnPlayerMechanic(61184, "Terrorstorm", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.LightPurple), "TrrStrm","Terrorstorm", "Terrorstorm",0),
            new PlayerBuffApplyMechanic(CrushingGuilt, "Crushing Guilt", new MechanicPlotlySetting(Symbols.StarOpen,Colors.LightPurple), "Crsh.Glt.","Crushing Guilt", "Crushing Guilt",0),
            new PlayerBuffRemoveMechanic(CrushingGuilt, "Crushing Guilt Down", new MechanicPlotlySetting(Symbols.Star,Colors.LightPurple), "Crsh.Glt.Dwn.","Downed by Crushing Guilt", "Crushing Guilt Down",0, (evt, log) => evt.RemovedStacks == 10 && Math.Abs(evt.RemovedDuration - 90000) < 10 * ParserHelper.ServerDelayConstant && log.CombatData.GetBuffData(Downed).Any(x => Math.Abs(x.Time - evt.Time) < 50 && x is BuffApplyEvent bae && bae.To == evt.To)),
            new EnemyCastStartMechanic(61508, "Empathic Manipulation (Fear)", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.LightPurple), "Fear Mnp.", "Empathic Manipulation (Fear)", "Empathic Manipulation (Fear)", 0),
            new EnemyCastEndMechanic(61508, "Empathic Manipulation (Fear) Interrupt", new MechanicPlotlySetting(Symbols.TriangleUpOpen,Colors.LightPurple), "IntFear.Mnp.", "Empathic Manipulation (Fear) Interrupt", "Empathic Manipulation (Fear) Interrupt", 0, (evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyCastStartMechanic(61606, "Empathic Manipulation (Sorrow)", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.LightPurple), "Sor.Mnp.", "Empathic Manipulation (Sorrow)", "Empathic Manipulation (Sorrow)", 0),
            new EnemyCastEndMechanic(61606, "Empathic Manipulation (Sorrow) Interrupt", new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.LightPurple), "IntSor.Mnp.", "Empathic Manipulation (Sorrow) Interrupt", "Empathic Manipulation (Sorrow) Interrupt", 0, (evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyCastStartMechanic(61602, "Empathic Manipulation (Guilt)", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.LightPurple), "Glt.Mnp.", "Empathic Manipulation (Guilt)", "Empathic Manipulation (Guilt)", 0),
            new EnemyCastEndMechanic(61602, "Empathic Manipulation (Guilt) Interrupt", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.LightPurple), "Int.Glt.Mnp.", "Empathic Manipulation (Guilt) Interrupt", "Empathic Manipulation (Guilt) Interrupt", 0, (evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyBuffApplyMechanic(CacophonousMind, "Cacophonous Mind", new MechanicPlotlySetting(Symbols.Pentagon,Colors.LightPurple), "Ccphns.Mnd.","Cacophonous Mind", "Cacophonous Mind",0),
            });
            Extension = "ai";
            Icon = "https://i.imgur.com/3mlCdI9.png";
            EncounterCategoryInformation.SubCategory = SubFightCategory.SunquaPeak;
            EncounterID |= EncounterIDs.FractalMasks.SunquaPeakMask;
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            if (_hasDarkMode && _hasElementalMode)
            {
                EncounterID |= 0x000001;
                return "Ai, Keeper of the Peak";
            }
            else if (_hasDarkMode)
            {
                EncounterID |= 0x000003;
                return "Dark Ai, Keeper of the Peak";
            }
            else
            {
                EncounterID |= 0x000002;
                return "Elemental Ai, Keeper of the Peak";
            }
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/zSBL7YP.png",
                            (823, 1000),
                            (5411, -95, 8413, 3552));
        }

        /*internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            // Tidal Bargain, Cacophonous Mind and Crushing Guilt adjust
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 61512);
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 61208);
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 61435);
            return res;
        }*/

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak,
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2,
                (int)ArcDPSEnums.TrashID.SorrowDemon5,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.FearDemon,
                ArcDPSEnums.TrashID.GuiltDemon,
                ArcDPSEnums.TrashID.EnrageWaterSprite,
                // Transition sorrow demons
                ArcDPSEnums.TrashID.SorrowDemon1,
                ArcDPSEnums.TrashID.SorrowDemon2,
                ArcDPSEnums.TrashID.SorrowDemon3,
                ArcDPSEnums.TrashID.SorrowDemon4,
            };
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak,
                (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2,
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            AgentItem aiAgent = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.AiKeeperOfThePeak).FirstOrDefault();
            if (aiAgent == null)
            {
                throw new MissingKeyActorsException("Ai not found");
            }
            _china = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.Language && x.SrcAgent == (ulong)LanguageEvent.LanguageEnum.Chinese) != null;
            CombatItem darkModePhaseEvent = combatData.FirstOrDefault(x => x.SkillID == 53569 && x.SrcMatchesAgent(aiAgent));
            _hasDarkMode = combatData.Exists(x => (_china ? x.SkillID == 61358 : x.SkillID == 61356) && x.SrcMatchesAgent(aiAgent));
            _hasElementalMode = !_hasDarkMode || darkModePhaseEvent != null;
            if (_hasDarkMode)
            {
                if (_hasElementalMode)
                {
                    long darkModeStart = combatData.FirstOrDefault(x => (_china ? x.SkillID == 61279 : x.SkillID == 61277) && x.Time >= darkModePhaseEvent.Time && x.SrcMatchesAgent(aiAgent)).Time;
                    CombatItem invul895Loss = combatData.FirstOrDefault(x => x.Time <= darkModeStart && x.SkillID == Determined895 && x.IsBuffRemove == ArcDPSEnums.BuffRemove.All && x.SrcMatchesAgent(aiAgent));
                    long lastAwareTime = (invul895Loss != null ? invul895Loss.Time : darkModeStart);
                    AgentItem darkAiAgent = agentData.AddCustomNPCAgent(lastAwareTime + 1, aiAgent.LastAware, aiAgent.Name, aiAgent.Spec, (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2, false, aiAgent.Toughness, aiAgent.Healing, aiAgent.Condition, aiAgent.Concentration, aiAgent.HitboxWidth, aiAgent.HitboxHeight);
                    // Redirect combat events
                    foreach (CombatItem evt in combatData)
                    {
                        if (evt.Time >= darkAiAgent.FirstAware && evt.Time <= darkAiAgent.LastAware)
                        {
                            if (evt.SrcMatchesAgent(aiAgent, extensions))
                            {
                                evt.OverrideSrcAgent(darkAiAgent.Agent);
                            }
                            if (evt.DstMatchesAgent(aiAgent, extensions))
                            {
                                evt.OverrideDstAgent(darkAiAgent.Agent);
                            }
                        }
                    }
                    CombatItem healthUpdateToCopy = combatData.LastOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && x.SrcMatchesAgent(aiAgent) && x.Time <= lastAwareTime - 1);
                    if (healthUpdateToCopy != null)
                    {
                        //
                        {
                            var elAI0HP = new CombatItem(healthUpdateToCopy);
                            elAI0HP.OverrideDstAgent(0);
                            elAI0HP.OverrideTime(lastAwareTime);
                            combatData.Add(elAI0HP);
                        }
                        //
                        {
                            var darkAI0HP = new CombatItem(healthUpdateToCopy);
                            darkAI0HP.OverrideDstAgent(0);
                            darkAI0HP.OverrideTime(darkAiAgent.FirstAware);
                            darkAI0HP.OverrideSrcAgent(darkAiAgent.Agent);
                            combatData.Add(darkAI0HP);
                        }
                    }
                    aiAgent.OverrideAwareTimes(aiAgent.FirstAware, lastAwareTime);
                    // Redirect NPC masters
                    foreach (AgentItem ag in agentData.GetAgentByType(AgentItem.AgentType.NPC))
                    {
                        if (ag.Master == aiAgent && ag.FirstAware >= aiAgent.LastAware)
                        {
                            ag.SetMaster(darkAiAgent);
                        }
                    }
                    // Redirect Gadget masters
                    foreach (AgentItem ag in agentData.GetAgentByType(AgentItem.AgentType.Gadget))
                    {
                        if (ag.Master == aiAgent && ag.FirstAware >= aiAgent.LastAware)
                        {
                            ag.SetMaster(darkAiAgent);
                        }
                    }
                }
                else
                {
                    Extension = "drkai";
                    aiAgent.OverrideID(ArcDPSEnums.TargetID.AiKeeperOfThePeak2);
                    agentData.Refresh();
                }
            }
            else
            {
                Extension = "elai";
            }
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            // Manually set HP and names
            AbstractSingleActor eleAi = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak);
            AbstractSingleActor darkAi = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2);
            darkAi?.OverrideName("Dark Ai");
            eleAi?.OverrideName("Elemental Ai");
            if (_hasElementalMode && _hasDarkMode)
            {
                CombatItem aiMaxHP = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && x.SrcMatchesAgent(aiAgent));
                if (aiMaxHP != null)
                {
                    darkAi.SetManualHealth((int)aiMaxHP.DstAgent);
                }
            }
            if (_hasDarkMode)
            {
                int sorrowCount = 0;
                foreach (AbstractSingleActor target in Targets)
                {
                    if (target.ID == (int)ArcDPSEnums.TrashID.SorrowDemon5)
                    {
                        target.OverrideName(target.Character + " " + (++sorrowCount));
                    }
                }
            }
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor elementalAi = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak);
            if (elementalAi == null)
            {
                if (_hasElementalMode)
                {
                    throw new MissingKeyActorsException("Ai not found");
                }
            }
            else
            {
                phases[0].AddTarget(elementalAi);
            }
            AbstractSingleActor darkAi = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2);
            if (darkAi == null)
            {
                if (_hasDarkMode)
                {
                    throw new MissingKeyActorsException("Ai not found");
                }
            }
            else
            {
                phases[0].AddTarget(darkAi);
            }
            if (_hasElementalMode)
            {
                BuffApplyEvent invul895Gain = log.CombatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == elementalAi.AgentItem).FirstOrDefault();
                long eleStart = Math.Max(elementalAi.FirstAware, 0);
                long eleEnd = invul895Gain != null ? invul895Gain.Time : log.FightData.FightEnd;
                if (_hasDarkMode)
                {
                    var elePhase = new PhaseData(eleStart, eleEnd, "Elemental Phase");
                    elePhase.AddTarget(elementalAi);
                    phases.Add(elePhase);
                }
                if (requirePhases)
                {

                    //
                    var invul762Gains = log.CombatData.GetBuffData(Determined762).OfType<BuffApplyEvent>().Where(x => x.To == elementalAi.AgentItem).ToList();
                    var invul762Losses = log.CombatData.GetBuffData(Determined762).OfType<BuffRemoveAllEvent>().Where(x => x.To == elementalAi.AgentItem).ToList();
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
                            subPhase.AddTarget(elementalAi);
                            phases.Add(subPhase);
                            long invul762Loss = invul762Losses[i].Time;
                            long skillID = _china ? 61388 : 61385;
                            AbstractCastEvent castEvt = elementalAi.GetCastEvents(log, eleStart, eleEnd).FirstOrDefault(x => x.SkillId == skillID && x.Time >= invul762Loss);
                            if (castEvt == null)
                            {
                                break;
                            }
                            subStart = castEvt.Time;
                        }
                        else
                        {
                            var subPhase = new PhaseData(subStart, subEnd, eleNames[i]);
                            subPhase.AddTarget(elementalAi);
                            phases.Add(subPhase);
                            break;
                        }

                    }
                }
            }
            if (_hasDarkMode)
            {
                BuffApplyEvent invul895Gain = log.CombatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == darkAi.AgentItem).FirstOrDefault();
                long darkStart = Math.Max(darkAi.FirstAware, 0);
                long darkEnd = invul895Gain != null ? invul895Gain.Time : log.FightData.FightEnd;
                if (_hasElementalMode)
                {
                    var darkPhase = new PhaseData(darkStart, darkEnd, "Dark Phase");
                    darkPhase.AddTarget(darkAi);
                    phases.Add(darkPhase);
                }
                if (requirePhases)
                {
                    // sub phases
                    long fearToSorrowSkillID = _china ? 61571 : 61606;
                    AbstractCastEvent fearToSorrow = darkAi.GetCastEvents(log, darkStart, darkEnd).FirstOrDefault(x => x.SkillId == fearToSorrowSkillID);
                    if (fearToSorrow != null)
                    {
                        var fearPhase = new PhaseData(darkStart + 1, fearToSorrow.Time, "Fear");
                        fearPhase.AddTarget(darkAi);
                        phases.Add(fearPhase);
                        long sorrowToGuiltSkillID = _china ? 61361 : 61602;
                        AbstractCastEvent sorrowToGuilt = darkAi.GetCastEvents(log, darkStart, darkEnd).FirstOrDefault(x => x.SkillId == sorrowToGuiltSkillID);
                        if (sorrowToGuilt != null)
                        {
                            var sorrowPhase = new PhaseData(fearToSorrow.Time + 1, sorrowToGuilt.Time, "Sorrow");
                            sorrowPhase.AddTarget(darkAi);
                            phases.Add(sorrowPhase);
                            var guiltPhase = new PhaseData(sorrowToGuilt.Time + 1, darkEnd, "Guilt");
                            guiltPhase.AddTarget(darkAi);
                            phases.Add(guiltPhase);
                        }
                        else
                        {
                            var sorrowPhase = new PhaseData(fearToSorrow.Time + 1, darkEnd, "Sorrow");
                            sorrowPhase.AddTarget(darkAi);
                            phases.Add(sorrowPhase);
                        }
                    }
                }
            }
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
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
                    BuffApplyEvent invul895Gain = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == Targets[0].AgentItem).FirstOrDefault();
                    if (invul895Gain != null)
                    {
                        fightData.SetSuccess(true, invul895Gain.Time);
                    }
                    break;
                case 3:
                    BuffApplyEvent darkInvul895Gain = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == Targets.FirstOrDefault(y => y.ID == (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak2).AgentItem).FirstOrDefault();
                    if (darkInvul895Gain != null)
                    {
                        fightData.SetSuccess(true, darkInvul895Gain.Time);
                    }
                    break;
                case 0:
                default:
                    throw new MissingKeyActorsException("Ai not found");
            }
        }
    }
}
