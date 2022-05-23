using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class ConjuredAmalgamate : MythwrightGambit
    {
        private readonly bool _cn;
        public ConjuredAmalgamate(int triggerID) : base((int)ArcDPSEnums.TargetID.ConjuredAmalgamate)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(52173, "Pulverize", new MechanicPlotlySetting(Symbols.Square,Colors.LightOrange), "Arm Slam","Pulverize (Arm Slam)", "Arm Slam",0, (de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new HitOnPlayerMechanic(52173, "Pulverize", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightOrange), "Stab.Slam","Pulverize (Arm Slam) while affected by stability", "Stabilized Arm Slam",0,(de, log) => de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new HitOnPlayerMechanic(52086, "Junk Absorption", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Purple), "Balls","Junk Absorption (Purple Balls during collect)", "Purple Balls",0),
            new HitOnPlayerMechanic(new long[] {52878, 52120 }, "Junk Fall", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Pink), "Junk","Junk Fall (Falling Debris)", "Junk Fall",0),
            new HitOnPlayerMechanic(52161, "Ruptured Ground", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Teal), "Ground","Ruptured Ground (Relics after Junk Wall)", "Ruptured Ground",0, (de,log) => de.HealthDamage > 0),
            new HitOnPlayerMechanic(52656, "Tremor", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Tremor","Tremor (Field adjacent to Arm Slam)", "Near Arm Slam",0, (de,log) => de.HealthDamage > 0),
            new HitOnPlayerMechanic(52150, "Junk Torrent", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Wall","Junk Torrent (Moving Wall)", "Junk Torrent (Wall)",0, (de,log) => de.HealthDamage > 0),
            new PlayerCastStartMechanic(52325, "Conjured Slash", new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Sword.Cst","Conjured Slash (Special action sword)", "Sword Cast",0),
            new PlayerCastStartMechanic(52780, "Conjured Protection", new MechanicPlotlySetting(Symbols.Square,Colors.Green), "Shield.Cst","Conjured Protection (Special action shield)", "Shield Cast",0),
            new PlayerBuffApplyMechanic(GreatswordPower, "Greatsword Power", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Sword.C","Collected Sword", "Sword Collect",50),
            new PlayerBuffApplyMechanic(ConjuredShield, "Conjured Shield", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Green), "Shield.C","Collected Shield", "Shield Collect",50),
            new EnemyBuffApplyMechanic(AugmentedPower, "Augmented Power", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Red), "Augmented Power","Augmented Power", "Augmented Power",50),
            new EnemyBuffApplyMechanic(Shielded, "Shielded", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Green), "Shielded","Shielded", "Shielded",50),
            });
            _cn = triggerID != (int)ArcDPSEnums.TargetID.ConjuredAmalgamate;
            Extension = "ca";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = "https://i.imgur.com/eLyIWd2.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/lgzr1xD.png",
                            (544, 1000),
                            (-5064, -15030, -2864, -10830)/*,
                            (-21504, -21504, 24576, 24576),
                            (13440, 14336, 15360, 16256)*/);
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.ConjuredAmalgamate,
                (int)ArcDPSEnums.TargetID.CARightArm,
                (int)ArcDPSEnums.TargetID.CALeftArm
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.ConjuredGreatsword,
                ArcDPSEnums.TrashID.ConjuredShield
            };
        }

        protected override List<int> GetFriendlyNPCIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TrashID.ConjuredPlayerSword
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // make those into npcs
            IReadOnlyList<AgentItem> cas = agentData.GetGadgetsByID(_cn ? (int)ArcDPSEnums.TargetID.ConjuredAmalgamate_CHINA : (int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
            if (!cas.Any())
            {
                throw new MissingKeyActorsException("Conjured Amalgamate not found");
            }
            IReadOnlyList<AgentItem> leftArms = agentData.GetGadgetsByID(_cn ? (int)ArcDPSEnums.TargetID.CALeftArm_CHINA : (int)ArcDPSEnums.TargetID.CALeftArm);
            IReadOnlyList<AgentItem> rightArms = agentData.GetGadgetsByID(_cn ? (int)ArcDPSEnums.TargetID.CARightArm_CHINA : (int)ArcDPSEnums.TargetID.CARightArm);
            foreach (AgentItem ca in cas)
            {
                ca.OverrideType(AgentItem.AgentType.NPC);
                ca.OverrideID(ArcDPSEnums.TargetID.ConjuredAmalgamate);
            }
            foreach (AgentItem leftArm in leftArms)
            {
                leftArm.OverrideType(AgentItem.AgentType.NPC);
                leftArm.OverrideID(ArcDPSEnums.TargetID.CALeftArm);
            }
            foreach (AgentItem rightArm in rightArms)
            {
                rightArm.OverrideType(AgentItem.AgentType.NPC);
                rightArm.OverrideID(ArcDPSEnums.TargetID.CARightArm);
            }
            agentData.Refresh();
            AgentItem sword = agentData.AddCustomNPCAgent(0, fightData.FightEnd, "Conjured Sword\0:Conjured Sword\051", ParserHelper.Spec.NPC, (int)ArcDPSEnums.TrashID.ConjuredPlayerSword, true);
            ComputeFightTargets(agentData, combatData, extensions);
            foreach (CombatItem c in combatData)
            {
                if (c.IsDamage(extensions) && c.SkillID == 52370)
                {
                    c.OverrideSrcAgent(sword.Agent);
                }
            }
        }
        /*internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            // Greatsword Power
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 52667);
            // Conjured Shield
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 52754);
            return res;
        }*/

        protected override HashSet<int> GetUniqueNPCIDs()
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
                    List<AbstractBuffEvent> shieldCA = GetFilteredList(log.CombatData, Shielded, target, true, true);
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
                    List<AbstractBuffEvent> shield = GetFilteredList(log.CombatData, Shielded, target, true, true);
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

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
                AbstractSingleActor leftArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CALeftArm);
                AbstractSingleActor rightArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CARightArm);
                if (target == null)
                {
                    throw new MissingKeyActorsException("Conjured Amalgamate not found");
                }
                AgentItem zommoros = agentData.GetNPCsByID(21118).LastOrDefault();
                if (zommoros == null)
                {
                    return;
                }
                SpawnEvent npcSpawn = combatData.GetSpawnEvents(zommoros).LastOrDefault();
                AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                if (lastDamageTaken == null)
                {
                    return;
                }
                if (rightArm != null)
                {
                    AbstractHealthDamageEvent lastDamageTakenArm = combatData.GetDamageTakenData(rightArm.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTakenArm != null)
                    {
                        lastDamageTaken = lastDamageTaken.Time > lastDamageTakenArm.Time ? lastDamageTaken : lastDamageTakenArm;
                    }
                }
                if (leftArm != null)
                {
                    AbstractHealthDamageEvent lastDamageTakenArm = combatData.GetDamageTakenData(leftArm.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTakenArm != null)
                    {
                        lastDamageTaken = lastDamageTaken.Time > lastDamageTakenArm.Time ? lastDamageTaken : lastDamageTakenArm;
                    }
                }
                if (npcSpawn != null)
                {
                    fightData.SetSuccess(true, lastDamageTaken.Time);
                }
            }
        }

        private static List<long> GetTargetableTimes(ParsedEvtcLog log, AbstractSingleActor target)
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
            AbstractSingleActor ca = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
            if (ca == null)
            {
                throw new MissingKeyActorsException("Conjured Amalgamate not found");
            }
            phases[0].AddTarget(ca);
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
                    phase.AddTarget(ca);
                }
                phase.Name = name;
            }
            AbstractSingleActor leftArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CALeftArm);
            if (leftArm != null)
            {
                List<long> targetables = GetTargetableTimes(log, leftArm);
                for (int i = 1; i < phases.Count; i += 2)
                {
                    PhaseData phase = phases[i];
                    if (targetables.Exists(x => phase.InInterval(x)))
                    {
                        phase.Name = "Left " + phase.Name;
                        phase.AddTarget(leftArm);
                    }
                }
            }
            AbstractSingleActor rightArm = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.CARightArm);
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
                        phase.AddTarget(rightArm);
                    }
                }
            }
            return phases;
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = p.GetCastEvents(log, 0, log.FightData.FightEnd);
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
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.ConjuredAmalgamate);
            if (target == null)
            {
                throw new MissingKeyActorsException("Conjured Amalgamate not found");
            }
            return combatData.GetBuffData(LockedOn).Count > 0 ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
