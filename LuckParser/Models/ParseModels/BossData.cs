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
        private BossLogic logic;
        private long first_aware = 0;
        private long last_aware = long.MaxValue;
        private ushort id;
        private String name = "UNKNOWN";
        private int health = -1;
        private int toughness = -1;
        private List<Point> healthOverTime = new List<Point>();
        private int isCM = -1;
        // Constructors
        public BossData(ushort id)
        {
            this.id = id;
            switch (ParseEnum.GetBossIDS(id))
            {
                case ParseEnum.BossIDS.ValeGuardian:
                    logic = new ValeGuardian();
                    break;
                case ParseEnum.BossIDS.Gorseval:
                    logic = new Gorseval();
                    break;
                case ParseEnum.BossIDS.Sabetha:
                    logic = new Sabetha();
                    break;
                case ParseEnum.BossIDS.Slothasor:
                    logic = new Slothasor();
                    break;
                case ParseEnum.BossIDS.Matthias:
                    logic = new Matthias();
                    break;
                case ParseEnum.BossIDS.KeepConstruct:
                    logic = new KeepConstruct();
                    break;
                case ParseEnum.BossIDS.Xera:
                    logic = new Xera();
                    break;
                case ParseEnum.BossIDS.Cairn:
                    logic = new Cairn();
                    break;
                case ParseEnum.BossIDS.MursaatOverseer:
                    logic = new MursaatOverseer();
                    break;
                case ParseEnum.BossIDS.Samarog:
                    logic = new Samarog();
                    break;
                case ParseEnum.BossIDS.Deimos:
                    logic = new Deimos();
                    break;
                case ParseEnum.BossIDS.SoullessHorror:
                    logic = new SoullessHorror();
                    break;
                case ParseEnum.BossIDS.Dhuum:
                    logic = new Dhuum();
                    break;
                case ParseEnum.BossIDS.MAMA:
                    logic = new MAMA();
                    break;
                case ParseEnum.BossIDS.Siax:
                    logic = new Siax();
                    break;
                case ParseEnum.BossIDS.Ensolyss:
                    logic = new Ensolyss();
                    break;
                case ParseEnum.BossIDS.Skorvald:
                    logic = new Skorvald();
                    break;
                case ParseEnum.BossIDS.Artsariiv:
                    logic = new Artasariiv();
                    break;
                case ParseEnum.BossIDS.Arkk:
                    logic = new Arkk();
                    break;
                case ParseEnum.BossIDS.Golem1:
                case ParseEnum.BossIDS.Golem2:
                case ParseEnum.BossIDS.Golem3:
                case ParseEnum.BossIDS.Golem4:
                case ParseEnum.BossIDS.Golem5:
                    logic = new Golem();
                    break;
                default:
                    // Unknown
                    logic = new BossLogic();
                    break;
            }
        }

        public String[] ToStringArray()
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
        public ulong GetAgent()
        {
            return agent;
        }

        public BossLogic GetBossBehavior()
        {
            return logic;
        }

        public ushort GetInstid()
        {
            return instid;
        }

        public long GetFirstAware()
        {
            return first_aware;
        }

        public long GetLastAware()
        {
            return last_aware;
        }

        public long GetAwareDuration()
        {
            return last_aware - first_aware;
        }

        public ushort GetID()
        {
            return id;
        }

        public String GetName()
        {
          
            return name;
        }

        public int GetHealth()
        {
            return health;
        }
        public int GetTough()
        {
            return toughness;
        }

        public List<Point> GetHealthOverTime() {
            return healthOverTime;
        }
        // Setters
        public void SetAgent(ulong agent)
        {
            this.agent = agent;
        }

        public void SetInstid(ushort instid)
        {
            this.instid = instid;
        }

        public void SetFirstAware(long first_aware)
        {
            this.first_aware = first_aware;
        }

        public void SetLastAware(long last_aware)
        {
            this.last_aware = last_aware;
        }

        public void SetName(String name)
        {
            name = name.Replace("\0", "");
            this.name = name;
        }

        public void SetHealth(int health)
        {
            this.health = health;
        }
        public void SetTough(int tough)
        {
            toughness = tough;
        }
        public void SetHealthOverTime(List<Point> hot) {
            healthOverTime = hot;
        }
        public bool IsCM()
        {
            return isCM == 1;
        }
        public void SetCM(List<CombatItem> clist)
        {
            if (isCM == -1)
            {
                isCM = logic.IsCM(clist, health);
                if (isCM == 1)
                {
                    name += " CM";
                }
            }
        }
    }
}