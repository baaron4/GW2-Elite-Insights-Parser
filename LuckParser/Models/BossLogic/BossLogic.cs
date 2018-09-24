using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class BossLogic
    {

        public enum ParseMode { Raid, Fractal, Golem, Unknown };

        private CombatReplayMap _map;
        public readonly List<Mechanic> MechanicList = new List<Mechanic> {
            new Mechanic(-2, "Dead", Mechanic.MechType.PlayerStatus, ParseEnum.BossIDS.Unknown, "symbol:'x',color:'rgb(0,0,0)',", "Dead",0),
            new Mechanic(-3, "Downed", Mechanic.MechType.PlayerStatus, ParseEnum.BossIDS.Unknown, "symbol:'cross',color:'rgb(255,0,0)',", "Downed",0),
            new Mechanic(SkillItem.ResurrectId, "Resurrect", Mechanic.MechType.PlayerStatus, ParseEnum.BossIDS.Unknown, "symbol:'cross-open',color:'rgb(0,255,255)',", "Res",0)}; //Resurrects (start), Resurrect
        public ParseMode Mode { get; protected set; } = ParseMode.Unknown;
        public bool CanCombatReplay { get; set; } = false;
        public string Extension { get; protected set; } = "boss";
        public string IconUrl { get; protected set; } = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
        public List<Mob> TrashMobs { get; } = new List<Mob>();
        private ushort _triggerID;

        public BossLogic(ushort triggerID)
        {
            _triggerID = triggerID;
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
                _triggerID
            };
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
            return phases;
        }

        public virtual void ComputeAdditionalBossData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
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

        public virtual void ComputeAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
        }

        public void ComputeTrashMobsData(ParsedLog log, int pollingRate)
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
            CombatItem killed = log.CombatData.GetStatesData(ParseEnum.StateChange.ChangeDead).LastOrDefault(x => x.SrcInstid == log.Boss.InstID);
            if (killed != null)
            {
                log.LogData.Success = true;
                log.FightData.FightEnd = killed.Time;
            }
        }

        public virtual void SetSuccess(ParsedLog log)
        {
            SetSuccessByDeath(log);
        }

        public virtual string GetReplayIcon()
        {
            return "";
        }


        public void ComputeMechanics(ParsedLog log)
        {
            MechanicData mechData = log.MechanicData;
            FightData fightData = log.FightData;
            CombatData combatData = log.CombatData;
            long start = fightData.FightStart;
            long end = fightData.FightEnd;
            Mechanic.CheckSpecialCondition condition;
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
                                case -2:
                                    cList = combatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDead, start, end);
                                    break;
                                case -3:
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
                            List<DamageLog> dls = p.GetDamageTakenLogs(log, 0, fightData.FightDuration);
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
                                foreach (DamageLog dl in p.GetDamageLogs((AbstractPlayer)null, log, 0, log.FightData.FightDuration))
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
                                if (c.DstInstid == log.Boss.InstID)
                                {
                                    amp = log.Boss;
                                }
                                else
                                {
                                    AgentItem a = log.AgentData.GetAgent(c.DstAgent);
                                    if (!regroupedMobs.TryGetValue(a.ID, out amp))
                                    {
                                        amp = new DummyPlayer(a);
                                        regroupedMobs.Add(a.ID, amp);
                                    }
                                }
                            }
                            else if (mech.MechanicType == Mechanic.MechType.EnemyBoonStrip && c.IsBuffRemove == ParseEnum.BuffRemove.Manual)
                            {
                                if (c.SrcInstid == log.Boss.InstID)
                                {
                                    amp = log.Boss;
                                }
                                else
                                {
                                    AgentItem a = log.AgentData.GetAgent(c.SrcAgent);
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
                                if (c.SrcInstid == log.Boss.InstID)
                                {
                                    amp = log.Boss;
                                }
                                else
                                {
                                    AgentItem a = log.AgentData.GetAgent(c.SrcAgent);
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

        public virtual void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, Boss boss)
        {
        }

        //
        protected static List<CombatItem> GetFilteredList(ParsedLog log, long skillID, ushort instid)
        {
            bool needStart = true;
            List<CombatItem> main = log.GetBoonData(skillID).Where(x => ((x.DstInstid == instid && x.IsBuffRemove == ParseEnum.BuffRemove.None) || (x.SrcInstid == instid && x.IsBuffRemove != ParseEnum.BuffRemove.None))).ToList();
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
