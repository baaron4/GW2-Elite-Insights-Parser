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
        public readonly String Name;
        public readonly String Prof;
        public readonly int Toughness;
        public readonly int Healing;
        public readonly int Condition;
        public readonly int Concentration;
        public readonly int HitboxWidth;
        public readonly int HitboxHeight;

        // Constructors
        public AgentItem(ulong agent, String name, String prof, int toughness, int healing, int condition, int concentration, int hbWidth, int hbHeight)
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

        // Public Methods
        public String[] ToStringArray()
        {
            String[] array = new String[9];
            array[0] = Agent.ToString(); 
            array[1] = InstID.ToString();
            array[2] = FirstAware.ToString();
            array[3] = LastAware.ToString();
            array[4] = Name;
            array[5] = Prof;
            array[6] = Toughness.ToString();
            array[7] = Healing.ToString();
            array[8] = Condition.ToString();
            return array;
        }
    }
}