using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class ConjuredAmalgamate : RaidLogic
    {
        public ConjuredAmalgamate(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new SkillOnPlayerMechanic(52173, "Pulverize", new MechanicPlotlySetting("square","rgb(255,140,0)"), "Arm Slam","Pulverize (Arm Slam)", "Arm Slam",0),
            new SkillOnPlayerMechanic(52086, "Junk Absorption", new MechanicPlotlySetting("circle-open","rgb(150,0,150)"), "Balls","Junk Absorption (Purple Balls during collect)", "Purple Balls",0),
            new SkillOnPlayerMechanic(52878, "Junk Fall", new MechanicPlotlySetting("circle-open","rgb(255,150,0)"), "Junk","Junk Fall (Falling Debris)", "Junk Fall",0),
            new SkillOnPlayerMechanic(52120, "Junk Fall", new MechanicPlotlySetting("circle-open","rgb(255,150,0)"), "Junk","Junk Fall (Falling Debris)", "Junk Fall",0),
            new SkillOnPlayerMechanic(52161, "Ruptured Ground", new MechanicPlotlySetting("square-open","rgb(0,255,255)"), "Ground","Ruptured Ground (Relics after Junk Wall)", "Ruptured Ground",0,new List<MechanicChecker>{ new CombatItemValueChecker(0, MechanicChecker.ValueCompare.G) }, Mechanic.TriggerRule.AND),
            new SkillOnPlayerMechanic(52656, "Tremor", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Tremor","Tremor (Field adjacent to Arm Slam)", "Near Arm Slam",0,new List<MechanicChecker>{ new CombatItemValueChecker(0, MechanicChecker.ValueCompare.G) }, Mechanic.TriggerRule.AND),
            new SkillOnPlayerMechanic(52150, "Junk Torrent", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Wall","Junk Torrent (Moving Wall)", "Junk Torrent (Wall)",0,new List<MechanicChecker>{ new CombatItemValueChecker(0, MechanicChecker.ValueCompare.G) }, Mechanic.TriggerRule.AND),
            }); 
            Extension = "ca";
            IconUrl = "https://i.imgur.com/eLyIWd2.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/9PJB5Ky.png",
                            (1414, 2601),
                            (-5064, -15030, -2864, -10830),
                            (-21504, -21504, 24576, 24576),
                            (13440, 14336, 15360, 16256));
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate,
                (ushort)ParseEnum.TargetIDS.CARightArm,
                (ushort)ParseEnum.TargetIDS.CALeftArm
            };
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>()
            {
                ConjuredGreatsword,
                ConjuredShield
            };
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            AgentItem sword = agentData.AddCustomAgent(combatData.First().Time, combatData.Last().Time, AgentItem.AgentType.Player, "Conjured Sword\0:Conjured Sword\050", "Sword", 0);
            foreach(CombatItem c in combatData)
            {
                if (c.SkillID == 52370 && c.IsStateChange == ParseEnum.StateChange.Normal && c.IsBuffRemove == ParseEnum.BuffRemove.None &&
                                        ((c.IsBuff == 1 && c.BuffDmg >= 0 && c.Value == 0) ||
                                        (c.IsBuff == 0 && c.Value >= 0)) && c.DstInstid != 0 && c.IFF == ParseEnum.IFF.Foe)
                {
                    c.OverrideSrcValues(sword.Agent, sword.InstID, 0);
                }
            }
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate,
                (ushort)ParseEnum.TargetIDS.CALeftArm,
                (ushort)ParseEnum.TargetIDS.CARightArm
            };
        }

        public override void ComputeMobCombatReplayActors(Mob mob, ParsedLog log, CombatReplay replay)
        {
            switch (mob.ID)
            {
                case (ushort)ConjuredGreatsword:
                    break;
                case (ushort)ConjuredShield:
                    List<CombatItem> shield = GetFilteredList(log.CombatData, 53003, mob, true);
                    int shieldStart = 0;
                    foreach (CombatItem c in shield)
                    {
                        if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                        {
                            shieldStart = (int)(log.FightData.ToFightSpace(c.Time));
                        }
                        else
                        {
                            int shieldEnd = (int)(log.FightData.ToFightSpace(c.Time));
                            int radius = 100;
                            replay.Actors.Add(new CircleActor(true, 0, radius, (shieldStart, shieldEnd), "rgba(0, 150, 255, 0.3)", new AgentConnector(mob)));
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        private List<long> GetTargetableTimes(ParsedLog log, Target target)
        {
            List<CombatItem> attackTargetsAgents = log.CombatData.GetStates(ParseEnum.StateChange.AttackTarget).Where(x => x.DstAgent == target.Agent).Take(2).ToList(); // 3rd one is weird
            List<AgentItem> attackTargets = new List<AgentItem>();
            foreach (CombatItem c in attackTargetsAgents)
            {
                attackTargets.Add(log.AgentData.GetAgent(c.SrcAgent, c.Time));
            }
            List<long> targetables = new List<long>();
            foreach (AgentItem attackTarget in attackTargets)
            {
                var aux = log.CombatData.GetStates(ParseEnum.StateChange.Targetable).Where(x => x.SrcAgent == attackTarget.Agent).ToList();
                targetables.AddRange(aux.Where(x => x.DstAgent == 1).Select(x => log.FightData.ToFightSpace(x.Time)));
            }
            return targetables;
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target ca = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate);
            if (ca == null)
            {
                throw new InvalidOperationException("Conjurate Amalgamate not found");
            }
            phases[0].Targets.Add(ca);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 52255, ca, true, false));
            phases.RemoveAll(x => x.DurationInMS < 1000);
            for (int i = 1; i < phases.Count; i++)
            {
                string name;
                PhaseData phase = phases[i];
                if (i % 2 == 1)
                {
                    name = "Arm Phase";
                }
                else
                {
                    name = "Burn Phase";
                    phase.Targets.Add(ca);
                }
                phase.Name = name;
            }
            Target leftArm = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.CALeftArm);
            if (leftArm != null)
            {
                List<long> targetables = GetTargetableTimes(log, leftArm);
                for (int i = 1; i < phases.Count; i += 2)
                {
                    PhaseData phase = phases[i];
                    if (targetables.Exists(x => phase.InInterval(x)))
                    {
                        phase.Name = "Left " + phase.Name;
                        phase.Targets.Add(leftArm);
                    }
                }
            }
            Target rightArm = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.CARightArm);
            if (rightArm != null)
            {
                List<long> targetables = GetTargetableTimes(log, rightArm);
                for (int i = 1; i < phases.Count; i += 2)
                {
                    PhaseData phase = phases[i];
                    if (targetables.Exists(x => phase.InInterval(x)))
                    {
                        if (phase.Name.Contains("Left"))
                        {
                            phase.Name = "Both Arms Phase";
                        }
                        else
                        {
                            phase.Name = "Right " + phase.Name;
                        }
                        phase.Targets.Add(rightArm);
                    }
                }
            }
            return phases;
        }

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);

            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate:
                    List<CombatItem> shield = GetFilteredList(log.CombatData, 53003, target, true);
                    int shieldStart = 0;
                    foreach (CombatItem c in shield)
                    {
                        if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                        {
                            shieldStart = (int)(log.FightData.ToFightSpace(c.Time));
                        }
                        else
                        {
                            int shieldEnd = (int)(log.FightData.ToFightSpace(c.Time));
                            int radius = 500;
                            replay.Actors.Add(new CircleActor(true, 0, radius, (shieldStart, shieldEnd), "rgba(0, 150, 255, 0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (ushort)ParseEnum.TargetIDS.CALeftArm:
                case (ushort)ParseEnum.TargetIDS.CARightArm:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = p.GetCastLogs(log, 0, log.FightData.FightDuration);
            List<AbstractCastEvent> shieldCast = cls.Where(x => x.SkillId == 52780).ToList();
            foreach (AbstractCastEvent c in shieldCast)
            {
                int start = (int)c.Time;
                int duration = 10000;
                int radius = 300;
                Point3D shieldNextPos = replay.Positions.FirstOrDefault(x => x.Time >= start);
                Point3D shieldPrevPos = replay.Positions.LastOrDefault(x => x.Time <= start);
                if (shieldNextPos != null || shieldPrevPos != null)
                {
                    replay.Actors.Add(new CircleActor(true, 0, radius, (start, start + duration), "rgba(255, 0, 255, 0.1)", new InterpolatedPositionConnector(shieldPrevPos, shieldNextPos, start)));
                    replay.Actors.Add(new CircleActor(false, 0, radius, (start, start + duration), "rgba(255, 0, 255, 0.3)", new InterpolatedPositionConnector(shieldPrevPos, shieldNextPos, start)));
                }
            }
        }

        public override int IsCM(ParsedEvtcContainer evtcContainer)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return evtcContainer.CombatData.GetBoonData(53075).Count > 0 ? 1 : 0;
        }
    }
}
