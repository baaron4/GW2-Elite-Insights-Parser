using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Deimos : RaidLogic
    {

        private long _specialSplit = 0;

        public Deimos(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(37716, "Rapid Decay", new MechanicPlotlySetting("circle-open","rgb(0,0,0)"), "Oil","Rapid Decay (Black expanding oil)", "Black Oil",0),
            new FirstHitOnPlayerMechanic(37716, "Rapid Decay", new MechanicPlotlySetting("circle","rgb(0,0,0)"), "Oil T.","Rapid Decay Trigger (Black expanding oil)", "Black Oil Trigger",0, (ce, log) => {
                AbstractSingleActor actor = log.FindActor(ce.To);
                if (actor == null)
                {
                    return false;
                }
                (_, List<(long start, long end)> downs , _) = actor.GetStatus(log);
                bool hitInDown = downs.Exists(x => x.start < ce.Time && ce.Time < x.end);
                return !hitInDown;
            }),
            new EnemyCastStartMechanic(37846, "Off Balance", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "TP CC","Off Balance (Saul TP Breakbar)", "Saul TP Start",0),
            new EnemyCastEndMechanic(37846, "Off Balance", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "TP CC Fail","Failed Saul TP CC", "Failed CC (TP)",0, (ce,log) => ce.ActualDuration >= 2200),
            new EnemyCastEndMechanic(37846, "Off Balance", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "TP CCed","Saul TP CCed", "CCed (TP)",0, (ce, log) => ce.ActualDuration < 2200),
            new EnemyCastStartMechanic(38272, "Boon Thief", new MechanicPlotlySetting("diamond-wide","rgb(0,160,150)"), "Thief CC","Boon Thief (Saul Breakbar)", "Boon Thief Start",0),
            new EnemyCastEndMechanic(38272, "Boon Thief", new MechanicPlotlySetting("diamond-wide","rgb(255,0,0)"), "Thief CC Fail","Failed Boon Thief CC", "Failed CC (Thief)",0,(ce,log) => ce.ActualDuration >= 4400),
            new EnemyCastEndMechanic(38272, "Boon Thief", new MechanicPlotlySetting("diamond-wide","rgb(0,160,0)"), "Thief CCed","Boon Thief CCed", "CCed (Thief)",0,(ce, log) => ce.ActualDuration < 4400),
            new HitOnPlayerMechanic(38208, "Annihilate", new MechanicPlotlySetting("hexagon","rgb(255,200,0)"), "Pizza","Annihilate (Cascading Pizza attack)", "Boss Smash",0),
            new HitOnPlayerMechanic(37929, "Annihilate", new MechanicPlotlySetting("hexagon","rgb(255,200,0)"), "Pizza","Annihilate (Cascading Pizza attack)", "Boss Smash",0),
            new HitOnPlayerMechanic(37980, "Demonic Shock Wave", new MechanicPlotlySetting("triangle-right-open","rgb(255,0,0)"), "10% RSmash","Knockback (right hand) in 10% Phase", "10% Right Smash",0),
            new HitOnPlayerMechanic(38046, "Demonic Shock Wave", new MechanicPlotlySetting("triangle-left-open","rgb(255,0,0)"), "10% LSmash","Knockback (left hand) in 10% Phase", "10% Left Smash",0),
            new HitOnPlayerMechanic(37982, "Demonic Shock Wave", new MechanicPlotlySetting("bowtie","rgb(255,0,0)"), "10% DSmash","Knockback (both hands) in 10% Phase", "10% Double Smash",0),
            new PlayerBuffApplyMechanic(37733, "Tear Instability", new MechanicPlotlySetting("diamond","rgb(0,128,128)"), "Tear","Collected a Demonic Tear", "Tear",0),
            new HitOnPlayerMechanic(37613, "Mind Crush", new MechanicPlotlySetting("square","rgb(0,0,255)"), "Mind Crush","Hit by Mind Crush without Bubble Protection", "Mind Crush",0, (de,log) => de.HealthDamage > 0),
            new PlayerBuffApplyMechanic(38187, "Weak Minded", new MechanicPlotlySetting("square-open","rgb(200,140,255)"), "Weak Mind","Weak Minded (Debuff after Mind Crush)", "Weak Minded",0),
            new PlayerBuffApplyMechanic(37730, "Chosen by Eye of Janthir", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Green","Chosen by the Eye of Janthir", "Chosen (Green)",0),
            new PlayerBuffApplyMechanic(38169, "Teleported", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "TP","Teleport to/from Demonic Realm", "Teleport",0),
            new EnemyBuffApplyMechanic(38224, "Unnatural Signet", new MechanicPlotlySetting("square-open","rgb(0,255,255)"), "DMG Debuff","Double Damage Debuff on Deimos", "+100% Dmg Buff",0)
            });
            Extension = "dei";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = "https://wiki.guildwars2.com/images/e/e0/Mini_Ragged_White_Mantle_Figurehead.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/GCwOVVE.png",
                            (4400, 5753),
                            (-9542, 1932, -7004, 5250),
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376));
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Deimos,
                (int)ArcDPSEnums.TrashID.Thief,
                (int)ArcDPSEnums.TrashID.Drunkard,
                (int)ArcDPSEnums.TrashID.Gambler,
            };
        }

        private static void MergeWithGadgets(AgentItem target, HashSet<ulong> gadgetAgents, List<CombatItem> combatData)
        {
            var allAgents = new HashSet<ulong>(gadgetAgents)
            {
                target.Agent
            };
            foreach (CombatItem c in combatData)
            {
                if (gadgetAgents.Contains(c.SrcAgent))
                {
                    if (c.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)
                    {
                        continue;
                    }
                    if (c.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && c.DstAgent > 1500)
                    {
                        continue;
                    }
                }
                if (allAgents.Contains(c.SrcAgent) && c.IsStateChange.SrcIsAgent())
                {
                    c.OverrideSrcAgent(target.Agent);

                }
                if (allAgents.Contains(c.DstAgent) && c.IsStateChange.DstIsAgent())
                {
                    c.OverrideDstAgent(target.Agent);
                }
            }
        }

        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            NPC target = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Deimos);
            if (target == null)
            {
                throw new MissingKeyActorsException("Deimos not found");
            }
            var res = new List<AbstractBuffEvent>();
            if (buffsById.TryGetValue(38224, out List<AbstractBuffEvent> list))
            {
                foreach (AbstractBuffEvent bfe in list)
                {
                    if (bfe is BuffApplyEvent ba)
                    {
                        AbstractBuffEvent removal = list.FirstOrDefault(x => x is BuffRemoveAllEvent && x.Time > bfe.Time && x.Time < bfe.Time + 30000);
                        if (removal == null)
                        {
                            res.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, target.AgentItem, ba.Time + ba.AppliedDuration, 0, skillData.Get(38224), 1, 0));
                            res.Add(new BuffRemoveManualEvent(ParserHelper._unknownAgent, target.AgentItem, ba.Time + ba.AppliedDuration, 0, skillData.Get(38224)));
                        }
                    }
                    else if (bfe is BuffRemoveAllEvent)
                    {
                        AbstractBuffEvent apply = list.FirstOrDefault(x => x is BuffApplyEvent && x.Time < bfe.Time && x.Time > bfe.Time - 30000);
                        if (apply == null)
                        {
                            res.Add(new BuffApplyEvent(ParserHelper._unknownAgent, target.AgentItem, bfe.Time - 10000, 10000, skillData.Get(38224), uint.MaxValue, true));
                        }
                    }
                }
            }
            return res;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success && _specialSplit > 0)
            {
                NPC deimos = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Deimos);
                if (deimos == null)
                {
                    throw new MissingKeyActorsException("Deimos not found");
                }
                NPC saul = TrashMobs.LastOrDefault(x => x.ID == (int)ArcDPSEnums.TrashID.Saul);
                if (saul == null)
                {
                    throw new MissingKeyActorsException("Saul not found");
                }
                if (combatData.GetDeadEvents(saul.AgentItem).Any())
                {
                    return;
                }
                List<AttackTargetEvent> attackTargets = combatData.GetAttackTargetEvents(deimos.AgentItem);
                if (attackTargets.Count == 0)
                {
                    return;
                }
                long specialSplitTime = _specialSplit;
                AgentItem attackTarget = attackTargets.Last().AttackTarget;
                // sanity check
                TargetableEvent attackableEvent = combatData.GetTargetableEvents(attackTarget).LastOrDefault(x => x.Targetable && x.Time > specialSplitTime - ParserHelper.ServerDelayConstant);
                if (attackableEvent == null)
                {
                    return;
                }
                TargetableEvent notAttackableEvent = combatData.GetTargetableEvents(attackTarget).LastOrDefault(x => !x.Targetable && x.Time > attackableEvent.Time);
                if (notAttackableEvent == null)
                {
                    return;
                }
                AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(deimos.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && x.Time > specialSplitTime && playerAgents.Contains(x.From.GetFinalMaster()));
                if (lastDamageTaken != null)
                {
                    if (!AtLeastOnePlayerAlive(combatData, fightData, notAttackableEvent.Time, playerAgents))
                    {
                        return;
                    }
                    fightData.SetSuccess(true, lastDamageTaken.Time);
                }
            }
        }

        private static long AttackTargetSpecialParse(CombatItem targetable, AgentData agentData, List<CombatItem> combatData, HashSet<ulong> gadgetAgents)
        {
            if (targetable == null)
            {
                return 0;
            }
            long firstAware = targetable.Time;
            AgentItem targetAgent = agentData.GetAgent(targetable.SrcAgent);
            if (targetAgent == ParserHelper._unknownAgent)
            {
                return 0;
            }
            CombatItem attackTargetEvent = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.AttackTarget && x.SrcAgent == targetAgent.Agent);
            if (attackTargetEvent == null)
            {
                return 0;
            }
            AgentItem deimosStructBody = agentData.GetAgent(attackTargetEvent.DstAgent);
            if (deimosStructBody == ParserHelper._unknownAgent)
            {
                return 0;
            }
            gadgetAgents.Add(deimosStructBody.Agent);
            CombatItem armDeimosDamageEvent = combatData.FirstOrDefault(x => x.Time >= firstAware && (x.SkillID == 37980 || x.SkillID == 37982 || x.SkillID == 38046) && x.SrcAgent != 0 && x.SrcInstid != 0);
            if (armDeimosDamageEvent != null)
            {
                gadgetAgents.Add(armDeimosDamageEvent.SrcAgent);
            }
            return firstAware;
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            List<AgentItem> deimosAgents = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Deimos);
            long start = fightData.LogStart;
            foreach (AgentItem deimos in deimosAgents)
            {
                // enter combat
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcAgent == deimos.Agent && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat);
                if (enterCombat != null)
                {
                    start = Math.Max(start, enterCombat.Time);

                }
            }
            return start;
        }

        internal override void EIEvtcParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, List<Player> playerList)
        {
            ComputeFightTargets(agentData, combatData);
            // Find target
            NPC target = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Deimos);
            if (target == null)
            {
                throw new MissingKeyActorsException("Deimos not found");
            }
            // Remove deimos despawn events as they are useless and mess with combat replay
            combatData.RemoveAll(x => x.IsStateChange == ArcDPSEnums.StateChange.Despawn && x.SrcAgent == target.Agent);
            // invul correction
            CombatItem invulApp = combatData.FirstOrDefault(x => x.DstAgent == target.Agent && x.IsBuff != 0 && x.BuffDmg == 0 && x.Value > 0 && x.SkillID == 762 && x.IsStateChange == ArcDPSEnums.StateChange.None);
            if (invulApp != null)
            {
                invulApp.OverrideValue((int)(target.LastAware - invulApp.Time));
            }
            // Deimos gadgets
            CombatItem targetable = combatData.LastOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.Targetable && x.DstAgent > 0);
            var gadgetAgents = new HashSet<ulong>();
            long firstAware = AttackTargetSpecialParse(targetable, agentData, combatData, gadgetAgents);
            // legacy method
            if (firstAware == 0)
            {
                CombatItem armDeimosDamageEvent = combatData.FirstOrDefault(x => x.Time >= target.LastAware && (x.SkillID == 37980 || x.SkillID == 37982 || x.SkillID == 38046) && x.SrcAgent != 0 && x.SrcInstid != 0);
                if (armDeimosDamageEvent != null)
                {
                    var deimosGadgets = agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.Name.Contains("Deimos") && x.LastAware > armDeimosDamageEvent.Time).ToList();
                    if (deimosGadgets.Count > 0)
                    {
                        firstAware = deimosGadgets.Max(x => x.FirstAware);
                        gadgetAgents = new HashSet<ulong>(deimosGadgets.Select(x => x.Agent));
                    }
                }
            }
            if (gadgetAgents.Count > 0)
            {
                _specialSplit = (firstAware >= target.LastAware ? firstAware : target.LastAware);
                MergeWithGadgets(target.AgentItem, gadgetAgents, combatData);
            }
            target.AgentItem.OverrideAwareTimes(target.FirstAware, fightData.FightEnd);
            target.OverrideName("Deimos");
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Deimos);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Deimos not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Determined + additional data on inst change
            AbstractBuffEvent invulDei = log.CombatData.GetBuffData(762).Find(x => x is BuffApplyEvent && x.To == mainTarget.AgentItem);
            if (invulDei != null)
            {
                end = invulDei.Time;
                phases.Add(new PhaseData(start, end));
                start = _specialSplit > 0 ? _specialSplit : fightDuration;
                //mainTarget.AddCustomCastLog(end, -6, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None, log);
            }
            else if (_specialSplit > 0)
            {
                long specialTime = _specialSplit;
                end = specialTime;
                phases.Add(new PhaseData(start, end));
                start = specialTime;
            }
            if (fightDuration - start > ParserHelper.PhaseTimeLimit && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            string[] names = { "100% - 10%", "10% - 0%" };
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = names[i - 1];
                phases[i].Targets.Add(mainTarget);
            }
            foreach (NPC tar in Targets)
            {
                if (tar.ID == (int)ArcDPSEnums.TrashID.Thief || tar.ID == (int)ArcDPSEnums.TrashID.Drunkard || tar.ID == (int)ArcDPSEnums.TrashID.Gambler)
                {
                    string name = (tar.ID == (int)ArcDPSEnums.TrashID.Thief ? "Thief" : (tar.ID == (int)ArcDPSEnums.TrashID.Drunkard ? "Drunkard" : (tar.ID == (int)ArcDPSEnums.TrashID.Gambler ? "Gambler" : "")));
                    tar.OverrideName(name);
                    var tarPhase = new PhaseData(tar.FirstAware - 1000, Math.Min(tar.LastAware + 1000, fightDuration), name);
                    tarPhase.Targets.Add(tar);
                    tarPhase.OverrideTimes(log);
                    // override first then add Deimos so that it does not disturb the override process
                    tarPhase.Targets.Add(mainTarget);
                    phases.Add(tarPhase);
                }
            }
            List<AbstractBuffEvent> signets = GetFilteredList(log.CombatData, 38224, mainTarget, true);
            long sigStart = 0;
            long sigEnd = 0;
            int burstID = 1;
            for (int i = 0; i < signets.Count; i++)
            {
                AbstractBuffEvent signet = signets[i];
                if (signet is BuffApplyEvent)
                {
                    sigStart = Math.Max(signet.Time + 1, 0);
                }
                else
                {
                    sigEnd = Math.Min(signet.Time - 1, fightDuration);
                    var burstPhase = new PhaseData(sigStart, sigEnd, "Burst " + burstID++);
                    burstPhase.Targets.Add(mainTarget);
                    phases.Add(burstPhase);
                }
            }
            return phases;
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Deimos,
                (int)ArcDPSEnums.TrashID.Thief,
                (int)ArcDPSEnums.TrashID.Drunkard,
                (int)ArcDPSEnums.TrashID.Gambler
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Saul,
                ArcDPSEnums.TrashID.GamblerClones,
                ArcDPSEnums.TrashID.GamblerReal,
                ArcDPSEnums.TrashID.Greed,
                ArcDPSEnums.TrashID.Pride,
                ArcDPSEnums.TrashID.Oil,
                ArcDPSEnums.TrashID.Tear,
                ArcDPSEnums.TrashID.Hands
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            List<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Deimos:
                    var mindCrush = cls.Where(x => x.SkillId == 37613).ToList();
                    foreach (AbstractCastEvent c in mindCrush)
                    {
                        start = (int)c.Time;
                        end = start + 5000;
                        replay.Decorations.Add(new CircleDecoration(true, end, 180, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(false, 0, 180, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        if (!log.FightData.IsCM)
                        {
                            replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(0, 0, 255, 0.3)", new PositionConnector(new Point3D(-8421.818f, 3091.72949f, -9.818082e8f, 216))));
                        }
                    }
                    var annihilate = cls.Where(x => (x.SkillId == 38208) || (x.SkillId == 37929)).ToList();
                    foreach (AbstractCastEvent c in annihilate)
                    {
                        start = (int)c.Time;
                        int delay = 1000;
                        end = start + 2400;
                        int duration = 120;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * duration), "rgba(255, 200, 0, 0.5)", new AgentConnector(target)));
                            replay.Decorations.Add(new PieDecoration(false, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * 120), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                            if (i % 5 != 0)
                            {
                                replay.Decorations.Add(new PieDecoration(true, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI - i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * 120), "rgba(255, 200, 0, 0.5)", new AgentConnector(target)));
                                replay.Decorations.Add(new PieDecoration(false, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI - i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * 120), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                            }
                        }
                    }
                    List<AbstractBuffEvent> signets = GetFilteredList(log.CombatData, 38224, target, true);
                    int sigStart = 0;
                    int sigEnd = 0;
                    foreach (AbstractBuffEvent signet in signets)
                    {
                        if (signet is BuffApplyEvent)
                        {
                            sigStart = (int)signet.Time;
                        }
                        else
                        {
                            sigEnd = (int)signet.Time;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 120, (sigStart, sigEnd), "rgba(0, 200, 200, 0.5)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Gambler:
                case (int)ArcDPSEnums.TrashID.Thief:
                case (int)ArcDPSEnums.TrashID.Drunkard:
                    break;
                case (int)ArcDPSEnums.TrashID.Saul:
                case (int)ArcDPSEnums.TrashID.GamblerClones:
                case (int)ArcDPSEnums.TrashID.GamblerReal:
                case (int)ArcDPSEnums.TrashID.Greed:
                case (int)ArcDPSEnums.TrashID.Pride:
                case (int)ArcDPSEnums.TrashID.Tear:
                    break;
                case (int)ArcDPSEnums.TrashID.Hands:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 90, (start, end), "rgba(255, 0, 0, 0.2)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Oil:
                    int delayOil = 3000;
                    replay.Decorations.Add(new CircleDecoration(true, start + delayOil, 200, (start, start + delayOil), "rgba(255,100, 0, 0.5)", new AgentConnector(target)));
                    replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start + delayOil, end), "rgba(0, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                default:
                    break;
            }

        }

        internal override void ComputePlayerCombatReplayActors(Player p, ParsedEvtcLog log, CombatReplay replay)
        {
            // teleport zone
            List<AbstractBuffEvent> tpDeimos = GetFilteredList(log.CombatData, 37730, p, true);
            int tpStart = 0;
            foreach (AbstractBuffEvent c in tpDeimos)
            {
                if (c is BuffApplyEvent)
                {
                    tpStart = (int)c.Time;
                }
                else
                {
                    int tpEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 180, (tpStart, tpEnd), "rgba(0, 150, 0, 0.3)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, tpEnd, 180, (tpStart, tpEnd), "rgba(0, 150, 0, 0.3)", new AgentConnector(p)));
                }
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            NPC target = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Deimos);
            if (target == null)
            {
                throw new MissingKeyActorsException("Deimos not found");
            }
            FightData.CMStatus res = (target.GetHealth(combatData) > 40e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
            if (_specialSplit > 0)
            {
                target.SetManualHealth(res > 0 ? 42804900 : 37388210);
            }
            return res;
        }
    }
}
