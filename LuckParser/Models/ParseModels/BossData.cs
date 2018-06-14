using System;
using System.Collections.Generic;
using System.Drawing;

namespace LuckParser.Models.ParseModels
{
    public class BossData
    {
        // Fields
        private ulong agent = 0;
        private ushort instid = 0;
        private long first_aware = 0;
        private long last_aware = long.MaxValue;
        private ushort id;
        private String name = "UNKNOWN";
        private int health = -1;
        private int toughness = -1;
        private List<Point> healthOverTime = new List<Point>();
        // Constructors
        public BossData(ushort id)
        {
            this.id = id;
        }

        public String[] toStringArray()
        {
            String[] array = new String[7];
            array[0] = string.Format("{0:X}", agent); ;
            array[1] =instid.ToString();
            array[2] = first_aware.ToString();
            array[3] = last_aware.ToString();
            array[4] =id.ToString();
            array[5] = name;
            array[6] = health.ToString();
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

        public long getAwareDuration()
        {
            return last_aware - first_aware;
        }

        public ushort getID()
        {
            return id;
        }

        public String getName()
        {
          
            return name;
        }

        public int getHealth()
        {
            return health;
        }
        public int getTough()
        {
            return toughness;
        }

        public List<Point> getHealthOverTime() {
            return healthOverTime;
        }
        // Setters
        public void setAgent(ulong agent)
        {
            this.agent = agent;
        }

        public void setInstid(ushort instid)
        {
            this.instid = instid;
        }

        public void setFirstAware(long first_aware)
        {
            this.first_aware = first_aware;
        }

        public void setLastAware(long last_aware)
        {
            this.last_aware = last_aware;
        }

        public void setName(String name)
        {
            name = name.Replace("\0", "");
            this.name = name;
        }

        public void setHealth(int health)
        {
            this.health = health;
        }
        public void setTough(int tough)
        {
            this.toughness = tough;
        }
        public void setHealthOverTime(List<Point> hot) {
            this.healthOverTime = hot;
        }
    }
}