using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.Logic
{
    public abstract class FightLogic
    {

        public enum ParseMode { Raid, Fractal, Golem, WvW, Unknown };

        private CombatReplayMap _map;
        protected readonly List<Mechanic> MechanicList; //Resurrects (start), Resurrect
        public ParseMode Mode { get; protected set; } = ParseMode.Unknown;
        public bool HasCombatReplayMap { get; protected set; } = false;
        public string Extension { get; protected set; }
        public string IconUrl { get; protected set; }
        private readonly int _basicMechanicsCount;
        public bool HasNoFightSpecificMechanics => MechanicList.Count == _basicMechanicsCount;
        public List<Mob> TrashMobs { get; } = new List<Mob>();
        public List<Target> Targets { get; } = new List<Target>();
        protected readonly ushort TriggerID;

        protected FightLogic(ushort triggerID, AgentData agentData)
        {
            TriggerID = triggerID;
            HasCombatReplayMap = GetCombatMap() != null;
            MechanicList = new List<Mechanic>() {
                new PlayerStatusMechanic(SkillItem.DeathId, "Dead", new MechanicPlotlySetting("x","rgb(0,0,0)"), "Dead",0),
                new PlayerStatusMechanic(SkillItem.DownId, "Downed", new MechanicPlotlySetting("cross","rgb(255,0,0)"), "Downed",0),
                new PlayerStatusMechanic(SkillItem.ResurrectId, "Resurrect", new MechanicPlotlySetting("cross-open","rgb(0,255,255)"), "Res",0),
                new PlayerStatusMechanic(SkillItem.AliveId, "Got up", new MechanicPlotlySetting("cross","rgb(0,255,0)"), "Got up",0),
                new PlayerStatusMechanic(SkillItem.DCId, "Disconnected", new MechanicPlotlySetting("x","rgb(120,120,120)"), "DC",0),
                new PlayerStatusMechanic(SkillItem.RespawnId, "Respawn", new MechanicPlotlySetting("cross","rgb(120,120,255)"), "Resp",0)
            };
            _basicMechanicsCount = MechanicList.Count;
            List<ParseEnum.TrashIDS> ids = GetTrashMobsIDS();
            List<AgentItem> aList = agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => ids.Contains(ParseEnum.GetTrashIDS(x.ID))).ToList();
            foreach (AgentItem a in aList)
            {
                Mob mob = new Mob(a);
                TrashMobs.Add(mob);
            }
        }

        public MechanicData GetMechanicData()
        {
            return new MechanicData(MechanicList);
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

        private void RegroupTargetsByID(ushort id, AgentData agentData, List<CombatItem> combatItems)
        {
            List<AgentItem> agents = agentData.GetAgentsByID(id);
            List<Target> toRegroup = Targets.Where(x => x.ID == id).ToList();
            if (agents.Count > 1 && toRegroup.Count > 1)
            {
                Targets.RemoveAll(x => x.ID == id);
                AgentItem firstItem = agents.First();
                HashSet<ulong> agentValues = new HashSet<ulong>(agents.Select(x => x.Agent));
                AgentItem newTargetAgent = new AgentItem(firstItem)
                {
                    FirstAwareLogTime = agents.Min(x => x.FirstAwareLogTime),
                    LastAwareLogTime = agents.Max(x => x.LastAwareLogTime)
                };
                // get unique id for the fusion
                ushort instID = 0;
                Random rnd = new Random();
                while (agentData.InstIDValues.Contains(instID) || instID == 0)
                {
                    instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
                }
                newTargetAgent.InstID = instID;
                agentData.OverrideID(id, newTargetAgent);
                Targets.Add(new Target(newTargetAgent));
                foreach (CombatItem c in combatItems)
                {
                    if (agentValues.Contains(c.SrcAgent))
                    {
                        c.OverrideSrcValues(newTargetAgent.Agent, newTargetAgent.InstID);
                    }
                    if (agentValues.Contains(c.DstAgent))
                    {
                        c.OverrideDstValues(newTargetAgent.Agent, newTargetAgent.InstID);
                    }
                }
            }
        }

        protected abstract HashSet<ushort> GetUniqueTargetIDs();

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
            foreach (ushort id in GetUniqueTargetIDs())
            {
                RegroupTargetsByID(id, agentData, combatItems);
            }
        }

        public void SetMaxHealth(ushort instid, long time, int health)
        {
            foreach (Target target in Targets)
            {
                if (target.Health == -1 && target.InstID == instid && target.FirstAwareLogTime <= time && target.LastAwareLogTime >= time)
                {
                    target.Health = health;
                    break;
                }
            }
        }

        protected void OverrideMaxHealths(ParsedEvtcContainer evtcContainer)
        {
            List<CombatItem> maxHUs = evtcContainer.CombatData.GetStates(ParseEnum.StateChange.MaxHealthUpdate);
            if (maxHUs.Count > 0)
            {
                foreach (Target tar in Targets)
                {
                    List<CombatItem> subList = maxHUs.Where(x => x.SrcInstid == tar.InstID && x.LogTime >= tar.FirstAwareLogTime && x.LogTime <= tar.LastAwareLogTime).ToList();
                    if (subList.Count > 0)
                    {
                        tar.Health = subList.Max(x => (int)x.DstAgent);
                    }
                }
            }
        }

        public virtual void AddHealthUpdate(ushort instid, long time, long healthTime, int health)
        {
            foreach (Target target in Targets)
            {
                if (target.InstID == instid && target.FirstAwareLogTime <= time && target.LastAwareLogTime >= time)
                {
                    target.HealthOverTime.Add((healthTime, health));
                    break;
                }
            }
        }

        protected List<PhaseData> GetPhasesByInvul(ParsedLog log, long skillID, Target mainTarget, bool addSkipPhases, bool beginWithStart)
        {
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = new List<PhaseData>();
            long last = 0;
            List<AbstractBuffEvent> invuls = GetFilteredList(log.CombatData, skillID, mainTarget, beginWithStart);
            for (int i = 0; i < invuls.Count; i++)
            {
                AbstractBuffEvent c = invuls[i];
                if (c is BuffApplyEvent)
                {
                    long end = c.Time;
                    phases.Add(new PhaseData(last, end));
                    /*if (i == invuls.Count - 1)
                    {
                        mainTarget.AddCustomCastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None, log);
                    }*/
                    last = end;
                }
                else
                {
                    long end = c.Time;
                    if (addSkipPhases)
                    {
                        phases.Add(new PhaseData(last, end));
                    }
                    //mainTarget.AddCustomCastLog(last, -5, (int)(end - last), ParseEnum.Activation.None, (int)(end - last), ParseEnum.Activation.None, log);
                    last = end;
                }
            }
            if (fightDuration - last > 5000)
            {
                phases.Add(new PhaseData(last, fightDuration));
            }
            return phases;
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
                if (ids.Contains(target.ID) && phase.InInterval(Math.Max(log.FightData.ToFightSpace(target.FirstAwareLogTime),0)))
                {
                    phase.Targets.Add(target);
                }
            }
            phase.OverrideTimes(log);
        }

        public virtual void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
        }

        public virtual void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
        }

        public virtual void ComputeMobCombatReplayActors(Mob mob, ParsedLog log, CombatReplay replay)
        {
        }

        protected virtual List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>();
        }

        public virtual int IsCM(ParsedEvtcContainer evtcContainer)
        {
            return -1;
        }

        protected void SetSuccessByDeath(ParsedEvtcContainer evtcContainer, bool all, ushort idFirst, params ushort[] ids)
        {
            int success = 0;
            long maxTime = long.MinValue;
            List<ushort> idsToUse = new List<ushort>
            {
                idFirst
            };
            idsToUse.AddRange(ids);
            foreach (ushort id in idsToUse)
            {
                Target target = Targets.Find(x => x.ID == id);
                if (target == null)
                {
                    throw new InvalidOperationException("Main target of the fight not found");
                }
                CombatItem killed = evtcContainer.CombatData.GetStatesData(target.InstID, ParseEnum.StateChange.ChangeDead, target.FirstAwareLogTime, target.LastAwareLogTime).LastOrDefault();
                if (killed != null)
                {
                    success++;
                    maxTime = Math.Max(killed.LogTime, maxTime);
                }
            }
            if ((all && success == idsToUse.Count) || (!all && success > 0))
            {
                evtcContainer.FightData.SetSuccess(true, maxTime);
            }
        }

        public virtual void CheckSuccess(ParsedEvtcContainer evtcContainer)
        {
            SetSuccessByDeath(evtcContainer, true, TriggerID);
        }

        public virtual void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
        }

        //
        protected static List<AbstractBuffEvent> GetFilteredList(CombatData combatData, long buffID, AbstractMasterActor target, bool beginWithStart)
        {
            bool needStart = beginWithStart;
            List<AbstractBuffEvent> main = combatData.GetBoonData(buffID).Where(x => x.To == target.AgentItem && (x is BuffApplyEvent || x is BuffRemoveManualEvent)).ToList();
            List<AbstractBuffEvent> filtered = new List<AbstractBuffEvent>();
            for (int i = 0; i < main.Count; i++)
            {
                AbstractBuffEvent c = main[i];
                if (needStart && c is BuffApplyEvent)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c is BuffRemoveManualEvent)
                {
                    // consider only last remove event before another application
                    if ((i == main.Count - 1) || (i < main.Count - 1 && main[i + 1] is BuffApplyEvent))
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
