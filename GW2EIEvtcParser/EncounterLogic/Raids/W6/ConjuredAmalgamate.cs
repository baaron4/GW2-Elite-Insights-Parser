using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class ConjuredAmalgamate : RaidLogic
    {
        public ConjuredAmalgamate(int triggerID) : base(triggerID)
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
            new PlayerCastStartMechanic(52325, "Conjured Slash", new MechanicPlotlySetting("square","rgb(255,0,0)"), "Sword.Cst","Conjured Slash (Special action sword)", "Sword Cast",0),
            new PlayerCastStartMechanic(52780, "Conjured Protection", new MechanicPlotlySetting("square","rgb(0,255,0)"), "Shield.Cst","Conjured Protection (Special action shield)", "Shield Cast",0),
            new PlayerBuffApplyMechanic(52667, "Greatsword Power", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Sword.C","Collected Sword", "Sword Collect",50),
            new PlayerBuffApplyMechanic(52754, "Conjured Shield", new MechanicPlotlySetting("diamond-tall","rgb(0,255,0)"), "Shield.C","Collected Shield", "Shield Collect",50),
            new EnemyBuffApplyMechanic(52074, "Augmented Power", new MechanicPlotlySetting("asterisk-open","rgb(255,0,0)"), "Augmented Power","Augmented Power", "Augmented Power",50),
            new EnemyBuffApplyMechanic(53003, "Shielded", new MechanicPlotlySetting("asterisk-open","rgb(0,255,0)"), "Shielded","Shielded", "Shielded",50),
            });
            Extension = "ca";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = "https://i.imgur.com/eLyIWd2.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/9PJB5Ky.png",
                            (1414, 2601),
                            (-5064, -15030, -2864, -10830),
                            (-21504, -21504, 24576, 24576),
                            (13440, 14336, 15360, 16256));
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.ConjuredAmalgamate,
                (int)ArcDPSEnums.TargetID.CARightArm,
                (int)ArcDPSEnums.TargetID.CALeftArm
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.ConjuredGreatsword,
                ArcDPSEnums.TrashID.ConjuredShield
            };
        }

        internal override void EIEvtcParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, List<Player> playerList)
        {
            // make those into npcs
            IReadOnlyList<AgentItem> cas = agentData.GetGadgetsByID((int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
            IReadOnlyList<AgentItem> leftArms = agentData.GetGadgetsByID((int)ArcDPSEnums.TargetID.CALeftArm);
            IReadOnlyList<AgentItem> rightArms = agentData.GetGadgetsByID((int)ArcDPSEnums.TargetID.CARightArm);
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
            AgentItem sword = agentData.AddCustomAgent(fightData.FightStart, fightData.FightEnd, AgentItem.AgentType.Player, "Conjured Sword\0:Conjured Sword\051", "Sword", 0);
            playerList.Add(new Player(sword, false, true));
            foreach (CombatItem c in combatData)
            {
                if (c.SkillID == 52370 && c.IsStateChange == ArcDPSEnums.StateChange.None && c.IsBuffRemove == ArcDPSEnums.BuffRemove.None &&
                                        ((c.IsBuff == 1 && c.BuffDmg >= 0 && c.Value == 0) ||
                                        (c.IsBuff == 0 && c.Value >= 0)) && c.DstInstid != 0 && c.IFF == ArcDPSEnums.IFF.Foe)
                {
                    c.OverrideSrcAgent(sword.Agent);
                }
            }
        }
        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            // Greatsword Power
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 52667);
            // Conjured Shield
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 52754);
            return res;
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.ConjuredAmalgamate,
                (int)ArcDPSEnums.TargetID.CALeftArm,
                (int)ArcDPSEnums.TargetID.CARightArm
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.ConjuredAmalgamate:
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
                case (int)ArcDPSEnums.TargetID.CALeftArm:
                case (int)ArcDPSEnums.TargetID.CARightArm:
                    break;
                case (int)ArcDPSEnums.TrashID.ConjuredGreatsword:
                    break;
                case (int)ArcDPSEnums.TrashID.ConjuredShield:
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

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                NPC target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
                NPC leftArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CALeftArm);
                NPC rightArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CARightArm);
                if (target == null)
                {
                    throw new InvalidOperationException("Conjured Amalgamate not found");
                }
                AgentItem zommoros = agentData.GetNPCsByID(21118).LastOrDefault();
                if (zommoros == null)
                {
                    return;
                }
                SpawnEvent npcSpawn = combatData.GetSpawnEvents(zommoros).LastOrDefault();
                AbstractDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.Damage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                if (rightArm != null)
                {
                    AbstractDamageEvent lastDamageTakenArm = combatData.GetDamageTakenData(rightArm.AgentItem).LastOrDefault(x => (x.Damage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTakenArm != null)
                    {
                        lastDamageTaken = lastDamageTaken.Time > lastDamageTakenArm.Time ? lastDamageTaken : lastDamageTakenArm;
                    }
                }
                if (leftArm != null)
                {
                    AbstractDamageEvent lastDamageTakenArm = combatData.GetDamageTakenData(leftArm.AgentItem).LastOrDefault(x => (x.Damage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
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

        private static List<long> GetTargetableTimes(ParsedEvtcLog log, NPC target)
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
                IReadOnlyList<TargetableEvent> aux = log.CombatData.GetTargetableEvents(attackTarget);
                targetables.AddRange(aux.Where(x => x.Targetable).Select(x => x.Time));
            }
            return targetables;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC ca = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
            if (ca == null)
            {
                throw new InvalidOperationException("Conjured Amalgamate not found");
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
            NPC leftArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CALeftArm);
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
            NPC rightArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CARightArm);
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

        internal override void ComputePlayerCombatReplayActors(Player p, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = p.GetCastLogs(log, 0, log.FightData.FightEnd);
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

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            NPC target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
            if (target == null)
            {
                throw new InvalidOperationException("Conjured Amalgamate not found");
            }
            return combatData.GetBuffData(53075).Count > 0 ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
