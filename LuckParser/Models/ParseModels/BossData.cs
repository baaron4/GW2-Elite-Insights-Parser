using System;
using System.Collections.Generic;
using System.Drawing;

namespace LuckParser.Models.ParseModels
{
    public class BossData
    {
        // Fields
        private long agent = 0;
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

        // Public Methods
        public String[] getPhaseNames()
        {
            if (name ==("Vale Guardian"))
            {
                return new String[] { "100% - 66%", "66% - 33%", "33% - 0%" };
            }
            else if (name==("Gorseval the Multifarious"))
            {
                return new String[] { "100% - 66%", "66% - 33%", "33% - 0%" };
            }
            else if (name==("Sabetha the Saboteur"))
            {
                return new String[] { "100% - 75%", "75% - 50%", "50% - 25%", "25% - 0%" };
            }
            else if (name==("Slothasor"))
            {
                return new String[] { "100% - 80%", "80% - 60%", "60% - 40%", "40% - 20%", "20% - 10%", "10% - 0%" };
            }
            else if (name==("Matthias Gabrel"))
            {
                return new String[] { "100% - 80%", "80% - 60%", "60% - 40%", "40% - 0%" };
            }
            else if (name==("Keep Construct"))
            {
                return new String[] { "100% - 66%", "66% - 33%", "33% - 0%" };
            }
            else if (name==("Xera"))
            {
                return new String[] { "100% - 50%", "50% - 0%" };
            }
            else if (name==("Cairn the Indomitable"))
            {
                return new String[] { "100% - 75%", "75% - 50%", "50% - 25%", "25% - 0%" };
            }
            else if (name==("Mursaat Overseer"))
            {
                return new String[] { "100% - 75%", "75% - 50%", "50% - 25%", "25% - 0%" };
            }
            else if (name==("Samarog"))
            {
                return new String[] { "100% - 66%", "66% - 33%", "33% - 0%" };
            }
            else if (name==("Deimos"))
            {
                return new String[] { "100% - 75%", "75% - 50%", "50% - 25%", "25% - 10%" };
            }
            return new String[] { "100% - 0%" };
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
        public long getAgent()
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
        public void setAgent(long agent)
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