using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class AgentItem
    {
        // Fields
        private long agent;
        private int instid = 0;
        private int first_aware = 0;
        private int last_aware = Int16.MaxValue;
        private String name;
        private String prof;
        private int toughness = 0;
        private int healing = 0;
        private int condition = 0;

        // Constructors
        public AgentItem(long agent, String name, String prof)
        {
            this.agent = agent;
            this.name = name;
            this.prof = prof;
        }

        public AgentItem(long agent, String name, String prof, int toughness, int healing, int condition)
        {
            this.agent = agent;
            this.name = name;
            this.prof = prof;
            this.toughness = toughness;
            this.healing = healing;
            this.condition = condition;
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
        public long getAgent()
        {
            return agent;
        }

        public int getInstid()
        {
            return instid;
        }

        public int getFirstAware()
        {
            return first_aware;
        }

        public int getLastAware()
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

        // Setters
        public void setInstid(int instid)
        {
            this.instid = instid;
        }

        public void setFirstAware(int first_aware)
        {
            this.first_aware = first_aware;
        }

        public void setLastAware(int last_aware)
        {
            this.last_aware = last_aware;
        }
    }
}