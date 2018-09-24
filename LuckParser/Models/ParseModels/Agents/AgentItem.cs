using System;

namespace LuckParser.Models.ParseModels
{
    public class AgentItem
    {
        // Fields
        public readonly ulong Agent;
        public readonly ushort ID;
        public ulong MasterAgent { get; set; }
        public ushort InstID { get; set; }
        public long FirstAware { get; set; }
        public long LastAware { get; set; } = long.MaxValue;
        public readonly string Name;
        public readonly string Prof;
        public readonly int Toughness;
        public readonly int Healing;
        public readonly int Condition;
        public readonly int Concentration;
        public readonly int HitboxWidth;
        public readonly int HitboxHeight;

        // Constructors
        public AgentItem(ulong agent, string name, string prof, int toughness, int healing, int condition, int concentration, int hbWidth, int hbHeight)
        {
            Agent = agent;
            Name = name;
            Prof = prof;
            if (prof.Contains(":"))
            {
                var splitted = Prof.Split(':');
                try
                {
                    ID = UInt16.Parse(splitted[splitted.Length - 1]);
                }
                catch(FormatException)
                {
                    ID = 0;
                }
            }
            Toughness = toughness;
            Healing = healing;
            Condition = condition;
            Concentration = concentration;
            HitboxWidth = hbWidth;
            HitboxHeight = hbHeight;
        }
    }
}