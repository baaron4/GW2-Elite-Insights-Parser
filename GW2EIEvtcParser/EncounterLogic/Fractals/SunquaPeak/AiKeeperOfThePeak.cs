using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using System.Reflection;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class AiKeeperOfThePeak : SunquaPeak
    {
        private bool _hasDarkMode = false;
        private bool _hasElementalMode = false;
        private bool _china = false;

        private const long Determined895Duration = int.MaxValue / 4;
        public AiKeeperOfThePeak(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                // General
                new PlayerDstHitMechanic(ElementalWhirl, "Elemental Whirl", new MechanicPlotlySetting(Symbols.Square,Colors.LightRed), "Ele.Whrl.","Elemental Whirl", "Elemental Whirl",0),
                // Air
            new PlayerDstHitMechanic(new long[] { ElementalManipulationAir1, ElementalManipulationAir2, ElementalManipulationAir3 }, "Elemental Manipulation (Air)", new MechanicPlotlySetting(Symbols.Square,Colors.Magenta), "Ar.Mnp.","Elemental Manipulation (Air)", "Elemental Manipulation (Air)",0),
            new PlayerDstHitMechanic(new long[] { FulgorSphere1, FulgorSphere2 }, "Fulgor Sphere", new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "Flg.Sph.","Fulgor Sphere", "Fulgor Sphere",0),
            new PlayerDstHitMechanic(VolatileWind, "Volatile Wind", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Magenta), "Vlt.Wnd.","Volatile Wind", "Volatile Wind",0),
            new PlayerDstHitMechanic(WindBurst, "Wind Burst", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Magenta), "Wnd.Brst.","Wind Burst", "Wind Burst",0),
            new PlayerDstHitMechanic(WindBurst, "Wind Burst Launch", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Magenta), "L.Wnd.Burst","Launched up by Wind Burst", "Wind Burst Launch",0).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
            new PlayerDstHitMechanic(CallOfStorms , "Call of Storms", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Magenta), "Call.Strs","Call of Storms", "Call of Storms",0),
            new EnemyDstBuffApplyMechanic(WhirlwindShield, "Whirlwind Shield",new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Magenta), "W.Shield" ,"Whirlwind Shield","Whirlwind Shield",0),
            // Fire
            new PlayerDstHitMechanic(new long[] { ElementalManipulationFire1, ElementalManipulationFire2, ElementalManipulationFire3 }, "Elemental Manipulation (Fire)", new MechanicPlotlySetting(Symbols.Square,Colors.Orange), "Fr.Mnp.","Elemental Manipulation (Fire)", "Elemental Manipulation (Fire)",0),
            new PlayerDstHitMechanic(new long[] { RoilingFlames1, RoilingFlames2 }, "Roiling Flames", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Rlng.Flms.","Roiling Flames", "Roiling Flames",0),
            new PlayerDstHitMechanic(VolatileFire, "Volatile Fire", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Orange), "Vlt.Fr.","Volatile Fire", "Volatile Fire",0),
            new PlayerDstSkillMechanic(CallMeteorHit, "Call Meteor", new MechanicPlotlySetting(Symbols.Hexagram,Colors.Orange), "Mtr.H","Hit by Meteor", "Meteor Hit",1000).UsingChecker((evt, log) => evt.HasDowned || evt.HasKilled),
            new PlayerDstHitMechanic(FlameBurst, "Flame Burst", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Orange), "Flm.Brst.","Flame Burst", "Flame Burst",0),
            new PlayerDstHitMechanic(FirestormAi, "Firestorm", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Orange), "Fr.Strm","Firestorm", "Firestorm",0),
            new EnemyCastStartMechanic(CallMeteorSummon, "Call Meteor", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Orange), "Smn.Meteor", "Summoned Meteor", "Summon Meteor", 0),
            // Water
            new PlayerDstHitMechanic(new long[] { ElementalManipulationWater1, ElementalManipulationWater2, ElementalManipulationWater3 }, "Elemental Manipulation (Water)", new MechanicPlotlySetting(Symbols.Square,Colors.LightBlue), "Wtr.Mnp.","Elemental Manipulation (Water)", "Elemental Manipulation (Water)",0),
            new PlayerDstHitMechanic(new long[] { TorrentialBolt1, TorrentialBolt2 }, "Torrential Bolt", new MechanicPlotlySetting(Symbols.Circle,Colors.LightBlue), "Tr.Blt.","Torrential Bolt", "Torrential Bolt",0),
            new PlayerDstHitMechanic(VolatileWater, "Volatile Water", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.LightBlue), "Vlt.Wtr.","Volatile Water", "Volatile Water",0),
            new PlayerDstHitMechanic(AquaticBurst, "Aquatic Burst", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.LightBlue), "Aq.Brst.","Aquatic Burst", "Aquatic Burst",0),
            new EnemyDstBuffApplyMechanic(TidalBarrier, "Tidal Barrier", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.LightBlue), "Tdl.Bar.", "Tidal Barrier", "Tidal Barrier", 0),
            new PlayerDstBuffApplyMechanic(TidalBargain, "Tidal Bargain", new MechanicPlotlySetting(Symbols.StarOpen,Colors.LightBlue), "Tdl.Brgn.","Downed by Tidal Bargain", "Tidal Bargain",0),
            new PlayerDstBuffRemoveMechanic(TidalBargain, "Tidal Bargain Downed", new MechanicPlotlySetting(Symbols.Star,Colors.LightBlue), "Tdl.Brgn.Dwn.","Downed by Tidal Bargain", "Tidal Bargain Downed",0).UsingChecker((evt, log) => evt.RemovedStacks == 10 && Math.Abs(evt.RemovedDuration - 90000) < 10 * ServerDelayConstant && log.CombatData.GetBuffData(Downed).Any(x => Math.Abs(x.Time - evt.Time) < 50 && x is BuffApplyEvent bae && bae.To == evt.To)),
            // Dark
            new PlayerDstHitMechanic(new long[] { EmpathicManipulationGuilt, EmpathicManipulation2, EmpathicManipulationSorrow, EmpathicManipulationFear, EmpathicManipulation5, EmpathicManipulation6, EmpathicManipulation7, EmpathicManipulation8, EmpathicManipulation9 }, 
                "Empathic Manipulation", new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "Emp.Mnp.","Empathic Manipulation", "Empathic Manipulation",0),
            new PlayerDstHitMechanic(new long[] { FocusedWrath, FocusedWrath2 }, "Focused Wrath", new MechanicPlotlySetting(Symbols.Circle,Colors.LightPurple), "Fcsd.Wrth.","Focused Wrath", "Focused Wrath",0),
            new PlayerDstHitMechanic(NegativeBurst, "Negative Burst", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.LightPurple), "N.Brst.","Negative Burst", "Negative Burst",500),
            new PlayerDstHitMechanic(Terrorstorm, "Terrorstorm", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.LightPurple), "TrrStrm","Terrorstorm", "Terrorstorm",0),
            new PlayerDstBuffApplyMechanic(CrushingGuilt, "Crushing Guilt", new MechanicPlotlySetting(Symbols.StarOpen,Colors.LightPurple), "Crsh.Glt.","Crushing Guilt", "Crushing Guilt",0),
            new PlayerDstBuffApplyMechanic(new long [] { FixatedFear1, FixatedFear2, FixatedFear3, FixatedFear4 }, "Fixated (Fear)", new MechanicPlotlySetting(Symbols.Bowtie, Colors.Purple), "Fear.Fix.A", "Fixated by Fear", "Fixated Application", 0),
            new PlayerDstBuffRemoveMechanic(CrushingGuilt, "Crushing Guilt Down", new MechanicPlotlySetting(Symbols.Star,Colors.LightPurple), "Crsh.Glt.Dwn.","Downed by Crushing Guilt", "Crushing Guilt Down",0).UsingChecker((evt, log) => evt.RemovedStacks == 10 && Math.Abs(evt.RemovedDuration - 90000) < 10 * ServerDelayConstant && log.CombatData.GetBuffData(Downed).Any(x => Math.Abs(x.Time - evt.Time) < 50 && x is BuffApplyEvent bae && bae.To == evt.To)),
            new EnemyCastStartMechanic(EmpathicManipulationFear, "Empathic Manipulation (Fear)", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.LightPurple), "Fear Mnp.", "Empathic Manipulation (Fear)", "Empathic Manipulation (Fear)", 0),
            new EnemyCastEndMechanic(EmpathicManipulationFear, "Empathic Manipulation (Fear) Interrupt", new MechanicPlotlySetting(Symbols.TriangleUpOpen,Colors.LightPurple), "IntFear.Mnp.", "Empathic Manipulation (Fear) Interrupt", "Empathic Manipulation (Fear) Interrupt", 0).UsingChecker((evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyCastStartMechanic(EmpathicManipulationSorrow, "Empathic Manipulation (Sorrow)", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.LightPurple), "Sor.Mnp.", "Empathic Manipulation (Sorrow)", "Empathic Manipulation (Sorrow)", 0),
            new EnemyCastEndMechanic(EmpathicManipulationSorrow, "Empathic Manipulation (Sorrow) Interrupt", new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.LightPurple), "IntSor.Mnp.", "Empathic Manipulation (Sorrow) Interrupt", "Empathic Manipulation (Sorrow) Interrupt", 0).UsingChecker((evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyCastStartMechanic(EmpathicManipulationGuilt, "Empathic Manipulation (Guilt)", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.LightPurple), "Glt.Mnp.", "Empathic Manipulation (Guilt)", "Empathic Manipulation (Guilt)", 0),
            new EnemyCastEndMechanic(EmpathicManipulationGuilt, "Empathic Manipulation (Guilt) Interrupt", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.LightPurple), "Int.Glt.Mnp.", "Empathic Manipulation (Guilt) Interrupt", "Empathic Manipulation (Guilt) Interrupt", 0).UsingChecker((evt, log) => evt is AnimatedCastEvent ace && ace.Status == AbstractCastEvent.AnimationStatus.Interrupted),
            new EnemyDstBuffApplyMechanic(CacophonousMind, "Cacophonous Mind", new MechanicPlotlySetting(Symbols.Pentagon,Colors.LightPurple), "Ccphns.Mnd.","Cacophonous Mind", "Cacophonous Mind",0),
            });
            Extension = "ai";
            Icon = EncounterIconAi;
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
            return new CombatReplayMap(CombatReplayAi,
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
                (int)TargetID.AiKeeperOfThePeak,
                (int)TargetID.AiKeeperOfThePeak2,
                (int)TrashID.CCSorrowDemon,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            var trashIDs = new List<ArcDPSEnums.TrashID>
            {
                TrashID.FearDemon,
                TrashID.GuiltDemon,
                TrashID.AiDoubtDemon,
                TrashID.PlayerDoubtDemon,
                TrashID.EnragedWaterSprite,
                // Transition sorrow demons
                TrashID.TransitionSorrowDemon1,
                TrashID.TransitionSorrowDemon2,
                TrashID.TransitionSorrowDemon3,
                TrashID.TransitionSorrowDemon4,
            };
            trashIDs.AddRange(base.GetTrashMobsIDs());
            return trashIDs;
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)TargetID.AiKeeperOfThePeak,
                (int)TargetID.AiKeeperOfThePeak2,
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            AgentItem aiAgent = agentData.GetNPCsByID(TargetID.AiKeeperOfThePeak).FirstOrDefault();
            if (aiAgent == null)
            {
                throw new MissingKeyActorsException("Ai not found");
            }
            _china = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.Language && x.SrcAgent == (ulong)LanguageEvent.LanguageEnum.Chinese) != null;
            CombatItem darkModePhaseEvent = combatData.FirstOrDefault(x => x.StartCasting() && x.SrcMatchesAgent(aiAgent) && x.SkillID == AiDarkPhaseEvent && x.SrcMatchesAgent(aiAgent));
            _hasDarkMode = combatData.Exists(x => (_china ? x.SkillID == AiHasDarkModeCN_SurgeOfDarkness : x.SkillID == AiHasDarkMode_SurgeOfDarkness) && x.SrcMatchesAgent(aiAgent));
            _hasElementalMode = !_hasDarkMode || darkModePhaseEvent != null;
            if (_hasDarkMode)
            {
                if (_hasElementalMode)
                {
                    long darkModeStart = combatData.FirstOrDefault(x => x.StartCasting() && x.SrcMatchesAgent(aiAgent) && (_china ? x.SkillID == AiDarkModeStartCN : x.SkillID == AiDarkModeStart) && x.Time >= darkModePhaseEvent.Time && x.SrcMatchesAgent(aiAgent)).Time;
                    CombatItem invul895Loss = combatData.FirstOrDefault(x => x.Time <= darkModeStart && x.SkillID == Determined895 && x.IsBuffRemove == BuffRemove.All && x.SrcMatchesAgent(aiAgent) && x.Value > Determined895Duration);
                    long elementalLastAwareTime = (invul895Loss != null ? invul895Loss.Time : darkModeStart);
                    AgentItem darkAiAgent = agentData.AddCustomNPCAgent(elementalLastAwareTime, aiAgent.LastAware, aiAgent.Name, aiAgent.Spec, TargetID.AiKeeperOfThePeak2, false, aiAgent.Toughness, aiAgent.Healing, aiAgent.Condition, aiAgent.Concentration, aiAgent.HitboxWidth, aiAgent.HitboxHeight);
                    RedirectEventsAndCopyPreviousStates(combatData, extensions, agentData, aiAgent, new List<AgentItem> { aiAgent}, darkAiAgent, false);
                    aiAgent.OverrideAwareTimes(aiAgent.FirstAware, elementalLastAwareTime);
                }
                else
                {
                    Extension = "drkai";
                    aiAgent.OverrideID(TargetID.AiKeeperOfThePeak2);
                    agentData.Refresh();
                }
            }
            else
            {
                Extension = "elai";
            }
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            // Manually set HP and names
            AbstractSingleActor eleAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak));
            AbstractSingleActor darkAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak2));
            darkAi?.OverrideName("Dark Ai");
            eleAi?.OverrideName("Elemental Ai");
            if (_hasDarkMode)
            {
                int sorrowCount = 0;
                foreach (AbstractSingleActor target in Targets)
                {
                    if (target.IsSpecies(TrashID.CCSorrowDemon))
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
            AbstractSingleActor elementalAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak));
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
            AbstractSingleActor darkAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak2));
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
                BuffApplyEvent invul895Gain = log.CombatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == elementalAi.AgentItem && x.AppliedDuration > Determined895Duration).FirstOrDefault();
                long eleStart = Math.Max(elementalAi.FirstAware, log.FightData.FightStart);
                long eleEnd = invul895Gain != null ? invul895Gain.Time : log.FightData.FightEnd;
                if (_hasDarkMode)
                {
                    var elePhase = new PhaseData(eleStart, eleEnd, "Elemental Phase");
                    elePhase.AddTarget(elementalAi);
                    phases.Add(elePhase);
                }
                if (requirePhases)
                {
                    // sub phases
                    string[] eleNames = { "Air", "Fire", "Water" };
                    var elementalPhases = GetPhasesByInvul(log, Determined762, elementalAi, false, true, log.FightData.FightStart, Math.Min(elementalAi.LastAware, log.FightData.FightEnd)).Take(3).ToList();
                    for (int i = 0; i < elementalPhases.Count; i++)
                    {
                        PhaseData phase = elementalPhases[i];
                        phase.Name = eleNames[i];
                        phase.AddTarget(elementalAi);
                        if (i > 0)
                        {
                            // try to use transition skill, fallback to determined loss
                            // long skillId = _china ? 61388 : 61385;
                            long skillID = 61187;
                            IReadOnlyList<AbstractCastEvent> casts = elementalAi.GetCastEvents(log, phase.Start, phase.End);
                            // use last cast since determined is fixed 5s and the transition out (ai flying up) can happen after loss
                            AbstractCastEvent castEvt = casts.LastOrDefault(x => x.SkillId == skillID);
                            if (castEvt != null)
                            {
                                phase.OverrideStart(castEvt.Time);
                            }
                            else
                            {
                                phase.Name += " (Fallback)";
                            }
                        }
                    }
                    phases.AddRange(elementalPhases);
                }
            }
            if (_hasDarkMode)
            {
                BuffApplyEvent invul895Gain = log.CombatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == darkAi.AgentItem && x.AppliedDuration > Determined895Duration).FirstOrDefault();
                long darkStart = Math.Max(darkAi.FirstAware, log.FightData.FightStart);
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
                    long fearToSorrowSkillID = _china ? EmpathicManipulationSorrowCN : EmpathicManipulationSorrow;
                    AbstractCastEvent fearToSorrow = darkAi.GetCastEvents(log, darkStart, darkEnd).FirstOrDefault(x => x.SkillId == fearToSorrowSkillID);
                    if (fearToSorrow != null)
                    {
                        var fearPhase = new PhaseData(darkStart, fearToSorrow.Time, "Fear");
                        fearPhase.AddTarget(darkAi);
                        phases.Add(fearPhase);
                        long sorrowToGuiltSkillID = _china ? EmpathicManipulationGuiltCN : EmpathicManipulationGuilt;
                        AbstractCastEvent sorrowToGuilt = darkAi.GetCastEvents(log, darkStart, darkEnd).FirstOrDefault(x => x.SkillId == sorrowToGuiltSkillID);
                        if (sorrowToGuilt != null)
                        {
                            var sorrowPhase = new PhaseData(fearToSorrow.Time, sorrowToGuilt.Time, "Sorrow");
                            sorrowPhase.AddTarget(darkAi);
                            phases.Add(sorrowPhase);
                            var guiltPhase = new PhaseData(sorrowToGuilt.Time, darkEnd, "Guilt");
                            guiltPhase.AddTarget(darkAi);
                            phases.Add(guiltPhase);
                        }
                        else
                        {
                            var sorrowPhase = new PhaseData(fearToSorrow.Time, darkEnd, "Sorrow");
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
                    BuffApplyEvent invul895Gain = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == Targets[0].AgentItem && x.AppliedDuration > Determined895Duration).FirstOrDefault();
                    if (invul895Gain != null)
                    {
                        fightData.SetSuccess(true, invul895Gain.Time);
                    }
                    break;
                case 3:
                    BuffApplyEvent darkInvul895Gain = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == Targets.FirstOrDefault(y => y.IsSpecies(TargetID.AiKeeperOfThePeak2)).AgentItem && x.AppliedDuration > Determined895Duration).FirstOrDefault();
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

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);

            if (log.FightData.Success && _hasDarkMode && _hasElementalMode)
            {
                IReadOnlyList<AbstractBuffEvent> dwd = log.CombatData.GetBuffData(AchievementEligibilityDancingWithDemons);
                IReadOnlyList<AbstractBuffEvent> energyDispersal = log.CombatData.GetBuffData(AchievementEligibilityEnergyDispersal);
                if (dwd.Any())
                {
                    int counter = 0;
                    foreach (Player p in log.PlayerList)
                    {
                        if (p.HasBuff(log, AchievementEligibilityDancingWithDemons, log.FightData.FightEnd - ServerDelayConstant))
                        {
                            counter++;
                        }
                    }
                    // The achievement requires 5 players alive with the buff, if the instance has only 4 players inside, you cannot get it.
                    if (counter == 5)
                    {
                        InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityDancingWithDemons], 1));
                    }
                }
                if (energyDispersal.Any())
                {
                    foreach (Player p in log.PlayerList)
                    {
                        if (p.HasBuff(log, AchievementEligibilityEnergyDispersal, log.FightData.FightEnd - ServerDelayConstant))
                        {
                            InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityEnergyDispersal], 1));
                            break;
                        }
                    }
                }
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // Tethering Players to Fears
            List<AbstractBuffEvent> fearFixations = GetFilteredList(log.CombatData, new long[] { FixatedFear1, FixatedFear2, FixatedFear3, FixatedFear4 }, p, true, true);
            replay.AddTether(fearFixations, "rgba(255, 0, 255, 0.5)");
        }
    }
}
