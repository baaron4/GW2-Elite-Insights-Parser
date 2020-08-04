using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using GW2EIUtils;

namespace GW2EIParser.Parser.ParsedData
{
    public class AgentItem
    {

        private static int AgentCount = 0;
        public enum AgentType { NPC, Gadget, Player, EnemyPlayer }

        // Fields
        public ulong Agent { get; }
        public int ID { get; protected set; }
        public int UniqueID { get; }
        public AgentItem Master { get; protected set; }
        public ushort InstID { get; protected set; }
        public AgentType Type { get; protected set; } = AgentType.NPC;
        public long FirstAware { get; protected set; }
        public long LastAware { get; protected set; } = long.MaxValue;
        public string Name { get; protected set; } = "UNKNOWN";
        public string Prof { get; } = "UNKNOWN";
        public uint Toughness { get; protected set; }
        public uint Healing { get;}
        public uint Condition { get; }
        public uint Concentration { get; }
        public uint HitboxWidth { get; }
        public uint HitboxHeight { get; }

        public bool HasCommanderTag { get; protected set; }

        // Constructors
        public AgentItem(ulong agent, string name, string prof, int id, AgentType type, uint toughness, uint healing, uint condition, uint concentration, uint hbWidth, uint hbHeight)
        {
            UniqueID = AgentCount++;
            Agent = agent;
            Name = name;
            Prof = prof;
            ID = id;
            Type = type;
            Toughness = toughness;
            Healing = healing;
            Condition = condition;
            Concentration = concentration;
            HitboxWidth = hbWidth;
            HitboxHeight = hbHeight;
            //
            try
            {
                if (type == AgentType.Player)
                {
                    string[] splitStr = Name.Split('\0');
                    if (splitStr.Length < 2 || (splitStr[1].Length == 0 || splitStr[2].Length == 0 || splitStr[0].Contains("-")))
                    {
                        Type = AgentType.EnemyPlayer;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public AgentItem(ulong agent, string name, string prof, int id, ushort instid, AgentType type, uint toughness, uint healing, uint condition, uint concentration, uint hbWidth, uint hbHeight, long firstAware, long lastAware): this(agent, name, prof, id, type, toughness, healing, condition, concentration, hbWidth, hbHeight)
        {
            InstID = instid;
            FirstAware = firstAware;
            LastAware = lastAware;
        }

        public AgentItem(AgentItem other)
        {
            UniqueID = AgentCount++;
            Agent = other.Agent;
            Name = other.Name;
            Prof = other.Prof;
            ID = other.ID;
            Type = other.Type;
            Toughness = other.Toughness;
            Healing = other.Healing;
            Condition = other.Condition;
            Concentration = other.Concentration;
            HitboxWidth = other.HitboxWidth;
            HitboxHeight = other.HitboxHeight;
            InstID = other.InstID;
            Master = other.Master;
            HasCommanderTag = other.HasCommanderTag;
        }

        public AgentItem()
        {
        }

        public void OverrideType(AgentType type)
        {
            Type = type;
        }

        public void SetInstid(ushort instid)
        {
            InstID = instid;
        }

        public void OverrideID(int id)
        {
            ID = id;
        }

        public void OverrideName(string name)
        {
            Name = name;
        }

        public void OverrideToughness(uint toughness)
        {
            Toughness = toughness;
        }

        public void OverrideAwareTimes(long firstAware, long lastAware)
        {
            FirstAware = firstAware;
            LastAware = lastAware;
        }

        public void SetMaster(AgentItem master )
        {
            Master = master;
        }

        public void SetCommanderTag(TagEvent tagEvt)
        {
            HasCommanderTag = tagEvt.TagID != 0;
        }

        private static void AddValueToStatusList(List<(long start, long end)> dead, List<(long start, long end)> down, List<(long start, long end)> dc, AbstractStatusEvent cur, AbstractStatusEvent next, long endTime, int index)
        {
            long cTime = cur.Time;
            long nTime = next != null ? next.Time : endTime;
            if (cur is DownEvent)
            {
                down.Add((cTime, nTime));
            }
            else if (cur is DeadEvent)
            {
                dead.Add((cTime, nTime));
            }
            else if (cur is DespawnEvent)
            {
                dc.Add((cTime, nTime));
            }
            else if (index == 0)
            {
                if (cur is SpawnEvent)
                {
                    dc.Add((0, cTime));
                }
                else if (cur is AliveEvent)
                {
                    dead.Add((0, cTime));
                }
            }
        }

        public void GetAgentStatus(List<(long start, long end)> dead, List<(long start, long end)> down, List<(long start, long end)> dc, ParsedLog log)
        {
            var status = new List<AbstractStatusEvent>();
            status.AddRange(log.CombatData.GetDownEvents(this));
            status.AddRange(log.CombatData.GetAliveEvents(this));
            status.AddRange(log.CombatData.GetDeadEvents(this));
            status.AddRange(log.CombatData.GetSpawnEvents(this));
            status.AddRange(log.CombatData.GetDespawnEvents(this));
            status = status.OrderBy(x => x.Time).ToList();
            for (int i = 0; i < status.Count - 1; i++)
            {
                AbstractStatusEvent cur = status[i];
                AbstractStatusEvent next = status[i + 1];
                AddValueToStatusList(dead, down, dc, cur, next, log.FightData.FightEnd, i);
            }
            // check last value
            if (status.Count > 0)
            {
                AbstractStatusEvent cur = status.Last();
                AddValueToStatusList(dead, down, dc, cur, null, log.FightData.FightEnd, status.Count - 1);
            }
        }

        public AgentItem GetFinalMaster()
        {
            AgentItem cur = this;
            while (cur.Master != null)
            {
                cur = cur.Master;
            }
            return cur;
        }

        /// <summary>
        /// Checks if a buff is present on the actor that corresponds to. Given buff id must be in the boon simulator
        /// </summary>
        /// <param name="log"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedLog log, long buffId, long time)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            AbstractSingleActor actor = log.FindActor(this, true);
            Dictionary<long, BuffsGraphModel> bgms = actor.GetBuffGraphs(log);
            if (bgms.TryGetValue(buffId, out BuffsGraphModel bgm))
            {
                return bgm.IsPresent(time, GeneralHelper.ServerDelayConstant);
            }
            else
            {
                return false;
            }
        }
    }
}
