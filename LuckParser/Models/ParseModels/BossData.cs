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
        private ulong _agent = 0;
        private ushort _instid = 0;
        private BossLogic _logic;
        private long _firstAware = 0;
        private long _lastAware = long.MaxValue;
        private ushort _id;
        private string _name = "UNKNOWN";
        private int _health = -1;
        private int _toughness = -1;
        private List<Point> _healthOverTime = new List<Point>();
        private int _isCM = -1;
        // Constructors
        public BossData(ushort id)
        {
            _id = id;
            switch (ParseEnum.GetBossIDS(id))
            {
                case ParseEnum.BossIDS.ValeGuardian:
                    _logic = new ValeGuardian();
                    break;
                case ParseEnum.BossIDS.Gorseval:
                    _logic = new Gorseval();
                    break;
                case ParseEnum.BossIDS.Sabetha:
                    _logic = new Sabetha();
                    break;
                case ParseEnum.BossIDS.Slothasor:
                    _logic = new Slothasor();
                    break;
                case ParseEnum.BossIDS.Matthias:
                    _logic = new Matthias();
                    break;
                case ParseEnum.BossIDS.KeepConstruct:
                    _logic = new KeepConstruct();
                    break;
                case ParseEnum.BossIDS.Xera:
                    _logic = new Xera();
                    break;
                case ParseEnum.BossIDS.Cairn:
                    _logic = new Cairn();
                    break;
                case ParseEnum.BossIDS.MursaatOverseer:
                    _logic = new MursaatOverseer();
                    break;
                case ParseEnum.BossIDS.Samarog:
                    _logic = new Samarog();
                    break;
                case ParseEnum.BossIDS.Deimos:
                    _logic = new Deimos();
                    break;
                case ParseEnum.BossIDS.SoullessHorror:
                    _logic = new SoullessHorror();
                    break;
                case ParseEnum.BossIDS.Dhuum:
                    _logic = new Dhuum();
                    break;
                case ParseEnum.BossIDS.MAMA:
                    _logic = new MAMA();
                    break;
                case ParseEnum.BossIDS.Siax:
                    _logic = new Siax();
                    break;
                case ParseEnum.BossIDS.Ensolyss:
                    _logic = new Ensolyss();
                    break;
                case ParseEnum.BossIDS.Skorvald:
                    _logic = new Skorvald();
                    break;
                case ParseEnum.BossIDS.Artsariiv:
                    _logic = new Artasariiv();
                    break;
                case ParseEnum.BossIDS.Arkk:
                    _logic = new Arkk();
                    break;
                case ParseEnum.BossIDS.Golem1:
                case ParseEnum.BossIDS.Golem2:
                case ParseEnum.BossIDS.Golem3:
                case ParseEnum.BossIDS.Golem4:
                case ParseEnum.BossIDS.Golem5:
                    _logic = new Golem();
                    break;
                default:
                    // Unknown
                    _logic = new BossLogic();
                    break;
            }
        }

        public String[] ToStringArray()
        {
            String[] array = new String[7];
            array[0] = string.Format("{0:X}", _agent); ;
            array[1] =_instid.ToString();
            array[2] = _firstAware.ToString();
            array[3] = _lastAware.ToString();
            array[4] =_id.ToString();
            array[5] = _name;
            array[6] = _health.ToString();
            return array;
        }

        // Getters
        public ulong GetAgent()
        {
            return _agent;
        }

        public BossLogic GetBossBehavior()
        {
            return _logic;
        }

        public ushort GetInstid()
        {
            return _instid;
        }

        public long GetFirstAware()
        {
            return _firstAware;
        }

        public long GetLastAware()
        {
            return _lastAware;
        }

        public long GetAwareDuration()
        {
            return _lastAware - _firstAware;
        }

        public ushort GetID()
        {
            return _id;
        }

        public String GetName()
        {
          
            return _name;
        }

        public int GetHealth()
        {
            return _health;
        }
        public int GetTough()
        {
            return _toughness;
        }

        public List<Point> GetHealthOverTime() {
            return _healthOverTime;
        }
        // Setters
        public void SetAgent(ulong agent)
        {
            _agent = agent;
        }

        public void SetInstid(ushort instid)
        {
            _instid = instid;
        }

        public void SetFirstAware(long firstAware)
        {
            _firstAware = firstAware;
        }

        public void SetLastAware(long lastAware)
        {
            _lastAware = lastAware;
        }

        public void SetName(String name)
        {
            name = name.Replace("\0", "");
            _name = name;
        }

        public void SetHealth(int health)
        {
            _health = health;
        }
        public void SetTough(int tough)
        {
            _toughness = tough;
        }
        public void SetHealthOverTime(List<Point> hot) {
            _healthOverTime = hot;
        }
        public bool IsCM()
        {
            return _isCM == 1;
        }
        public void SetCM(List<CombatItem> clist)
        {
            if (_isCM == -1)
            {
                _isCM = _logic.IsCM(clist, _health);
                if (_isCM == 1)
                {
                    _name += " CM";
                }
            }
        }
    }
}