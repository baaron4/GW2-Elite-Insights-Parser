using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Parser.ParseEnum.TrashIDS;

namespace GW2EIParser.Logic
{
    public class ConjuredAmalgamate : RaidLogic
    {
        public ConjuredAmalgamate(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(52173, "Pulverize", new MechanicPlotlySetting("square","rgb(255,140,0)"), "Arm Slam","Pulverize (Arm Slam)", "Arm Slam",0),
            new HitOnPlayerMechanic(52173, "Pulverize", new MechanicPlotlySetting("square-open","rgb(255,140,0)"), "Stab.Slam","Pulverize (Arm Slam) while affected by stability", "Stabilized Arm Slam",0,(de, log) => de.To.HasBuff(log, 1122, de.Time)),
            new HitOnPlayerMechanic(52086, "Junk Absorption", new MechanicPlotlySetting("circle-open","rgb(150,0,150)"), "Balls","Junk Absorption (Purple Balls during collect)", "Purple Balls",0),
            new HitOnPlayerMechanic(52878, "Junk Fall", new MechanicPlotlySetting("circle-open","rgb(255,150,0)"), "Junk","Junk Fall (Falling Debris)", "Junk Fall",0),
            new HitOnPlayerMechanic(52120, "Junk Fall", new MechanicPlotlySetting("circle-open","rgb(255,150,0)"), "Junk","Junk Fall (Falling Debris)", "Junk Fall",0),
            new HitOnPlayerMechanic(52161, "Ruptured Ground", new MechanicPlotlySetting("square-open","rgb(0,255,255)"), "Ground","Ruptured Ground (Relics after Junk Wall)", "Ruptured Ground",0, (de,log) => de.Damage > 0),
            new HitOnPlayerMechanic(52656, "Tremor", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Tremor","Tremor (Field adjacent to Arm Slam)", "Near Arm Slam",0, (de,log) => de.Damage > 0),
            new HitOnPlayerMechanic(52150, "Junk Torrent", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Wall","Junk Torrent (Moving Wall)", "Junk Torrent (Wall)",0, (de,log) => de.Damage > 0),
            new PlayerCastStartMechanic(52325, "Conjured Greatsword", new MechanicPlotlySetting("square","rgb(255,0,0)"), "Sword","Conjured Greatsword (Special action sword)", "Sword",0),
            new PlayerCastStartMechanic(52780, "Conjured Protection", new MechanicPlotlySetting("square","rgb(0,255,0)"), "Shield","Conjured Protection (Special action shield)", "Shield",0),
            });
            Extension = "ca";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = "https://i.imgur.com/eLyIWd2.png";
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

        public override void EIEvtcParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // make those into npcs
            List<AgentItem> cas = agentData.GetGadgetsByID((ushort)ParseEnum.TargetIDS.ConjuredAmalgamate);
            List<AgentItem> leftArms = agentData.GetGadgetsByID((ushort)ParseEnum.TargetIDS.CALeftArm);
            List<AgentItem> rightArms = agentData.GetGadgetsByID((ushort)ParseEnum.TargetIDS.CARightArm);
            foreach (AgentItem ca in cas)
            {
                ca.OverrideType(AgentItem.AgentType.NPC);
            }
            foreach (AgentItem leftArm in leftArms)
            {
                leftArm.OverrideType(AgentItem.AgentType.NPC);
            }
            foreach (AgentItem rightArm in rightArms)
            {
                rightArm.OverrideType(AgentItem.AgentType.NPC);
            }
            agentData.Refresh();
            ComputeFightTargets(agentData, combatData);
            AgentItem sword = agentData.AddCustomAgent(fightData.FightStart, fightData.FightEnd, AgentItem.AgentType.Player, "Conjured Sword\0:Conjured Sword\050", "Sword", 0);
            foreach (CombatItem c in combatData)
            {
                if (c.SkillID == 52370 && c.IsStateChange == ParseEnum.StateChange.None && c.IsBuffRemove == ParseEnum.BuffRemove.None &&
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

        public override void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate:
                    List<AbstractBuffEvent> shieldCA = GetFilteredList(log.CombatData, 53003, target, true);
                    int shieldCAStart = 0;
                    foreach (AbstractBuffEvent c in shieldCA)
                    {
                        if (c is BuffApplyEvent)
                        {
                            shieldCAStart = (int)c.Time;
                        }
                        else
                        {
                            int shieldEnd = (int)c.Time;
                            int radius = 500;
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (shieldCAStart, shieldEnd), "rgba(0, 150, 255, 0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (ushort)ParseEnum.TargetIDS.CALeftArm:
                case (ushort)ParseEnum.TargetIDS.CARightArm:
                    break;
                case (ushort)ConjuredGreatsword:
                    break;
                case (ushort)ConjuredShield:
                    List<AbstractBuffEvent> shield = GetFilteredList(log.CombatData, 53003, target, true);
                    int shieldStart = 0;
                    foreach (AbstractBuffEvent c in shield)
                    {
                        if (c is BuffApplyEvent)
                        {
                            shieldStart = (int)c.Time;
                        }
                        else
                        {
                            int shieldEnd = (int)c.Time;
                            int radius = 100;
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (shieldStart, shieldEnd), "rgba(0, 150, 255, 0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                NPC target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate);
                NPC leftArm = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.CALeftArm);
                NPC rightArm = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.CARightArm);
                if (target == null)
                {
                    throw new InvalidOperationException("Target for success by combat exit not found");
                }
                AgentItem zommoros = agentData.GetNPCsByID(21118).LastOrDefault();
                if (zommoros == null)
                {
                    return;
                }
                SpawnEvent npcSpawn = combatData.GetSpawnEvents(zommoros).LastOrDefault();
                AbstractDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.Damage > 0) && (playerAgents.Contains(x.From) || playerAgents.Contains(x.From.Master)));
                if (rightArm != null)
                {
                    AbstractDamageEvent lastDamageTakenArm = combatData.GetDamageTakenData(rightArm.AgentItem).LastOrDefault(x => (x.Damage > 0) && (playerAgents.Contains(x.From) || playerAgents.Contains(x.From.Master)));
                    if (lastDamageTakenArm != null)
                    {
                        lastDamageTaken = lastDamageTaken.Time > lastDamageTakenArm.Time ? lastDamageTaken : lastDamageTakenArm;
                    }
                }
                if (leftArm != null)
                {
                    AbstractDamageEvent lastDamageTakenArm = combatData.GetDamageTakenData(leftArm.AgentItem).LastOrDefault(x => (x.Damage > 0) && (playerAgents.Contains(x.From) || playerAgents.Contains(x.From.Master)));
                    if (lastDamageTakenArm != null)
                    {
                        lastDamageTaken = lastDamageTaken.Time > lastDamageTakenArm.Time ? lastDamageTaken : lastDamageTakenArm;
                    }
                }
                if (npcSpawn != null && lastDamageTaken != null)
                {
                    fightData.SetSuccess(true, lastDamageTaken.Time);
                }
            }
        }

        private List<long> GetTargetableTimes(ParsedLog log, NPC target)
        {
            var attackTargetsAgents = log.CombatData.GetAttackTargetEvents(target.AgentItem).Take(2).ToList(); // 3rd one is weird
            var attackTargets = new List<AgentItem>();
            foreach (AttackTargetEvent c in attackTargetsAgents)
            {
                attackTargets.Add(c.AttackTarget);
            }
            var targetables = new List<long>();
            foreach (AgentItem attackTarget in attackTargets)
            {
                List<TargetableEvent> aux = log.CombatData.GetTargetableEvents(attackTarget);
                targetables.AddRange(aux.Where(x => x.Targetable).Select(x => x.Time));
            }
            return targetables;
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC ca = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate);
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
            NPC leftArm = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.CALeftArm);
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
            NPC rightArm = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.CARightArm);
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

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = p.GetCastLogs(log, 0, log.FightData.FightEnd);
            var shieldCast = cls.Where(x => x.SkillId == 52780).ToList();
            foreach (AbstractCastEvent c in shieldCast)
            {
                int start = (int)c.Time;
                int duration = 10000;
                int radius = 300;
                Point3D shieldNextPos = replay.PolledPositions.FirstOrDefault(x => x.Time >= start);
                Point3D shieldPrevPos = replay.PolledPositions.LastOrDefault(x => x.Time <= start);
                if (shieldNextPos != null || shieldPrevPos != null)
                {
                    replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, start + duration), "rgba(255, 0, 255, 0.1)", new InterpolatedPositionConnector(shieldPrevPos, shieldNextPos, start)));
                    replay.Decorations.Add(new CircleDecoration(false, 0, radius, (start, start + duration), "rgba(255, 0, 255, 0.3)", new InterpolatedPositionConnector(shieldPrevPos, shieldNextPos, start)));
                }
            }
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            NPC target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return combatData.GetBuffData(53075).Count > 0 ? 1 : 0;
        }
    }
}
