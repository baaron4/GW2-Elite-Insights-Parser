using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
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
        internal AgentItem(ulong agent, string name, string prof, int id, AgentType type, uint toughness, uint healing, uint condition, uint concentration, uint hbWidth, uint hbHeight)
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
                        Name = Prof + " " + Name;
                        Type = AgentType.EnemyPlayer;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        internal AgentItem(ulong agent, string name, string prof, int id, ushort instid, AgentType type, uint toughness, uint healing, uint condition, uint concentration, uint hbWidth, uint hbHeight, long firstAware, long lastAware): this(agent, name, prof, id, type, toughness, healing, condition, concentration, hbWidth, hbHeight)
        {
            InstID = instid;
            FirstAware = firstAware;
            LastAware = lastAware;
        }

        internal AgentItem(AgentItem other)
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

        internal AgentItem()
        {
        }

        internal void OverrideType(AgentType type)
        {
            Type = type;
        }

        internal void SetInstid(ushort instid)
        {
            InstID = instid;
        }

        internal void OverrideID(int id)
        {
            ID = id;
        }

        internal void OverrideName(string name)
        {
            Name = name;
        }

        internal void OverrideToughness(uint toughness)
        {
            Toughness = toughness;
        }

        internal void OverrideAwareTimes(long firstAware, long lastAware)
        {
            FirstAware = firstAware;
            LastAware = lastAware;
        }

        internal void SetMaster(AgentItem master )
        {
            Master = master;
        }

        internal void SetCommanderTag(TagEvent tagEvt)
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

        internal void GetAgentStatus(List<(long start, long end)> dead, List<(long start, long end)> down, List<(long start, long end)> dc, CombatData combatData, FightData fightData)
        {
            var status = new List<AbstractStatusEvent>();
            status.AddRange(combatData.GetDownEvents(this));
            status.AddRange(combatData.GetAliveEvents(this));
            status.AddRange(combatData.GetDeadEvents(this));
            status.AddRange(combatData.GetSpawnEvents(this));
            status.AddRange(combatData.GetDespawnEvents(this));
            status = status.OrderBy(x => x.Time).ToList();
            for (int i = 0; i < status.Count - 1; i++)
            {
                AbstractStatusEvent cur = status[i];
                AbstractStatusEvent next = status[i + 1];
                AddValueToStatusList(dead, down, dc, cur, next, fightData.FightEnd, i);
            }
            // check last value
            if (status.Count > 0)
            {
                AbstractStatusEvent cur = status.Last();
                AddValueToStatusList(dead, down, dc, cur, null, fightData.FightEnd, status.Count - 1);
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

        public bool InAwareTimes(long time)
        {
            return FirstAware <= time && LastAware >= time;
        }

        /// <summary>
        /// Checks if a buff is present on the actor that corresponds to. Given buff id must be in the boon simulator
        /// </summary>
        /// <param name="log"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedEvtcLog log, long buffId, long time)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            AbstractSingleActor actor = log.FindActor(this, true);
            Dictionary<long, BuffsGraphModel> bgms = actor.GetBuffGraphs(log);
            if (bgms.TryGetValue(buffId, out BuffsGraphModel bgm))
            {
                return bgm.IsPresent(time, ParserHelper.ServerDelayConstant);
            }
            else
            {
                return false;
            }
        }
    }
}
