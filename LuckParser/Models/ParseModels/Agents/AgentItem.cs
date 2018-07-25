using System;

namespace LuckParser.Models.ParseModels
{
    public class AgentItem
    {
        // Fields
        private ulong agent;
        private ushort id = 0;
        private ulong master_agent = 0;
        private ushort instid = 0;
        private long first_aware = 0;
        private long last_aware = long.MaxValue;
        private String name;
        private String prof;
        private int toughness = 0;
        private int healing = 0;
        private int condition = 0;
        private int concentration = 0;

        // Constructors
        public AgentItem(ulong agent, String name, String prof)
        {
            this.agent = agent;
            this.name = name;
            this.prof = prof;
            if (prof.Contains(":"))
            {
                id = UInt16.Parse(this.prof.Split(':')[1]);
            }
        }

        public AgentItem(ulong agent, String name, String prof, int toughness, int healing, int condition, int concentration)
        {
            this.agent = agent;
            this.name = name;
            this.prof = prof;
            this.toughness = toughness;
            this.healing = healing;
            this.condition = condition;
            this.concentration = concentration;
        }

        // Public Methods
        public String[] toStringArray()
        {
            String[] array = new String[9];
            array[0] = string.Format("{0:X}", agent);//Long.toHexString(agent); 
            array[1] = instid.ToString();
            array[2] = first_aware.ToString();
            array[3] = last_aware.ToString();
            array[4] = name;
            array[5] = prof;
            array[6] = toughness.ToString();
            array[7] = healing.ToString();
            array[8] = condition.ToString();
            return array;
        }

        // Getters
        public ulong getAgent()
        {
            return agent;
        }

        public ushort getInstid()
        {
            return instid;
        }

        public long getFirstAware()
        {
            return first_aware;
        }

        public long getLastAware()
        {
            return last_aware;
        }

        public String getName()
        {
           // name = name.Replace("\0", "");
            
            return name;
        }

        public String getProf()
        {
            return prof;
        }

        public ulong getMasterAgent()
        {
            return master_agent;
        }

        public int getToughness()
        {
            return toughness;
        }

        public int getHealing()
        {
            return healing;
        }

        public int getCondition()
        {
            return condition;
        }

        public int getConcentration()
        {
            return concentration;
        }

        public ushort getID()
        {
            return id;
        }

        // Setters
        public void setInstid(ushort instid)
        {
            this.instid = instid;
        }

        public void setMasterAgent(ulong master)
        {
            this.master_agent = master;
        }

        public void setFirstAware(long first_aware)
        {
            this.first_aware = first_aware;
        }

        public void setLastAware(long last_aware)
        {
            this.last_aware = last_aware;
        }
    }
}