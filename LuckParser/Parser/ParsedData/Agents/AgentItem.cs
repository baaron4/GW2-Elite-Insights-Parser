using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.Parser.ParsedData
{
    public class AgentItem
    {

        public enum AgentType { NPC, Gadget, Player, EnemyPlayer }

        // Fields
        public ulong Agent { get; set; }
        public ushort ID { get; }
        public AgentItem MasterAgent { get; set; }
        public ushort InstID { get; set; }
        public AgentType Type { get; } = AgentType.NPC;
        public long FirstAwareLogTime { get; set; }
        public long LastAwareLogTime { get; set; } = long.MaxValue;
        public string Name { get; } = "UNKNOWN";
        public string Prof { get; } = "UNKNOWN";
        public uint Toughness { get; }
        public uint Healing { get; }
        public uint Condition { get; }
        public uint Concentration { get; }
        public uint HitboxWidth { get; }
        public uint HitboxHeight { get; }

        // Constructors
        public AgentItem(ulong agent, string name, string prof, ushort id, AgentType type, uint toughness, uint healing, uint condition, uint concentration, uint hbWidth, uint hbHeight)
        {
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

        public AgentItem(AgentItem other)
        {
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
            MasterAgent = other.MasterAgent;
        }

        public AgentItem()
        {
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
                AddValueToStatusList(dead, down, dc, cur, next, log.FightData.FightDuration, i);
            }
            // check last value
            if (status.Count > 0)
            {
                AbstractStatusEvent cur = status.Last();
                AddValueToStatusList(dead, down, dc, cur, null, log.FightData.FightDuration, status.Count - 1);
            }
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
            if (!log.Boons.BoonsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            AbstractActor actor = log.FindActor(this);
            Dictionary<long, BoonsGraphModel> bgms = actor.GetBoonGraphs(log);
            if (bgms.TryGetValue(buffId, out BoonsGraphModel bgm))
            {
                return bgm.IsPresent(time, 10);
            }
            else
            {
                return false;
            }
        }
    }
}
