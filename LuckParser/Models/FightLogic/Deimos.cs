using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Deimos : RaidLogic
    {

        private long _specialSplitLogTime = 0;

        public Deimos(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(37716, "Rapid Decay", new MechanicPlotlySetting("circle-open","rgb(0,0,0)"), "Oil","Rapid Decay (Black expanding oil)", "Black Oil",0),
            new FirstHitOnPlayerMechanic(37716, "Rapid Decay", new MechanicPlotlySetting("circle","rgb(0,0,0)"), "Oil Trigger","Rapid Decay Trigger (Black expanding oil)", "Black Oil Trigger",0),
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
            new HitOnPlayerMechanic(37982, "Demonic Shock Wave", new MechanicPlotlySetting("bowtie","rgb(255,0,0)"), "10% Double Smash","Knockback (both hands) in 10% Phase", "10% Double Smash",0),
            new PlayerBoonApplyMechanic(37733, "Tear Instability", new MechanicPlotlySetting("diamond","rgb(0,128,128)"), "Tear","Collected a Demonic Tear", "Tear",0),
            new HitOnPlayerMechanic(37613, "Mind Crush", new MechanicPlotlySetting("square","rgb(0,0,255)"), "Mind Crush","Hit by Mind Crush without Bubble Protection", "Mind Crush",0, (de,log) => de.Damage > 0),
            new PlayerBoonApplyMechanic(38187, "Weak Minded", new MechanicPlotlySetting("square-open","rgb(200,140,255)"), "Weak Mind","Weak Minded (Debuff after Mind Crush)", "Weak Minded",0),
            new PlayerBoonApplyMechanic(37730, "Chosen by Eye of Janthir", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Green","Chosen by the Eye of Janthir", "Chosen (Green)",0),
            new PlayerBoonApplyMechanic(38169, "Teleported", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "TP","Teleport to/from Demonic Realm", "Teleport",0),
            new EnemyBoonApplyMechanic(38224, "Unnatural Signet", new MechanicPlotlySetting("square-open","rgb(0,255,255)"), "DMG Debuff","Double Damage Debuff on Deimos", "+100% Dmg Buff",0)
            });
            Extension = "dei";
            GenericFallBackMethod = FallBackMethod.None;
            IconUrl = "https://wiki.guildwars2.com/images/e/e0/Mini_Ragged_White_Mantle_Figurehead.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/GCwOVVE.png",
                            (4400, 5753),
                            (-9542, 1932, -7004, 5250),
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376));
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Deimos,
                (ushort)Thief,
                (ushort)Drunkard,
                (ushort)Gambler,
            };
        }

        private void SetUniqueID(AgentItem target, HashSet<ulong> gadgetAgents, AgentData agentData, List<CombatItem> combatData)
        {
            // get unique id for the fusion
            ushort instID = 0;
            Random rnd = new Random();
            while (agentData.InstIDValues.Contains(instID) || instID == 0)
            {
                instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
            }
            target.InstID = instID;
            agentData.Refresh();
            HashSet<ulong> allAgents = new HashSet<ulong>(gadgetAgents)
            {
                target.Agent
            };
            foreach (CombatItem c in combatData)
            {
                if (gadgetAgents.Contains(c.SrcAgent) && c.IsStateChange == ParseEnum.StateChange.MaxHealthUpdate)
                {
                    continue;
                }
                if (allAgents.Contains(c.SrcAgent))
                {
                    c.OverrideSrcValues(target.Agent, target.InstID);

                }
                if (allAgents.Contains(c.DstAgent))
                {
                    c.OverrideDstValues(target.Agent, target.InstID);
                }
            }
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                Target target = Targets.Find(x => x.ID == TriggerID);
                if (target == null)
                {
                    throw new InvalidOperationException("Target for success by combat exit not found");
                }
                List<AttackTargetEvent> attackTargets = combatData.GetAttackTargetEvents(target.AgentItem);
                if (attackTargets.Count == 0)
                {
                    return;
                }
                AgentItem attackTarget = attackTargets.Last().AttackTarget;
                List<ExitCombatEvent> playerExits = new List<ExitCombatEvent>();
                foreach (AgentItem a in playerAgents)
                {
                    playerExits.AddRange(combatData.GetExitCombatEvents(a));
                }
                ExitCombatEvent lastPlayerExit = playerExits.Count > 0 ? playerExits.MaxBy(x => x.Time) : null;
                TargetableEvent notAttackableEvent = combatData.GetTargetableEvents(attackTarget).LastOrDefault(x => !x.Targetable);
                AbstractDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.Damage > 0) && (playerAgents.Contains(x.From) || playerAgents.Contains(x.MasterFrom)));
                if (notAttackableEvent != null && lastDamageTaken != null && lastPlayerExit != null)
                {
                    fightData.SetSuccess(lastPlayerExit.Time > notAttackableEvent.Time + 1000, fightData.ToLogSpace(lastDamageTaken.Time));
                }
            }
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            ComputeFightTargets(agentData, fightData, combatData);
            // Find target
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Deimos);
            if (target == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            // enter combat
            CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcInstid == target.InstID && x.IsStateChange == ParseEnum.StateChange.EnterCombat && x.SrcInstid == target.InstID && x.LogTime <= target.LastAwareLogTime && x.LogTime >= target.FirstAwareLogTime);
            if (enterCombat != null)
            {
                fightData.OverrideStart(enterCombat.LogTime);
            }
            // Remove deimos despawn events as they are useless and mess with combat replay
            combatData.RemoveAll(x => x.IsStateChange == ParseEnum.StateChange.Despawn && x.SrcAgent == target.Agent);
            // Deimos gadgets
            List<AgentItem> deimosGadgets = agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.Name.Contains("Deimos") && x.LastAwareLogTime > target.LastAwareLogTime).ToList();
            CombatItem invulApp = combatData.FirstOrDefault(x => x.DstInstid == target.InstID && x.IsBuff != 0 && x.BuffDmg == 0 && x.Value > 0 && x.SkillID == 762);
            CombatItem targetable = combatData.LastOrDefault(x => x.IsStateChange == ParseEnum.StateChange.Targetable && x.LogTime > combatData.First().LogTime && x.DstAgent > 0);
            if (invulApp != null && targetable != null)
            {
                HashSet<ulong> gadgetAgents = new HashSet<ulong>();
                long firstAware = targetable.LogTime;
                AgentItem targetAgent = agentData.GetAgentByInstID(targetable.SrcInstid, targetable.LogTime);
                if (targetAgent != GeneralHelper.UnknownAgent)
                {
                    try
                    {
                        string[] names = targetAgent.Name.Split('-');
                        if (ushort.TryParse(names[2], out ushort masterInstid))
                        {
                            CombatItem structDeimosDamageEvent = combatData.FirstOrDefault(x => x.LogTime >= firstAware && x.IFF == ParseEnum.IFF.Foe && x.DstInstid == masterInstid && x.IsStateChange == ParseEnum.StateChange.None && x.IsBuffRemove == ParseEnum.BuffRemove.None &&
                                    ((x.IsBuff == 1 && x.BuffDmg >= 0 && x.Value == 0) ||
                                    (x.IsBuff == 0 && x.Value >= 0)));
                            if (structDeimosDamageEvent != null)
                            {
                                gadgetAgents.Add(structDeimosDamageEvent.DstAgent);
                            }
                            CombatItem armDeimosDamageEvent = combatData.FirstOrDefault(x => x.LogTime >= firstAware && (x.SkillID == 37980 || x.SkillID == 37982 || x.SkillID == 38046) && x.SrcAgent != 0 && x.SrcInstid != 0);
                            if (armDeimosDamageEvent != null)
                            {
                                gadgetAgents.Add(armDeimosDamageEvent.SrcAgent);
                            }
                        };
                    }
                    catch
                    {
                        // nothing to do
                    }
                }
                invulApp.OverrideValue((int)(firstAware - invulApp.LogTime));
                _specialSplitLogTime = (firstAware >= target.LastAwareLogTime ? firstAware : target.LastAwareLogTime);
                target.AgentItem.LastAwareLogTime = combatData.Last().LogTime;
                SetUniqueID(target.AgentItem, gadgetAgents, agentData, combatData);
            }
            // legacy method
            else if (deimosGadgets.Count > 0)
            {
                long firstAware = deimosGadgets.Max(x => x.FirstAwareLogTime);
                _specialSplitLogTime = (firstAware >= target.LastAwareLogTime ? firstAware : target.LastAwareLogTime);
                target.AgentItem.LastAwareLogTime = deimosGadgets.Max(x => x.LastAwareLogTime);
                HashSet<ulong> gadgetAgents = new HashSet<ulong>(deimosGadgets.Select(x => x.Agent));
                SetUniqueID(target.AgentItem, gadgetAgents, agentData, combatData);
            }
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Deimos);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Determined + additional data on inst change
            AbstractBuffEvent invulDei = log.CombatData.GetBoonData(762).Find(x => x is BuffApplyEvent && x.To == mainTarget.AgentItem);
            if (invulDei != null)
            {
                end = invulDei.Time;
                phases.Add(new PhaseData(start, end));
                start = (_specialSplitLogTime > 0 ? log.FightData.ToFightSpace(_specialSplitLogTime) : fightDuration);
                //mainTarget.AddCustomCastLog(end, -6, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None, log);
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            string[] names = { "100% - 10%", "10% - 0%" };
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = names[i - 1];
                phases[i].Targets.Add(mainTarget);
            }
            foreach (Target tar in Targets)
            {
                if (tar.ID == (ushort)Thief || tar.ID == (ushort)Drunkard || tar.ID == (ushort)Gambler)
                {
                    string name = (tar.ID == (ushort)Thief ? "Thief" : (tar.ID == (ushort)Drunkard ? "Drunkard" : (tar.ID == (ushort)Gambler ? "Gambler" : "")));
                    PhaseData tarPhase = new PhaseData(log.FightData.ToFightSpace(tar.FirstAwareLogTime) - 1000, log.FightData.ToFightSpace(tar.LastAwareLogTime) + 1000);
                    tarPhase.Targets.Add(tar);
                    tarPhase.OverrideTimes(log);
                    // override first then add Deimos so that it does not disturb the override process
                    tarPhase.Targets.Add(mainTarget);
                    tarPhase.Name = name;
                    phases.Add(tarPhase);
                }
            }
            /*
            List<CombatItem> signets = GetFilteredList(log, 38224, mainTarget, true);
            long sigStart = 0;
            long sigEnd = 0;
            int burstID = 1;
            for (int i = 0; i < signets.Count; i++)
            {
                CombatItem signet = signets[i];
                if (signet.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    sigStart = log.FightData.ToFightSpace(signet.Time);
                }
                else
                {
                    sigEnd = log.FightData.ToFightSpace(signet.Time);
                    PhaseData burstPhase = new PhaseData(sigStart, sigEnd)
                    {
                        Name = "Burst " + burstID++
                    };
                    burstPhase.Targets.Add(mainTarget);
                    phases.Add(burstPhase);
                }
            }*/
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            phases.RemoveAll(x => x.Targets.Count == 0);
            return phases;
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Deimos,
                (ushort)Thief,
                (ushort)Drunkard,
                (ushort)Gambler
            };
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Saul,
                GamblerClones,
                GamblerReal,
                Greed,
                Pride,
                Oil,
                Tear,
                Hands
            };
        }

        public override void ComputeMobCombatReplayActors(Mob mob, ParsedLog log, CombatReplay replay)
        {
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (mob.ID)
            {
                case (ushort)Saul:
                case (ushort)GamblerClones:
                case (ushort)GamblerReal:
                case (ushort)Greed:
                case (ushort)Pride:
                case (ushort)Tear:
                    break;
                case (ushort)Hands:
                    replay.Actors.Add(new CircleActor(true, 0, 90, (start, end), "rgba(255, 0, 0, 0.2)", new AgentConnector(mob)));
                    break;
                case (ushort)Oil:
                    int delay = 3000;
                    replay.Actors.Add(new CircleActor(true, start + delay, 200, (start, start + delay), "rgba(255,100, 0, 0.5)", new AgentConnector(mob)));
                    replay.Actors.Add(new CircleActor(true, 0, 200, (start + delay, end), "rgba(0, 0, 0, 0.5)", new AgentConnector(mob)));
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Deimos:
                    List<AbstractCastEvent> mindCrush = cls.Where(x => x.SkillId == 37613).ToList();
                    foreach (AbstractCastEvent c in mindCrush)
                    {
                        int start = (int)c.Time;
                        int end = start + 5000;
                        replay.Actors.Add(new CircleActor(true, end, 180, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        replay.Actors.Add(new CircleActor(false, 0, 180, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        if (!log.FightData.IsCM)
                        {
                            replay.Actors.Add(new CircleActor(true, 0, 180, (start, end), "rgba(0, 0, 255, 0.3)", new PositionConnector(new Point3D(-8421.818f, 3091.72949f, -9.818082e8f, 216))));
                        }
                    }
                    List<AbstractCastEvent> annihilate = cls.Where(x => (x.SkillId == 38208) || (x.SkillId == 37929)).ToList();
                    foreach (AbstractCastEvent c in annihilate)
                    {
                        int start = (int)c.Time;
                        int delay = 1000;
                        int end = start + 2400;
                        int duration = 120;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            replay.Actors.Add(new PieActor(true, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * duration), "rgba(255, 200, 0, 0.5)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(false, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * 120), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                            if (i % 5 != 0)
                            {
                                replay.Actors.Add(new PieActor(true, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI - i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * 120), "rgba(255, 200, 0, 0.5)", new AgentConnector(target)));
                                replay.Actors.Add(new PieActor(false, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI - i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * 120), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
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
                            replay.Actors.Add(new CircleActor(true, 0, 120, (sigStart, sigEnd), "rgba(0, 200, 200, 0.5)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (ushort)Gambler:
                case (ushort)Thief:
                case (ushort)Drunkard:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }

        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
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
                    replay.Actors.Add(new CircleActor(true, 0, 180, (tpStart, tpEnd), "rgba(0, 150, 0, 0.3)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, tpEnd, 180, (tpStart, tpEnd), "rgba(0, 150, 0, 0.3)", new AgentConnector(p)));
                }
            }
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Deimos);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > 40e6) ? 1 : 0;
        }
    }
}
