using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LuckParser.Models.ParseModels
{
    public class FightData
    {
        // Fields
        public ulong Agent { get; set; }
        public ushort InstID { get; set; }
        public readonly BossLogic Logic;
        public long FightStart { get; set; }
        public long FightEnd { get; set; } = long.MaxValue;
        public long FightDuration
        {
            get
            {
                return FightEnd - FightStart;
            }
        }
        public readonly ushort ID;
        public string Name { get; set; } = "UNKNOWN";
        public int Health { get; set; } = -1;
        public List<Point> HealthOverTime { get; set; } = new List<Point>();
        private int _isCM = -1;
        public bool IsCM
        {
            get
            {
                return _isCM == 1;
            }
        }
        // Constructors
        public FightData(ushort id)
        {
            ID = id;
            switch (ParseEnum.GetBossIDS(id))
            {
                case ParseEnum.BossIDS.ValeGuardian:
                    Logic = new ValeGuardian();
                    break;
                case ParseEnum.BossIDS.Gorseval:
                    Logic = new Gorseval();
                    break;
                case ParseEnum.BossIDS.Sabetha:
                    Logic = new Sabetha();
                    break;
                case ParseEnum.BossIDS.Slothasor:
                    Logic = new Slothasor();
                    break;
                case ParseEnum.BossIDS.Matthias:
                    Logic = new Matthias();
                    break;
                case ParseEnum.BossIDS.KeepConstruct:
                    Logic = new KeepConstruct();
                    break;
                case ParseEnum.BossIDS.Xera:
                    Logic = new Xera();
                    break;
                case ParseEnum.BossIDS.Cairn:
                    Logic = new Cairn();
                    break;
                case ParseEnum.BossIDS.MursaatOverseer:
                    Logic = new MursaatOverseer();
                    break;
                case ParseEnum.BossIDS.Samarog:
                    Logic = new Samarog();
                    break;
                case ParseEnum.BossIDS.Deimos:
                    Logic = new Deimos();
                    break;
                case ParseEnum.BossIDS.SoullessHorror:
                    Logic = new SoullessHorror();
                    break;
                case ParseEnum.BossIDS.Dhuum:
                    Logic = new Dhuum();
                    break;
                case ParseEnum.BossIDS.MAMA:
                    Logic = new MAMA();
                    break;
                case ParseEnum.BossIDS.Siax:
                    Logic = new Siax();
                    break;
                case ParseEnum.BossIDS.Ensolyss:
                    Logic = new Ensolyss();
                    break;
                case ParseEnum.BossIDS.Skorvald:
                    Logic = new Skorvald();
                    break;
                case ParseEnum.BossIDS.Artsariiv:
                    Logic = new Artsariiv();
                    break;
                case ParseEnum.BossIDS.Arkk:
                    Logic = new Arkk();
                    break;
                case ParseEnum.BossIDS.Golem1:
                case ParseEnum.BossIDS.Golem2:
                case ParseEnum.BossIDS.Golem3:
                case ParseEnum.BossIDS.Golem4:
                case ParseEnum.BossIDS.Golem5:
                    Logic = new Golem();
                    break;
                default:
                    // Unknown
                    Logic = new BossLogic();
                    break;
            }
        }

        public String[] ToStringArray()
        {
            String[] array = new String[7];
            array[0] = Agent.ToString();
            array[1] = InstID.ToString();
            array[2] = FightStart.ToString();
            array[3] = FightEnd.ToString();
            array[4] = ID.ToString();
            array[5] = Name;
            array[6] = Health.ToString();
            return array;
        }
        // Setters
        public void SetCM(List<CombatItem> clist)
        {
            if (_isCM == -1)
            {
                _isCM = Logic.IsCM(clist, Health);
                if (_isCM == 1)
                {
                    Name += " CM";
                }
            }
        }
        public void SetSuccess(CombatData combatData, LogData logData)
        {
            Logic.SetSuccess(combatData, logData, this);
        }
    }
}