using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractActor
    {
        public readonly AgentItem AgentItem;
        public readonly string Character;
        // Damage
        protected readonly List<DamageLog> DamageLogs = new List<DamageLog>();
        protected Dictionary<ushort,List<DamageLog>> DamageLogsByDst = new Dictionary<ushort, List<DamageLog>>();
        //protected List<DamageLog> HealingLogs = new List<DamageLog>();
        //protected List<DamageLog> HealingReceivedLogs = new List<DamageLog>();
        private readonly List<DamageLog> _damageTakenlogs = new List<DamageLog>();
        protected Dictionary<ushort, List<DamageLog>> _damageTakenLogsBySrc = new Dictionary<ushort, List<DamageLog>>();
        // Cast
        protected readonly List<CastLog> CastLogs = new List<CastLog>();
        // Boons
        public HashSet<Boon> TrackedBoons { get; } = new HashSet<Boon>();
        protected readonly List<BoonDistribution> BoonDistribution = new List<BoonDistribution>();
        protected readonly Dictionary<long, BoonsGraphModel> BoonPoints = new Dictionary<long, BoonsGraphModel>();

        public uint Toughness => AgentItem.Toughness;
        public uint Condition => AgentItem.Condition;
        public uint Concentration => AgentItem.Concentration;
        public uint Healing => AgentItem.Healing;
        public ushort InstID => AgentItem.InstID;
        public string Prof => AgentItem.Prof;
        public ulong Agent => AgentItem.Agent;
        public long LastAware => AgentItem.LastAware;
        public long FirstAware => AgentItem.FirstAware;
        public ushort ID => AgentItem.ID;
        public uint HitboxHeight => AgentItem.HitboxHeight;
        public uint HitboxWidth => AgentItem.HitboxWidth;

        protected AbstractActor(AgentItem agent)
        {
            string[] name = agent.Name.Split('\0');
            Character = name[0];
            AgentItem = agent;
            if (GeneralHelper.AllActors.TryGetValue(agent, out var actor))
            {
                if (actor.GetType() == typeof(DummyActor))
                {
                    GeneralHelper.AllActors[agent] = this;
                }
            }
            else
            {
                GeneralHelper.AllActors.Add(agent, this);
            }
        }
        // Getters

        public long GetDeath(ParsedLog log, long start, long end)
        {
            CombatItem dead = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.ChangeDead, Math.Max(log.FightData.ToLogSpace(start), FirstAware), Math.Min(log.FightData.ToLogSpace(end), LastAware)).LastOrDefault();
            if (dead != null && dead.Time > 0)
            {
                return log.FightData.ToFightSpace(dead.Time);
            }
            return 0;
        }

        public List<DamageLog> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (DamageLogs.Count == 0)
            {
                SetDamageLogs(log);
                DamageLogsByDst = DamageLogs.GroupBy(x => x.DstInstId).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageLogsByDst.TryGetValue(target.InstID, out var list))
                {
                    long targetStart = log.FightData.ToFightSpace(target.FirstAware);
                    long targetEnd = log.FightData.ToFightSpace(target.LastAware);
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<DamageLog>();
                }
            }
            return DamageLogs.Where( x => x.Time >= start && x.Time <= end).ToList();
        }
        public List<DamageLog> GetDamageTakenLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (_damageTakenlogs.Count == 0)
            {
                SetDamageTakenLogs(log);
                _damageTakenLogsBySrc = _damageTakenlogs.GroupBy(x => x.SrcInstId).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (_damageTakenLogsBySrc.TryGetValue(target.InstID, out var list))
                {
                    long targetStart = log.FightData.ToFightSpace(target.FirstAware);
                    long targetEnd = log.FightData.ToFightSpace(target.LastAware);
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<DamageLog>();
                }
            }
            return _damageTakenlogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public BoonDistribution GetBoonDistribution(ParsedLog log, int phaseIndex)
        {
            if (BoonDistribution.Count == 0)
            {
                SetBoonDistribution(log);
            }
            return BoonDistribution[phaseIndex];
        }
        public Dictionary<long, BoonsGraphModel> GetBoonGraphs(ParsedLog log)
        {
            if (BoonDistribution.Count == 0)
            {
                SetBoonDistribution(log);
            }
            return BoonPoints;
        }
        /*public List<DamageLog> getHealingLogs(ParsedLog log, long start, long end)//isntid = 0 gets all logs if specified sets and returns filtered logs
        {
            if (healingLogs.Count == 0)
            {
                setHealingLogs(log);
            }
            return healingLogs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }
        public List<DamageLog> getHealingReceivedLogs(ParsedLog log, long start, long end)
        {
            if (healingReceivedLogs.Count == 0)
            {
                setHealingReceivedLogs(log);
            }
            return healingReceivedLogs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }*/
        public List<CastLog> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (CastLogs.Count == 0)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time >= start && x.Time <= end).ToList();

        }

        public List<CastLog> GetCastLogsActDur(ParsedLog log, long start, long end)
        {
            if (CastLogs.Count == 0)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time + x.ActualDuration >= start && x.Time <= end).ToList();

        }
        // privates
        protected void AddDamageLog(long time, CombatItem c)
        {        
            if (c.IFF == ParseEnum.IFF.Friend)
            {
                return;
            }
            if (c.IsBuff != 0)//condi
            {
                DamageLogs.Add(new DamageLogCondition(time, c));
            }
            else if (c.IsBuff == 0)//power
            {
                DamageLogs.Add(new DamageLogPower(time, c));
            }
            else if (c.Result == ParseEnum.Result.Absorb || c.Result == ParseEnum.Result.Blind || c.Result == ParseEnum.Result.Interrupt)
            {//Hits that where blinded, invulned, interrupts
                DamageLogs.Add(new DamageLogPower(time, c));
            }


        }
        protected void AddDamageTakenLog(long time, CombatItem c)
        {
            if (c.IsBuff != 0)
            {
                //inco,ing condi dmg not working or just not present?
                // damagetaken.Add(c.getBuffDmg());
                _damageTakenlogs.Add(new DamageLogCondition(time, c));
            }
            else if (c.IsBuff == 0)
            {
                _damageTakenlogs.Add(new DamageLogPower(time, c));

            }
        }

        protected static void Add<T>(Dictionary<T, long> dictionary, T key, long value)
        {
            if (dictionary.TryGetValue(key, out var existing))
            {
                dictionary[key] = existing + value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        private ushort TryFindSrc(List<CastLog> castsToCheck, long time, long extension, ParsedLog log)
        {
            HashSet<long> idsToCheck = new HashSet<long>();
            switch (extension)
            {
                // SoI
                case 5000:
                    idsToCheck.Add(10236);
                    break;
                // Treated True Nature
                case 3000:
                    idsToCheck.Add(51696);
                    break;
                // Sand Squall, True Nature, Soulbeast trait
                case 2000:
                    if (Prof == "Soulbeast")
                    {
                        if (log.PlayerListBySpec.ContainsKey("Herald") || log.PlayerListBySpec.ContainsKey("Tempest"))
                        {
                            return 0;
                        }
                        // if not herald or tempest in squad then can only be the trait
                        return InstID;
                    }
                    idsToCheck.Add(51696);
                    idsToCheck.Add(29453);
                    break;

            }
            List<CastLog> cls = castsToCheck.Where(x => idsToCheck.Contains(x.SkillId) && x.Time <= time && time <= x.Time + x.ActualDuration + 10 && x.EndActivation.NoInterruptEndCasting()).ToList();
            if (cls.Count == 1)
            {
                CastLog item = cls.First();
                if (extension == 2000 && log.PlayerListBySpec.TryGetValue("Tempest", out List<Player> tempests))
                {
                    List<CombatItem> magAuraApplications = log.GetBoonData(5684).Where(x => x.IsBuffRemove == ParseEnum.BuffRemove.None && x.IsOffcycle == 0).ToList();
                    foreach (Player tempest in tempests)
                    {
                        if (magAuraApplications.FirstOrDefault(x => x.SrcInstid == tempest.InstID && Math.Abs(x.Time - time) < 50) != null)
                        {
                            return 0;
                        }
                    }
                }
                return item.SrcInstId;
            }
            return 0;
        }

        protected BoonMap GetBoonMap(ParsedLog log)
        {
            // buff extension ids
            HashSet<long> idsToCheck = new HashSet<long>()
            {
                10236,
                51696,
                29453
            };
            List<CastLog> extensionSkills = new List<CastLog>();
            foreach (Player p in log.PlayerList)
            {
                extensionSkills.AddRange(p.GetCastLogs(log, log.FightData.ToFightSpace(p.FirstAware), log.FightData.ToFightSpace(p.LastAware)).Where(x => idsToCheck.Contains(x.SkillId)));
            }
            //
            BoonMap boonMap = new BoonMap();
            // Fill in Boon Map
            foreach (CombatItem c in log.GetBoonDataByDst(InstID, FirstAware, LastAware))
            {
                long boonId = c.SkillID;
                if (!boonMap.ContainsKey(boonId))
                {
                    if (!Boon.BoonsByIds.ContainsKey(boonId))
                    {
                        continue;
                    }
                    boonMap.Add(Boon.BoonsByIds[boonId]);
                }
                if (c.IsBuffRemove == ParseEnum.BuffRemove.Manual
                    || (c.IsBuffRemove == ParseEnum.BuffRemove.Single && c.IFF == ParseEnum.IFF.Unknown && c.DstInstid == 0)
                    || (c.IsBuffRemove != ParseEnum.BuffRemove.None && c.Value <= 50))
                {
                    continue;
                }
                long time = log.FightData.ToFightSpace(c.Time);
                List<BoonLog> loglist = boonMap[boonId];
                if (c.IsStateChange == ParseEnum.StateChange.BuffInitial)
                {
                    ushort src = c.SrcMasterInstid > 0 ? c.SrcMasterInstid : c.SrcInstid;
                    loglist.Add(new BoonApplicationLog(time, src, c.Value));
                }
                else if (c.IsStateChange != ParseEnum.StateChange.BuffInitial)
                {
                    if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                    {
                        ushort src = c.SrcMasterInstid > 0 ? c.SrcMasterInstid : c.SrcInstid;
                        if (c.IsOffcycle > 0)
                        {
                            if (src == 0)
                            {
                                src = TryFindSrc(extensionSkills, time, c.Value, log);
                            }
                            loglist.Add(new BoonExtensionLog(time, c.Value, c.OverstackValue - c.Value, src));
                        }
                        else
                        {
                            loglist.Add(new BoonApplicationLog(time, src, c.Value));
                        }
                    }
                    else if (time < log.FightData.FightDuration - 50)
                    {
                        loglist.Add(new BoonRemovalLog(time, c.DstInstid, c.Value, c.IsBuffRemove));
                    }
                }
            }
            //boonMap.Sort();
            foreach (var pair in boonMap)
            {
                TrackedBoons.Add(Boon.BoonsByIds[pair.Key]);
            }
            return boonMap;
        }


        /*protected void addHealingLog(long time, CombatItem c)
        {
            if (c.isBuffremove() == ParseEnum.BuffRemove.None)
            {
                if (c.isBuff() == 1 && c.getBuffDmg() != 0)//boon
                {
                    healing_logs.Add(new DamageLogCondition(time, c));
                }
                else if (c.isBuff() == 0 && c.getValue() != 0)//skill
                {
                    healing_logs.Add(new DamageLogPower(time, c));
                }
            }

        }
        protected void addHealingReceivedLog(long time, CombatItem c)
        {
            if (c.isBuff() == 1 && c.getBuffDmg() != 0)
            {
                healing_received_logs.Add(new DamageLogCondition(time, c));
            }
            else if (c.isBuff() == 0 && c.getValue() >= 0)
            {
                healing_received_logs.Add(new DamageLogPower(time, c));

            }
        }*/
        // Setters
        protected abstract void SetDamageLogs(ParsedLog log);     
        protected abstract void SetCastLogs(ParsedLog log);
        protected abstract void SetDamageTakenLogs(ParsedLog log);
        protected abstract void SetBoonDistribution(ParsedLog log);
        protected virtual void GenerateExtraBoonData(ParsedLog log, long boonid, GenerationSimulationResult buffSimulationGeneration, List<PhaseData> phases)
        {
            
        }
        //protected abstract void setHealingLogs(ParsedLog log);
        //protected abstract void setHealingReceivedLogs(ParsedLog log);
    }
}
