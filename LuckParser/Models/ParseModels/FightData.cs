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
        public readonly FightLogic Logic;
        public long FightStart { get; set; }
        public long FightEnd { get; set; } = long.MaxValue;
        public long FightDuration => FightEnd - FightStart;
        public bool Success { get; set; }
        public string Name => Logic.GetFightName() + (_isCM == 1 ? " CM" : "") ;
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
            switch (ParseEnum.GetTargetIDS(id))
            {
                case ParseEnum.TargetIDS.ValeGuardian:
                    Logic = new ValeGuardian(id);
                    break;
                case ParseEnum.TargetIDS.Gorseval:
                    Logic = new Gorseval(id);
                    break;
                case ParseEnum.TargetIDS.Sabetha:
                    Logic = new Sabetha(id);
                    break;
                case ParseEnum.TargetIDS.Slothasor:
                    Logic = new Slothasor(id);
                    break;
                case ParseEnum.TargetIDS.Matthias:
                    Logic = new Matthias(id);
                    break;
                case ParseEnum.TargetIDS.KeepConstruct:
                    Logic = new KeepConstruct(id);
                    break;
                case ParseEnum.TargetIDS.Xera:
                    Logic = new Xera(id);
                    break;
                case ParseEnum.TargetIDS.Cairn:
                    Logic = new Cairn(id);
                    break;
                case ParseEnum.TargetIDS.MursaatOverseer:
                    Logic = new MursaatOverseer(id);
                    break;
                case ParseEnum.TargetIDS.Samarog:
                    Logic = new Samarog(id);
                    break;
                case ParseEnum.TargetIDS.Deimos:
                    Logic = new Deimos(id);
                    break;
                case ParseEnum.TargetIDS.SoullessHorror:
                    Logic = new SoullessHorror(id);
                    break;
                case ParseEnum.TargetIDS.Dhuum:
                    Logic = new Dhuum(id);
                    break;
                case ParseEnum.TargetIDS.ConjuredAmalgamate:
                    Logic = new ConjuredAmalgamate(id);
                    break;
                case ParseEnum.TargetIDS.Kenut:
                case ParseEnum.TargetIDS.Nikare:
                    Logic = new TwinLargos(id);
                    break;
                case ParseEnum.TargetIDS.Qadim:
                    Logic = new Qadim(id);
                    break;
                case ParseEnum.TargetIDS.MAMA:
                    Logic = new MAMA(id);
                    break;
                case ParseEnum.TargetIDS.Siax:
                    Logic = new Siax(id);
                    break;
                case ParseEnum.TargetIDS.Ensolyss:
                    Logic = new Ensolyss(id);
                    break;
                case ParseEnum.TargetIDS.Skorvald:
                    Logic = new Skorvald(id);
                    break;
                case ParseEnum.TargetIDS.Artsariiv:
                    Logic = new Artsariiv(id);
                    break;
                case ParseEnum.TargetIDS.Arkk:
                    Logic = new Arkk(id);
                    break;
                case ParseEnum.TargetIDS.MassiveGolem:
                case ParseEnum.TargetIDS.AvgGolem:
                case ParseEnum.TargetIDS.LGolem:
                case ParseEnum.TargetIDS.MedGolem:
                case ParseEnum.TargetIDS.StdGolem:
                    Logic = new Golem(id);
                    break;
                default:
                    // Unknown
                    Logic = new FightLogic(id);
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

        public List<Target> GetMainTargets(ParsedLog log)
        {
            if (_phases.Count == 0)
            {
                long fightDuration = log.FightData.FightDuration;
                _phases = log.FightData.Logic.GetPhases(log, _requirePhases);
            }
            return _phases[0].Targets;
        }

        // Setters
        public void SetCM(ParsedLog log)
        {
            if (_isCM == -1)
            {
                _isCM = Logic.IsCM(log);
            }
        }
        public void SetSuccess(ParsedLog log)
        {
            Logic.SetSuccess(log);
        }
    }
}