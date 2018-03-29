using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class BossData
    {
        // Fields
        private long agent = 0;
        private int instid = 0;
        private int first_aware = 0;
        private int last_aware = Int16.MaxValue;
        private int id;
        private String name = "UNKNOWN";
        private int health = -1;
        private int toughness = -1;

        // Constructors
        public BossData(int id)
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

        public int getID()
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

        // Setters
        public void setAgent(long agent)
        {
            this.agent = agent;
        }

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

        public void setName(String name)
        {
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
    }
}