using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models
{
    public class FightLogic
    {

        public enum ParseMode { Raid, Fractal, Golem, Unknown };

        private CombatReplayMap _map;
        public readonly List<Mechanic> MechanicList = new List<Mechanic> {
            new Mechanic(SkillItem.DeathId, "Dead", Mechanic.MechType.PlayerStatus, ParseEnum.TargetIDS.Unknown, "symbol:'x',color:'rgb(0,0,0)'", "Dead",0),
            new Mechanic(SkillItem.DownId, "Downed", Mechanic.MechType.PlayerStatus, ParseEnum.TargetIDS.Unknown, "symbol:'cross',color:'rgb(255,0,0)'", "Downed",0),
            new Mechanic(SkillItem.ResurrectId, "Resurrect", Mechanic.MechType.PlayerStatus, ParseEnum.TargetIDS.Unknown, "symbol:'cross-open',color:'rgb(0,255,255)'", "Res",0)}; //Resurrects (start), Resurrect
        public ParseMode Mode { get; protected set; } = ParseMode.Unknown;
        public bool CanCombatReplay { get; set; } = false;
        public string Extension { get; protected set; } = "boss";
        public string IconUrl { get; protected set; } = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
        public List<Mob> TrashMobs { get; } = new List<Mob>();
        public List<Target> Targets { get; } = new List<Target>();
        protected readonly ushort TriggerID;

        public FightLogic(ushort triggerID)
        {
            TriggerID = triggerID;
        }

        protected virtual CombatReplayMap GetCombatMapInternal()
        {
            return null;
        }

        public CombatReplayMap GetCombatMap()
        {
            if (_map == null)
            {
                _map = GetCombatMapInternal();
            }
            return _map;
        }

        protected virtual List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                TriggerID
            };
        }

        public virtual string GetFightName()
        {
            Target target = Targets.Find(x => x.ID == TriggerID);
            if (target == null)
            {
                return "UNKNOWN";
            }
            return target.Character;
        }

        protected void RegroupTargetsByID(ushort id, AgentData agentData, List<CombatItem> combatItems)
        {
            List<AgentItem> agents = agentData.GetAgentsByID(id);
            List<Target> toRegroup = Targets.Where(x => x.ID == id).ToList();
            if (agents.Count > 0 && toRegroup.Count > 0)
            {
                Targets.RemoveAll(x => x.ID == id);
                AgentItem firstItem = agents.First();
                agents = agents.Where(x => x.InstID == firstItem.InstID).ToList();
                HashSet<ulong> agentValues = new HashSet<ulong>(agents.Select(x => x.Agent));
                agentValues.Remove(firstItem.Agent);
                AgentItem newTargetAgent = new AgentItem(firstItem)
                {
                    FirstAware = agents.Min(x => x.FirstAware),
                    LastAware = agents.Max(x => x.LastAware)
                };
                agentData.OverrideID(id, firstItem.InstID, newTargetAgent);
                Targets.Add(new Target(newTargetAgent));
                if (agentValues.Count == 0)
                {
                    return;
                }
                foreach (CombatItem c in combatItems)
                {
                    if (agentValues.Contains(c.SrcAgent))
                    {
                        c.SrcAgent = newTargetAgent.Agent;
                    }
                    if (agentValues.Contains(c.DstAgent))
                    {
                        c.DstAgent = newTargetAgent.Agent;
                    }
                }
            }
        }

        protected virtual void RegroupTargets(AgentData agentData, List<CombatItem> combatItems)
        {
        }

        public void ComputeFightTargets(AgentData agentData, FightData fightData, List<CombatItem> combatItems)
        {
            List<ushort> ids = GetFightTargetsIDs();
            foreach (ushort id in ids)
            {
                List<AgentItem> agents = agentData.GetAgentsByID(id);
                foreach (AgentItem agentItem in agents)
                {
                    Targets.Add(new Target(agentItem));
                }
            }
            RegroupTargets(agentData, combatItems);
        }

        public void SetMaxHealth(ushort instid, long time, int health)
        {
            foreach (Target target in Targets)
            {
                if (target.Health == -1 && target.InstID == instid && target.FirstAware <= time && target.LastAware >= time)
                {
                    target.Health = health;
                    break;
                }
            }
        }

        protected void OverrideMaxHealths(ParsedLog log)
        {
            List<CombatItem> maxHUs = log.CombatData.GetStatesData(ParseEnum.StateChange.MaxHealthUpdate);
            if (maxHUs.Count > 0)
            {
                foreach (Target tar in Targets)
                {
                    List<CombatItem> subList = maxHUs.Where(x => x.SrcInstid == tar.InstID && x.Time >= tar.FirstAware && x.Time <= tar.LastAware).ToList();
                    if (subList.Count > 0)
                    {
                        tar.Health = subList.Max(x => (int)x.DstAgent);
                    }
                }
            }
        }

        public virtual void AddHealthUpdate(ushort instid, long time, int healthTime, int health)
        {
            foreach (Target target in Targets)
            {
                if (target.InstID == instid && target.FirstAware <= time && target.LastAware >= time)
                {
                    target.HealthOverTime.Add(new Point(healthTime, health));
                    break;
                }
            }
        }

        protected List<PhaseData> GetInitialPhase(ParsedLog log)
        {
            List<PhaseData> phases = new List<PhaseData>();
            long fightDuration = log.FightData.FightDuration;
            phases.Add(new PhaseData(0, fightDuration));
            phases[0].Name = "Full Fight";
            return phases;
        }

        public virtual List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            return phases;
        }

        protected void AddTargetsToPhase(PhaseData phase, List<ushort> ids, ParsedLog log)
        {
            foreach (Target target in Targets)
            {
                if (ids.Contains(target.ID) && phase.InInterval(target.FirstAware, log.FightData.FightStart))
                {
                    phase.Targets.Add(target);
                }
            }
            phase.OverrideTimes(log.FightData.FightStart, log.CombatData);
        }

        public virtual void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {

        }

        public virtual void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {

        }

        protected virtual List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>();
        }

        public virtual int IsCM(ParsedLog log)
        {
            return -1;
        }

        public virtual void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
        }

        public void InitTrashMobCombatReplay(ParsedLog log, int pollingRate)
        {
            List<ParseEnum.TrashIDS> ids = GetTrashMobsIDS();
            List<AgentItem> aList = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => ids.Contains(ParseEnum.GetTrashIDS(x.ID))).ToList();
            foreach (AgentItem a in aList)
            {
                Mob mob = new Mob(a);
                mob.InitCombatReplay(log, pollingRate, true, false);
                TrashMobs.Add(mob);
            }
        }

        protected void SetSuccessByDeath(ParsedLog log)
        {
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            CombatItem killed = log.CombatData.GetStatesData(ParseEnum.StateChange.ChangeDead).LastOrDefault(x => x.SrcInstid == mainTarget.InstID);
            if (killed != null)
            {
                log.FightData.Success = true;
                log.FightData.FightEnd = killed.Time;
            }
        }

        public virtual void SetSuccess(ParsedLog log)
        {
            SetSuccessByDeath(log);
        }


        public void ComputeMechanics(ParsedLog log)
        {
            MechanicData mechData = log.MechanicData;
            FightData fightData = log.FightData;
            CombatData combatData = log.CombatData;
            long start = fightData.FightStart;
            long end = fightData.FightEnd;
            Mechanic.CheckSpecialCondition condition;
            HashSet<ushort> playersIds = new HashSet<ushort>(log.PlayerList.Select(x => x.InstID));
            Dictionary<ushort, AbstractMasterPlayer> regroupedMobs = new Dictionary<ushort, AbstractMasterPlayer>();
            foreach (Mechanic mech in MechanicList)
            {
                switch (mech.MechanicType)
                {
                    case Mechanic.MechType.PlayerStatus:
                        foreach (Player p in log.PlayerList)
                        {
                            List<CombatItem> cList = new List<CombatItem>();
                            switch (mech.SkillId)
                            {
                                case SkillItem.DeathId:
                                    cList = combatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDead, start, end);
                                    break;
                                case SkillItem.DownId:
                                    cList = combatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDown, start, end);
                                    break;
                                case SkillItem.ResurrectId:
                                    cList = log.GetCastData(p.InstID).Where(x => x.SkillID == SkillItem.ResurrectId && x.IsActivation.IsCasting()).ToList();
                                    break;
                            }
                            foreach (CombatItem mechItem in cList)
                            {
                                mechData[mech].Add(new MechanicLog(mechItem.Time - start, mech, p));
                            }
                        }
                        break;
                    case Mechanic.MechType.SkillOnPlayer:
                        foreach (Player p in log.PlayerList)
                        {
                            List<DamageLog> dls = p.GetDamageTakenLogs(null, log, 0, fightData.FightDuration);
                            condition = mech.SpecialCondition;
                            foreach (DamageLog dLog in dls)
                            {
                                if (condition != null && !condition(new SpecialConditionItem(dLog)))
                                {
                                    continue;
                                }
                                if (dLog.SkillId == mech.SkillId && dLog.Result.IsHit())
                                {
                                    mechData[mech].Add(new MechanicLog(dLog.Time, mech, p));

                                }
                            }
                        }
                        break;
                    case Mechanic.MechType.PlayerBoon:
                    case Mechanic.MechType.PlayerOnPlayer:
                    case Mechanic.MechType.PlayerBoonRemove:
                        foreach (Player p in log.PlayerList)
                        {
                            condition = mech.SpecialCondition;
                            foreach (CombatItem c in log.GetBoonData(mech.SkillId))
                            {
                                if (condition != null && !condition(new SpecialConditionItem(c)))
                                {
                                    continue;
                                }
                                if (mech.MechanicType == Mechanic.MechType.PlayerBoonRemove)
                                {
                                    if (c.IsBuffRemove == ParseEnum.BuffRemove.Manual && p.InstID == c.SrcInstid)
                                    {
                                        mechData[mech].Add(new MechanicLog(c.Time - start, mech, p));
                                    }
                                }
                                else
                                {

                                    if (c.IsBuffRemove == ParseEnum.BuffRemove.None && p.InstID == c.DstInstid)
                                    {
                                        mechData[mech].Add(new MechanicLog(c.Time - start, mech, p));
                                        if (mech.MechanicType == Mechanic.MechType.PlayerOnPlayer)
                                        {
                                            mechData[mech].Add(new MechanicLog(c.Time - start, mech, log.PlayerList.FirstOrDefault(x => x.InstID == c.SrcInstid)));
                                        }
                                    }
                                }
                            }
                        }                       
                        break;
                    case Mechanic.MechType.HitOnEnemy:
                        foreach (Player p in log.PlayerList)
                        {
                            condition = mech.SpecialCondition;
                            IEnumerable<AgentItem> agents = log.AgentData.GetAgentsByID((ushort)mech.SkillId);
                            foreach (AgentItem a in agents)
                            {
                                foreach (DamageLog dl in p.GetDamageLogs(null, log, 0, log.FightData.FightDuration))
                                {
                                    if (dl.DstInstId != a.InstID || dl.IsCondi > 0 || dl.Time < a.FirstAware - start || dl.Time > a.LastAware - start || (condition != null && !condition(new SpecialConditionItem(dl))))
                                    {
                                        continue;
                                    }
                                    mechData[mech].Add(new MechanicLog(dl.Time, mech, p));
                                }
                            }
                        }
                        break;
                    case Mechanic.MechType.PlayerSkill:
                        foreach (Player p in log.PlayerList)
                        {
                            condition = mech.SpecialCondition;
                            foreach (CombatItem c in log.GetCastDataById(mech.SkillId))
                            {
                                if (condition != null && !condition(new SpecialConditionItem(c)))
                                {
                                    continue;
                                }
                                if (c.IsActivation.IsCasting() && c.SrcInstid == p.InstID)
                                {
                                    mechData[mech].Add(new MechanicLog(c.Time - fightData.FightStart, mech, p));

                                }
                            }
                        }
                        break;
                    case Mechanic.MechType.EnemyBoon:
                    case Mechanic.MechType.EnemyBoonStrip:
                        condition = mech.SpecialCondition;
                        foreach (CombatItem c in log.GetBoonData(mech.SkillId))
                        {
                            if (condition != null && !condition(new SpecialConditionItem(c)))
                            {
                                continue;
                            }
                            AbstractMasterPlayer amp = null;
                            if (mech.MechanicType == Mechanic.MechType.EnemyBoon && c.IsBuffRemove == ParseEnum.BuffRemove.None)
                            {
                                Target target = Targets.Find(x => x.InstID == c.DstInstid && x.FirstAware <= c.Time && x.LastAware >= c.Time);
                                if (target != null)
                                {
                                    amp = target;
                                }
                                else
                                {
                                    AgentItem a = log.AgentData.GetAgent(c.DstAgent);
                                    if (playersIds.Contains(a.InstID))
                                    {
                                        continue;
                                    }
                                    else if (a.MasterAgent != 0)
                                    {
                                        AgentItem m = log.AgentData.GetAgent(a.MasterAgent);
                                        if (playersIds.Contains(m.InstID))
                                        {
                                            continue;
                                        }
                                    }
                                    if (!regroupedMobs.TryGetValue(a.ID, out amp))
                                    {
                                        amp = new DummyPlayer(a);
                                        regroupedMobs.Add(a.ID, amp);
                                    }
                                }
                            }
                            else if (mech.MechanicType == Mechanic.MechType.EnemyBoonStrip && c.IsBuffRemove == ParseEnum.BuffRemove.Manual)
                            {
                                Target target = Targets.Find(x => x.InstID == c.SrcInstid && x.FirstAware <= c.Time && x.LastAware >= c.Time);
                                if (target != null)
                                { 
                                    amp = target;
                                }
                                else
                                {
                                    AgentItem a = log.AgentData.GetAgent(c.SrcAgent);
                                    if (playersIds.Contains(a.InstID))
                                    {
                                        continue;
                                    }
                                    else if (a.MasterAgent != 0)
                                    {
                                        AgentItem m = log.AgentData.GetAgent(a.MasterAgent);
                                        if (playersIds.Contains(m.InstID))
                                        {
                                            continue;
                                        }
                                    }
                                    if (!regroupedMobs.TryGetValue(a.ID, out amp))
                                    {
                                        amp = new DummyPlayer(a);
                                        regroupedMobs.Add(a.ID, amp);
                                    }
                                }
                            }
                            if (amp != null)
                            {
                                mechData[mech].Add(new MechanicLog(c.Time - fightData.FightStart, mech, amp));
                            }

                        }
                        break;
                    case Mechanic.MechType.EnemyCastEnd:
                    case Mechanic.MechType.EnemyCastStart:
                        condition = mech.SpecialCondition;
                        foreach (CombatItem c in log.GetCastDataById(mech.SkillId))
                        {
                            if (condition != null && !condition(new SpecialConditionItem(c)))
                            {
                                continue;
                            }
                            AbstractMasterPlayer amp = null;
                            if ((mech.MechanicType == Mechanic.MechType.EnemyCastStart && c.IsActivation.IsCasting()) || (mech.MechanicType == Mechanic.MechType.EnemyCastEnd && !c.IsActivation.IsCasting()))
                            {
                                Target target = Targets.Find(x => x.InstID == c.SrcInstid && x.FirstAware <= c.Time && x.LastAware >= c.Time);
                                if (target != null)
                                {
                                    amp = target;
                                }
                                else
                                {
                                    AgentItem a = log.AgentData.GetAgent(c.SrcAgent);
                                    if (playersIds.Contains(a.InstID))
                                    {
                                        continue;
                                    }
                                    else if (a.MasterAgent != 0)
                                    {
                                        AgentItem m = log.AgentData.GetAgent(a.MasterAgent);
                                        if (playersIds.Contains(m.InstID))
                                        {
                                            continue;
                                        }
                                    }
                                    if (!regroupedMobs.TryGetValue(a.ID, out amp))
                                    {
                                        amp = new DummyPlayer(a);
                                        regroupedMobs.Add(a.ID, amp);
                                    }
                                }
                            }
                            if (amp != null)
                            {
                                mechData[mech].Add(new MechanicLog(c.Time - fightData.FightStart, mech, amp));
                            }
                        }
                        break;
                    case Mechanic.MechType.Spawn:
                        foreach (AgentItem a in log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.ID == mech.SkillId))
                        {
                            if (!regroupedMobs.TryGetValue(a.ID, out AbstractMasterPlayer amp))
                            {
                                amp = new DummyPlayer(a);
                                regroupedMobs.Add(a.ID, amp);
                            }
                            mechData[mech].Add(new MechanicLog(a.FirstAware - fightData.FightStart, mech, amp));
                        }
                        break;
                }
            }
            mechData.ComputePresentMechanics(log);
        }

        public virtual void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
        }

        //
        protected static List<CombatItem> GetFilteredList(ParsedLog log, long skillID, AbstractMasterPlayer target, bool beginWithStart = true)
        {
            bool needStart = beginWithStart;
            List<CombatItem> main = log.GetBoonData(skillID).Where(x => ((x.DstInstid == target.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None) || (x.SrcInstid == target.InstID && x.IsBuffRemove != ParseEnum.BuffRemove.None)) && x.Time >= target.FirstAware && x.Time <= target.LastAware).ToList();
            List<CombatItem> filtered = new List<CombatItem>();
            for (int i = 0; i < main.Count; i++)
            {
                CombatItem c = main[i];
                if (needStart && c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c.IsBuffRemove != ParseEnum.BuffRemove.None)
                {
                    // consider only last remove event before another application
                    if ((i == main.Count - 1) || (i < main.Count - 1 && main[i + 1].IsBuffRemove == ParseEnum.BuffRemove.None))
                    {
                        needStart = true;
                        filtered.Add(c);
                    }
                }
            }
            return filtered;
        }

    }
}
