using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class ConjuredAmalgamate : MythwrightGambit
    {
        private readonly bool _cn;
        private static readonly Point3D CAChestPosition = new Point3D(-4594f, -13004f, -2063.04f);
        public ConjuredAmalgamate(int triggerID) : base((int)ArcDPSEnums.TargetID.ConjuredAmalgamate)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(Pulverize, "Pulverize", new MechanicPlotlySetting(Symbols.Square,Colors.LightOrange), "Arm Slam","Pulverize (Arm Slam)", "Arm Slam",0).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstHitMechanic(Pulverize, "Pulverize", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightOrange), "Stab.Slam","Pulverize (Arm Slam) while affected by stability", "Stabilized Arm Slam",0).UsingChecker((de, log) => de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstHitMechanic(JunkAbsorption, "Junk Absorption", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Purple), "Balls","Junk Absorption (Purple Balls during collect)", "Purple Balls",0),
            new PlayerDstHitMechanic(new long[] {JunkFall1, JunkFall2 }, "Junk Fall", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Pink), "Junk","Junk Fall (Falling Debris)", "Junk Fall",0),
            new PlayerDstHitMechanic(RupturedGround, "Ruptured Ground", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Teal), "Ground","Ruptured Ground (Relics after Junk Wall)", "Ruptured Ground",0).UsingChecker((de,log) => de.HealthDamage > 0),
            new PlayerDstHitMechanic(Tremor, "Tremor", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Tremor","Tremor (Field adjacent to Arm Slam)", "Near Arm Slam",0).UsingChecker((de,log) => de.HealthDamage > 0),
            new PlayerDstHitMechanic(JunkTorrent, "Junk Torrent", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Wall","Junk Torrent (Moving Wall)", "Junk Torrent (Wall)",0).UsingChecker((de,log) => de.HealthDamage > 0),
            new PlayerCastStartMechanic(ConjuredSlashSAK, "Conjured Slash", new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Sword.Cst","Conjured Slash (Special action sword)", "Sword Cast",0),
            new PlayerCastStartMechanic(ConjuredProtectionSAK, "Conjured Protection", new MechanicPlotlySetting(Symbols.Square,Colors.Green), "Shield.Cst","Conjured Protection (Special action shield)", "Shield Cast",0),
            new PlayerDstBuffApplyMechanic(GreatswordPower, "Greatsword Power", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Sword.C","Collected Sword", "Sword Collect",50),
            new PlayerDstBuffApplyMechanic(ConjuredShield, "Conjured Shield", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Green), "Shield.C","Collected Shield", "Shield Collect",50),
            new EnemyDstBuffApplyMechanic(AugmentedPower, "Augmented Power", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Red), "Augmented Power","Augmented Power", "Augmented Power",50),
            new EnemyDstBuffApplyMechanic(Shielded, "Shielded", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Green), "Shielded","Shielded", "Shielded",50),
            });
            _cn = triggerID != (int)ArcDPSEnums.TargetID.ConjuredAmalgamate;
            Extension = "ca";
            GenericFallBackMethod = FallBackMethod.ChestGadget;
            Icon = EncounterIconConjuredAmalgamate;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
            ChestID = ArcDPSEnums.ChestID.CAChest;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayConjuredAmalgamate,
                            (544, 1000),
                            (-5064, -15030, -2864, -10830)/*,
                            (-21504, -21504, 24576, 24576),
                            (13440, 14336, 15360, 16256)*/);
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // time starts at first smash
            if (combatData.Any(x => x.IsStateChange == ArcDPSEnums.StateChange.EffectIDToGUID))
            {
                CombatItem armSmashGUID = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.EffectIDToGUID).FirstOrDefault(x => IDToGUIDEvent.UnpackGUID(x.SrcAgent, x.DstAgent) == EffectGUIDs.CAArmSmash);
                if (armSmashGUID != null)
                {
                    CombatItem firstArmSmash = combatData.FirstOrDefault(x => x.IsEffect && x.SkillID == armSmashGUID.SkillID);
                    if (firstArmSmash != null)
                    {
                        CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
                        if (logStartNPCUpdate != null)
                        {
                            // we couldn't have hit CA before the initial smash
                            return firstArmSmash.Time > GetPostLogStartNPCUpdateDamageEventTime(fightData, agentData, combatData, logStartNPCUpdate.Time, agentData.GetGadgetsByID(_cn ? ArcDPSEnums.TargetID.ConjuredAmalgamate_CHINA : ArcDPSEnums.TargetID.ConjuredAmalgamate).FirstOrDefault()) ? logStartNPCUpdate.Time : firstArmSmash.Time;
                        } 
                        else
                        {
                            // Before new logging, log would start when everyone in combat + boss in combat or enters combat
                            // as such the first smash can only happen within the first few seconds of the start
                            return firstArmSmash.Time - fightData.LogStart > 6000 ? GetGenericFightOffset(fightData) : firstArmSmash.Time ;
                        }
                    }
                }
            }
            return GetGenericFightOffset(fightData);
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            // Can be improved
            if (_cn)
            {
                if (TargetHPPercentUnderThreshold(ArcDPSEnums.TargetID.ConjuredAmalgamate_CHINA, fightData.FightStart, combatData, Targets))
                {
                    return FightData.EncounterStartStatus.Late;
                }
            } 
            else
            {
                if (TargetHPPercentUnderThreshold(ArcDPSEnums.TargetID.ConjuredAmalgamate, fightData.FightStart, combatData, Targets))
                {
                    return FightData.EncounterStartStatus.Late;
                }
            }
            return FightData.EncounterStartStatus.Normal;
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
            IReadOnlyList<AgentItem> cas = agentData.GetGadgetsByID(_cn ? ArcDPSEnums.TargetID.ConjuredAmalgamate_CHINA : ArcDPSEnums.TargetID.ConjuredAmalgamate);
            if (!cas.Any())
            {
                throw new MissingKeyActorsException("Conjured Amalgamate not found");
            }
            IReadOnlyList<AgentItem> leftArms = agentData.GetGadgetsByID(_cn ? ArcDPSEnums.TargetID.CALeftArm_CHINA : ArcDPSEnums.TargetID.CALeftArm);
            IReadOnlyList<AgentItem> rightArms = agentData.GetGadgetsByID(_cn ? ArcDPSEnums.TargetID.CARightArm_CHINA : ArcDPSEnums.TargetID.CARightArm);
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
            FindChestGadget(ChestID, agentData, combatData, CAChestPosition, (agentItem) => agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100);
            agentData.Refresh();
            AgentItem sword = agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Conjured Sword\0:Conjured Sword\051", ParserHelper.Spec.NPC, ArcDPSEnums.TrashID.ConjuredPlayerSword, true);
            ComputeFightTargets(agentData, combatData, extensions);
            foreach (CombatItem c in combatData)
            {
                if (c.IsDamage(extensions) && c.SkillID == ConjuredSlashPlayer)
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
                    var shieldCA = target.GetBuffStatus(log, Shielded, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    int CAShieldRadius = 500;
                    foreach (Segment seg in shieldCA)
                    {
                        replay.Decorations.Add(new CircleDecoration(CAShieldRadius, seg, "rgba(0, 150, 255, 0.3)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.CALeftArm:
                case (int)ArcDPSEnums.TargetID.CARightArm:
                    break;
                case (int)ArcDPSEnums.TrashID.ConjuredGreatsword:
                    break;
                case (int)ArcDPSEnums.TrashID.ConjuredShield:
                    var shieldShield = target.GetBuffStatus(log, Shielded, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    int ShieldShieldRadius = 100;
                    foreach (Segment seg in shieldShield)
                    {
                        replay.Decorations.Add(new CircleDecoration(ShieldShieldRadius, seg, "rgba(0, 150, 255, 0.3)", new AgentConnector(target)));
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
                AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.ConjuredAmalgamate));
                AbstractSingleActor leftArm = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.CALeftArm));
                AbstractSingleActor rightArm = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.CARightArm));
                if (target == null)
                {
                    throw new MissingKeyActorsException("Conjured Amalgamate not found");
                }
                AgentItem zommoros = agentData.GetNPCsByID(ArcDPSEnums.TrashID.ChillZommoros).LastOrDefault();
                if (zommoros == null)
                {
                    return;
                }
                SpawnEvent npcSpawn = combatData.GetSpawnEvents(zommoros).LastOrDefault();
                AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && !x.ToFriendly && playerAgents.Contains(x.From.GetFinalMaster()));
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
            var attackTargetsAgents = log.CombatData.GetAttackTargetEvents(target.AgentItem).ToList();
            var attackTargets = new HashSet<AgentItem>();
            foreach (AttackTargetEvent c in attackTargetsAgents) // 3rd one is weird
            {
                attackTargets.Add(c.AttackTarget);
                if (attackTargets.Count == 2)
                {
                    break;
                }
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
            AbstractSingleActor ca = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.ConjuredAmalgamate));
            if (ca == null)
            {
                throw new MissingKeyActorsException("Conjured Amalgamate not found");
            }
            phases[0].AddTarget(ca);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, CAInvul, ca, true, false));
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
            AbstractSingleActor leftArm = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.CALeftArm));
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
            AbstractSingleActor rightArm = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.CARightArm));
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
            base.ComputePlayerCombatReplayActors(p, log, replay);
            // Conjured Protection - Shield AoE
            IReadOnlyList<AbstractCastEvent> cls = p.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            var shieldCast = cls.Where(x => x.SkillId == ConjuredProtectionSAK).ToList();
            foreach (AbstractCastEvent c in shieldCast)
            {
                int start = (int)c.Time;
                int duration = 10000;
                int radius = 300;
                Point3D position = p.GetCurrentInterpolatedPosition(log, start);
                if (position != null)
                {
                    var circle = new CircleDecoration(radius, (start, start + duration), "rgba(255, 0, 255, 0.2)", new PositionConnector(position));
                    replay.AddDecorationWithBorder(circle);
                }
            }
            // Shields and Greatswords Overheads
            replay.AddOverheadIcons(p.GetBuffStatus(log, ConjuredShield, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0), p, ParserIcons.ConjuredShieldEmptyOverhead);
            replay.AddOverheadIcons(p.GetBuffStatus(log, GreatswordPower, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0), p, ParserIcons.GreatswordPowerEmptyOverhead);
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.ConjuredAmalgamate));
            if (target == null)
            {
                throw new MissingKeyActorsException("Conjured Amalgamate not found");
            }
            return combatData.GetBuffData(LockedOn).Count > 0 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }
    }
}
