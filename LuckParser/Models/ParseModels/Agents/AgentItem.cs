using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class AgentItem
    {

        public enum AgentType { NPC, Gadget, Player }

        // Fields
        public ulong Agent { get; set; }
        public readonly ushort ID;
        public AgentItem MasterAgent { get; set; }
        public ushort InstID { get; set; }
        public AgentType Type { get; }
        public long FirstAwareLogTime { get; set; }
        public long LastAwareLogTime { get; set; } = long.MaxValue;
        public readonly string Name;
        public readonly string Prof;
        public readonly uint Toughness;
        public readonly uint Healing;
        public readonly uint Condition;
        public readonly uint Concentration;
        public readonly uint HitboxWidth;
        public readonly uint HitboxHeight;

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

        public AgentItem(ulong agent, string name)
        {
            Agent = agent;
            Name = name;
        }


        public void GetAgentStatus(List<(long start, long end)> dead, List<(long start, long end)> down, List<(long start, long end)> dc, ParsedLog log)
        {
            List<AbstractStatusEvent> status = new List<AbstractStatusEvent>();
            status.AddRange(log.CombatData.GetDownEvents(this));
            status.AddRange(log.CombatData.GetAliveEvents(this));
            status.AddRange(log.CombatData.GetDeadEvents(this));
            status.AddRange(log.CombatData.GetSpawnEvents(this));
            status.AddRange(log.CombatData.GetDespawnEvents(this));
            status = status.OrderBy(x => x.Time).ToList();
            for (var i = 0; i < status.Count - 1; i++)
            {
                AbstractStatusEvent cur = status[i];
                AbstractStatusEvent next = status[i + 1];
                if (cur is DownEvent)
                {
                    down.Add((cur.Time, next.Time));
                }
                else if (cur is DeadEvent)
                {
                    dead.Add((cur.Time, next.Time));
                }
                else if (cur is DespawnEvent)
                {
                    dc.Add((cur.Time, next.Time));
                }
            }
            // check last value
            if (status.Count > 0)
            {
                AbstractStatusEvent cur = status.Last();
                if (cur is DownEvent)
                {
                    down.Add((cur.Time, log.FightData.FightDuration));
                }
                else if (cur is DeadEvent)
                {
                    dead.Add((cur.Time, log.FightData.FightDuration));
                }
                else if (cur is DespawnEvent)
                {
                    dc.Add((cur.Time, log.FightData.FightDuration));
                }
            }
        }

        /// <summary>
        /// Checks if a buff is present on the actor that corresponds to 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedLog log, long buffId, long time)
        {
            AbstractActor actor = log.FindActor(this);
            Dictionary<long, BoonsGraphModel> bgms = actor.GetBoonGraphs(log);
            if (bgms.TryGetValue(buffId, out var bgm))
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