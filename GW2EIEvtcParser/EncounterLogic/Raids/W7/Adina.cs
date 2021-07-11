using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Adina : TheKeyOfAhdashim
    {
        public Adina(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new PlayerBuffApplyMechanic(56593, "Radiant Blindness", new MechanicPlotlySetting("circle",Colors.Magenta), "R.Blind", "Unremovable blindness", "Radiant Blindness", 0),
                new PlayerBuffApplyMechanic(56440, "Eroding Curse", new MechanicPlotlySetting("square",Colors.LightPurple), "Curse", "Stacking damage debuff from Hand of Erosion", "Eroding Curse", 0),
                new HitOnPlayerMechanic(56648, "Boulder Barrage", new MechanicPlotlySetting("hexagon",Colors.Red), "Boulder", "Hit by boulder thrown during pillars", "Boulder Barrage", 0),
                new HitOnPlayerMechanic(56390, "Perilous Pulse", new MechanicPlotlySetting("triangle-right",Colors.Pink), "Perilous Pulse", "Perilous Pulse", "Perilous Pulse", 0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
                new HitOnPlayerMechanic(56141, "Stalagmites", new MechanicPlotlySetting("pentagon",Colors.Red), "Mines", "Hit by mines", "Mines", 0),
                new HitOnPlayerMechanic(56114, "Diamond Palisade", new MechanicPlotlySetting("star-diamond",Colors.Pink), "Eye", "Looked at Eye", "Looked at Eye", 0),
                new SkillOnPlayerMechanic(56035, "Quantum Quake", new MechanicPlotlySetting("hourglass",Colors.Brown), "S.Thrower", "Hit by rotating SandThrower", "SandThrower", 0, (de, log) => de.HasKilled),
                new SkillOnPlayerMechanic(56381, "Quantum Quake", new MechanicPlotlySetting("hourglass",Colors.Brown), "S.Thrower", "Hit by rotating SandThrower", "SandThrower", 0, (de, log) => de.HasKilled),
            });
            Extension = "adina";
            Icon = "https://wiki.guildwars2.com/images/a/a0/Mini_Earth_Djinn.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(56351, 56351, InstantCastFinder.DefaultICD), // Seismic Suffering
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, List<AbstractSingleActor> friendlies)
        {
            var attackTargets = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.AttackTarget).ToList();
            long first = 0;
            long final = fightData.FightEnd;
            foreach (CombatItem at in attackTargets)
            {
                AgentItem hand = agentData.GetAgent(at.DstAgent, at.Time);
                AgentItem atAgent = agentData.GetAgent(at.SrcAgent, at.Time);
                var attackables = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Targetable && x.SrcMatchesAgent(atAgent)).ToList();
                var attackOn = attackables.Where(x => x.DstAgent == 1 && x.Time >= first + 2000).Select(x => x.Time).ToList();
                var attackOff = attackables.Where(x => x.DstAgent == 0 && x.Time >= first + 2000).Select(x => x.Time).ToList();
                var posFacingHP = combatData.Where(x => x.SrcMatchesAgent(hand) && (x.IsStateChange == ArcDPSEnums.StateChange.Position || x.IsStateChange == ArcDPSEnums.StateChange.Rotation || x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)).ToList();
                CombatItem pos = posFacingHP.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.Position);
                int id = (int)ArcDPSEnums.TrashID.HandOfErosion;
                if (pos != null)
                {
                    (float x, float y, _) = AbstractMovementEvent.UnpackMovementData(pos.DstAgent, 0);
                    if ((Math.Abs(x - 15570.5) < 10 && Math.Abs(y + 693.117) < 10) ||
                            (Math.Abs(x - 14277.2) < 10 && Math.Abs(y + 2202.52) < 10))
                    {
                        id = (int)ArcDPSEnums.TrashID.HandOfEruption;
                    }
                }
                for (int i = 0; i < attackOn.Count; i++)
                {
                    long start = attackOn[i];
                    long end = final;
                    if (i <= attackOff.Count - 1)
                    {
                        end = attackOff[i];
                    }
                    AgentItem extra = agentData.AddCustomAgent(start, end, AgentItem.AgentType.NPC, hand.Name, hand.Prof, id, false, hand.Toughness, hand.Healing, hand.Condition, hand.Concentration, hand.HitboxWidth, hand.HitboxHeight);
                    foreach (CombatItem c in combatData)
                    {
                        if (c.Time >= extra.FirstAware && c.Time <= extra.LastAware)
                        {
                            if (c.SrcMatchesAgent(hand))
                            {
                                c.OverrideSrcAgent(extra.Agent);
                            }
                            if (c.DstMatchesAgent(hand))
                            {
                                c.OverrideDstAgent(extra.Agent);
                            }
                        }
                    }
                    foreach (CombatItem c in posFacingHP)
                    {
                        var cExtra = new CombatItem(c);
                        cExtra.OverrideTime(extra.FirstAware);
                        cExtra.OverrideSrcAgent(extra.Agent);
                        combatData.Add(cExtra);
                    }
                }
            }
            var auxCombatData = combatData.OrderBy(x => x.Time).ToList();
            combatData.Clear();
            combatData.AddRange(auxCombatData);
            ComputeFightTargets(agentData, combatData);
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Adina,
                (int)ArcDPSEnums.TrashID.HandOfErosion,
                (int)ArcDPSEnums.TrashID.HandOfEruption
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Adina);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Adina not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            List<AbstractBuffEvent> invuls = GetFilteredList(log.CombatData, 762, mainTarget, true);
            long start = 0, end = 0;
            for (int i = 0; i < invuls.Count; i++)
            {
                AbstractBuffEvent be = invuls[i];
                if (be is BuffApplyEvent)
                {
                    start = be.Time;
                    if (i == invuls.Count - 1)
                    {
                        phases.Add(new PhaseData(start, log.FightData.FightEnd, "Split " + (i / 2 + 1)));
                    }
                }
                else
                {
                    end = be.Time;
                    phases.Add(new PhaseData(start, end, "Split " + (i / 2 + 1)));
                }
            }
            var mainPhases = new List<PhaseData>();
            var quantumQuakes = mainTarget.GetCastEvents(log, 0, log.FightData.FightEnd).Where(x => x.SkillId == 56035 || x.SkillId == 56381).ToList();
            AbstractCastEvent boulderBarrage = mainTarget.GetCastEvents(log, 0, log.FightData.FightEnd).FirstOrDefault(x => x.SkillId == 56648 && x.Time < 6000);
            start = boulderBarrage == null ? 0 : boulderBarrage.EndTime;
            end = 0;
            if (phases.Count > 1)
            {
                for (int i = 1; i < phases.Count; i++)
                {
                    AbstractCastEvent qQ = quantumQuakes[i - 1];
                    end = qQ.Time;
                    mainPhases.Add(new PhaseData(start, end, "Phase " + i));
                    PhaseData split = phases[i];
                    AddTargetsToPhaseAndFit(split, new List<int> { (int)ArcDPSEnums.TrashID.HandOfErosion, (int)ArcDPSEnums.TrashID.HandOfEruption }, log);
                    start = split.End;
                    if (i == phases.Count - 1 && start != log.FightData.FightEnd)
                    {
                        mainPhases.Add(new PhaseData(start, log.FightData.FightEnd, "Phase " + (i + 1)));
                    }
                }
            }
            else if (start > 0)
            {
                // no split
                mainPhases.Add(new PhaseData(start, log.FightData.FightEnd, "Phase 1"));
            }

            foreach (PhaseData phase in mainPhases)
            {
                phase.AddTarget(mainTarget);
            }
            phases.AddRange(mainPhases);
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            GetCombatMap(log).MatchMapsToPhases(new List<string> {
                "https://i.imgur.com/IQn2RJV.png",
                "https://i.imgur.com/gJ55jKy.png",
                "https://i.imgur.com/3pO7eCB.png",
                "https://i.imgur.com/c2Oz5bj.png",
                "https://i.imgur.com/ZFw590w.png",
                "https://i.imgur.com/P4SGbrc.png",
                "https://i.imgur.com/2P7UE8q.png"
            }, phases, log.FightData.FightEnd);
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/IQn2RJV.png",
                            (866, 1000),
                            (13840, -2698, 15971, -248)/*,
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970)*/);
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Adina);
            if (target == null)
            {
                throw new MissingKeyActorsException("Adina not found");
            }
            return (target.GetHealth(combatData) > 23e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
