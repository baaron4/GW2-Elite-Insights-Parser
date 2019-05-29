using System;

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
        public long FirstAware { get; set; }
        public long LastAware { get; set; } = long.MaxValue;
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
    }
}