using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LuckParser.Models.ParseModels
{
    public class FightData
    {
        // Fields
        private List<PhaseData> _phases = new List<PhaseData>();
        public readonly List<long> PhaseData = new List<long>();
        public ushort ID { get; }
        private readonly bool _requirePhases;
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
        public string Name { get; set; } = "UNKNOWN";
        private int _isCM = -1;
        public bool IsCM
        {
            get
            {
                return _isCM == 1;
            }
        }
        // Constructors
        public FightData(ushort id, bool requirePhases)
        {
            ID = id;
            _requirePhases = requirePhases;
            switch (ParseEnum.GetBossIDS(id))
            {
                case ParseEnum.BossIDS.ValeGuardian:
                    Logic = new ValeGuardian(id);
                    break;
                case ParseEnum.BossIDS.Gorseval:
                    Logic = new Gorseval(id);
                    break;
                case ParseEnum.BossIDS.Sabetha:
                    Logic = new Sabetha(id);
                    break;
                case ParseEnum.BossIDS.Slothasor:
                    Logic = new Slothasor(id);
                    break;
                case ParseEnum.BossIDS.Matthias:
                    Logic = new Matthias(id);
                    break;
                case ParseEnum.BossIDS.KeepConstruct:
                    Logic = new KeepConstruct(id);
                    break;
                case ParseEnum.BossIDS.Xera:
                    Logic = new Xera(id);
                    break;
                case ParseEnum.BossIDS.Cairn:
                    Logic = new Cairn(id);
                    break;
                case ParseEnum.BossIDS.MursaatOverseer:
                    Logic = new MursaatOverseer(id);
                    break;
                case ParseEnum.BossIDS.Samarog:
                    Logic = new Samarog(id);
                    break;
                case ParseEnum.BossIDS.Deimos:
                    Logic = new Deimos(id);
                    break;
                case ParseEnum.BossIDS.SoullessHorror:
                    Logic = new SoullessHorror(id);
                    break;
                case ParseEnum.BossIDS.Dhuum:
                    Logic = new Dhuum(id);
                    break;
                case ParseEnum.BossIDS.ConjuredAmalgamate:
                    Logic = new ConjuredAmalgamate(id);
                    break;
                case ParseEnum.BossIDS.Kenut:
                case ParseEnum.BossIDS.Nikare:
                    Logic = new TwinLargos(id);
                    break;
                case ParseEnum.BossIDS.Qadim:
                    Logic = new Qadim(id);
                    break;
                case ParseEnum.BossIDS.MAMA:
                    Logic = new MAMA(id);
                    break;
                case ParseEnum.BossIDS.Siax:
                    Logic = new Siax(id);
                    break;
                case ParseEnum.BossIDS.Ensolyss:
                    Logic = new Ensolyss(id);
                    break;
                case ParseEnum.BossIDS.Skorvald:
                    Logic = new Skorvald(id);
                    break;
                case ParseEnum.BossIDS.Artsariiv:
                    Logic = new Artsariiv(id);
                    break;
                case ParseEnum.BossIDS.Arkk:
                    Logic = new Arkk(id);
                    break;
                case ParseEnum.BossIDS.MassiveGolem:
                case ParseEnum.BossIDS.AvgGolem:
                case ParseEnum.BossIDS.LGolem:
                case ParseEnum.BossIDS.MedGolem:
                case ParseEnum.BossIDS.StdGolem:
                    Logic = new Golem(id);
                    break;
                default:
                    // Unknown
                    Logic = new BossLogic(id);
                    break;
            }
        }

        public List<PhaseData> GetPhases(ParsedLog log)
        {

            if (_phases.Count == 0)
            {
                long fightDuration = log.FightData.FightDuration;
                _phases = log.FightData.Logic.GetPhases(log, _requirePhases);
            }
            return _phases;
        }
        // Setters
        public void SetCM(ParsedLog log)
        {
            if (_isCM == -1)
            {
                _isCM = Logic.IsCM(log);
                if (_isCM == 1)
                {
                    Name += " CM";
                }
            }
        }
        public void SetSuccess(ParsedLog log)
        {
            Logic.SetSuccess(log);
        }
    }
}