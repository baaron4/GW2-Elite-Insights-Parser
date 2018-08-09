using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BossData
    {
        // Fields
        private ulong agent = 0;
        private ushort instid = 0;
        private BossStrategy strategy;
        private long first_aware = 0;
        private long last_aware = long.MaxValue;
        private ushort id;
        private String name = "UNKNOWN";
        private int health = -1;
        private int toughness = -1;
        private List<Point> healthOverTime = new List<Point>();
        private bool isCM = false;
        // Constructors
        public BossData(ushort id)
        {
            this.id = id;
            switch (ParseEnum.getBossIDS(id))
            {
                case ParseEnum.BossIDS.ValeGuardian:
                    strategy = new ValeGuardian();
                    break;
                case ParseEnum.BossIDS.Gorseval:
                    strategy = new Gorseval();
                    break;
                case ParseEnum.BossIDS.Sabetha:
                    strategy = new Sabetha();
                    break;
                case ParseEnum.BossIDS.Slothasor:
                    strategy = new Slothasor();
                    break;
                case ParseEnum.BossIDS.Matthias:
                    strategy = new Matthias();
                    break;
                case ParseEnum.BossIDS.KeepConstruct:
                    strategy = new KeepConstruct();
                    break;
                case ParseEnum.BossIDS.Xera:
                    strategy = new Xera();
                    break;
                case ParseEnum.BossIDS.Cairn:
                    strategy = new Cairn();
                    break;
                case ParseEnum.BossIDS.MursaatOverseer:
                    strategy = new MursaatOverseer();
                    break;
                case ParseEnum.BossIDS.Samarog:
                    strategy = new Samarog();
                    break;
                case ParseEnum.BossIDS.Deimos:
                    strategy = new Deimos();
                    break;
                case ParseEnum.BossIDS.SoullessHorror:
                    strategy = new SoullessHorror();
                    break;
                case ParseEnum.BossIDS.Dhuum:
                    strategy = new Dhuum();
                    break;
                case ParseEnum.BossIDS.MAMA:
                    strategy = new MAMA();
                    break;
                case ParseEnum.BossIDS.Siax:
                    strategy = new Siax();
                    break;
                case ParseEnum.BossIDS.Ensolyss:
                    strategy = new Ensolyss();
                    break;
                case ParseEnum.BossIDS.Skorvald:
                    strategy = new Skorvald();
                    break;
                case ParseEnum.BossIDS.Artsariiv:
                    strategy = new Artasariiv();
                    break;
                case ParseEnum.BossIDS.Arkk:
                    strategy = new Arkk();
                    break;
                case ParseEnum.BossIDS.Golem1:
                case ParseEnum.BossIDS.Golem2:
                case ParseEnum.BossIDS.Golem3:
                case ParseEnum.BossIDS.Golem4:
                case ParseEnum.BossIDS.Golem5:
                    strategy = new Golem();
                    break;
                default:
                    // Unknown
                    strategy = new BossStrategy();
                    break;
            }
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

        public BossStrategy getBossBehavior()
        {
            return strategy;
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
            toughness = tough;
        }
        public void setHealthOverTime(List<Point> hot) {
            healthOverTime = hot;
        }
        public bool getCM()
        {
            return isCM;
        }
        public void setCM(List<CombatItem> clist)
        {
            isCM = false;
            switch(ParseEnum.getBossIDS(id))
            {
                // Cairn
                case ParseEnum.BossIDS.Cairn:
                    isCM = clist.Exists(x => x.getSkillID() == 38098);
                    break;
                // MO
                case ParseEnum.BossIDS.MursaatOverseer:
                    isCM = (health > 25e6);
                    break;
                // Samarog
                case ParseEnum.BossIDS.Samarog:
                    isCM = (health > 30e6);
                    break;
                // Deimos
                case ParseEnum.BossIDS.Deimos:
                    isCM = (health > 40e6);
                    break;
                // SH
                case ParseEnum.BossIDS.SoullessHorror:
                    List<CombatItem> necrosis = clist.Where(x => x.getSkillID() == 47414 && x.isBuffremove() == ParseEnum.BuffRemove.None).ToList();
                    if (necrosis.Count == 0)
                    {
                        break;
                    }
                    // split necrosis
                    Dictionary<ushort, List<CombatItem>> splitNecrosis = new Dictionary<ushort, List<CombatItem>>();
                    foreach (CombatItem c in necrosis)
                    {
                        ushort inst = c.getDstInstid();
                        if (!splitNecrosis.ContainsKey(inst))
                        {
                            splitNecrosis.Add(inst, new List<CombatItem>());
                        }
                        splitNecrosis[inst].Add(c);
                    }
                    List<CombatItem> longestNecrosis = splitNecrosis.Values.First(l => l.Count == splitNecrosis.Values.Max(x => x.Count));
                    long minDiff = long.MaxValue;
                    for (int i = 0; i < longestNecrosis.Count - 1; i++)
                    {
                        CombatItem cur = longestNecrosis[i];
                        CombatItem next = longestNecrosis[i + 1];
                        long timeDiff = next.getTime() - cur.getTime();
                        if (timeDiff > 1000 && minDiff > timeDiff)
                        {
                            minDiff = timeDiff;
                        }
                    }
                    isCM = minDiff < 11000;
                    break;
                // Dhuum
                case ParseEnum.BossIDS.Dhuum:
                    isCM = (health > 35e6);
                    break;
                // Skorvald
                case ParseEnum.BossIDS.Skorvald:
                    isCM = (health == 5551340);
                    break;
            }
            if (isCM && !name.Contains("CM"))
            {
                name += " CM";
            }
        }
    }
}