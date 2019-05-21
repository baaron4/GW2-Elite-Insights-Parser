using LuckParser.Parser;
using LuckParser.Models.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LuckParser.Models.ParseModels
{
    public class FightData
    {
        // Fields
        private List<PhaseData> _phases = new List<PhaseData>();
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
        public FightData(ushort id, AgentData agentData)
        {
            ID = id;
            _requirePhases = Properties.Settings.Default.ParsePhases;
            switch (ParseEnum.GetTargetIDS(id))
            {
                case ParseEnum.TargetIDS.ValeGuardian:
                    Logic = new ValeGuardian(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Gorseval:
                    Logic = new Gorseval(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Sabetha:
                    Logic = new Sabetha(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Slothasor:
                    Logic = new Slothasor(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Zane:
                case ParseEnum.TargetIDS.Berg:
                case ParseEnum.TargetIDS.Narella:
                    Logic = new BanditTrio(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Matthias:
                    Logic = new Matthias(id, agentData);
                    break;
                /*case ParseEnum.TargetIDS.Escort:
                    Logic = new Escort(id, agentData);
                    break;*/
                case ParseEnum.TargetIDS.KeepConstruct:
                    Logic = new KeepConstruct(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Xera:
                    Logic = new Xera(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Cairn:
                    Logic = new Cairn(id, agentData);
                    break;
                case ParseEnum.TargetIDS.MursaatOverseer:
                    Logic = new MursaatOverseer(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Samarog:
                    Logic = new Samarog(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Deimos:
                    Logic = new Deimos(id, agentData);
                    break;
                case ParseEnum.TargetIDS.SoullessHorror:
                    Logic = new SoullessHorror(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Desmina:
                    Logic = new River(id, agentData);
                    break;
                case ParseEnum.TargetIDS.BrokenKing:
                    Logic = new BrokenKing(id, agentData);
                    break;
                case ParseEnum.TargetIDS.SoulEater:
                    Logic = new EaterOfSouls(id, agentData);
                    break;
                case ParseEnum.TargetIDS.EyeOfFate:
                case ParseEnum.TargetIDS.EyeOfJudgement:
                    Logic = new DarkMaze(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Dhuum:
                    // some eyes log are registered as Dhuum
                    if (agentData.GetAgentsByID((ushort)ParseEnum.TargetIDS.EyeOfFate).Count > 0 ||
                        agentData.GetAgentsByID((ushort)ParseEnum.TargetIDS.EyeOfJudgement).Count > 0)
                    {
                        ID = (ushort)ParseEnum.TargetIDS.EyeOfFate;
                        Logic = new DarkMaze((ushort)ParseEnum.TargetIDS.EyeOfFate, agentData);
                        break;
                    }
                    Logic = new Dhuum(id, agentData);
                    break;
                case ParseEnum.TargetIDS.ConjuredAmalgamate:
                    Logic = new ConjuredAmalgamate(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Kenut:
                case ParseEnum.TargetIDS.Nikare:
                    Logic = new TwinLargos(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Qadim:
                    Logic = new Qadim(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Freezie:
                    Logic = new Freezie(id, agentData);
                    break;
                case ParseEnum.TargetIDS.MAMA:
                    Logic = new MAMA(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Siax:
                    Logic = new Siax(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Ensolyss:
                    Logic = new Ensolyss(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Skorvald:
                    Logic = new Skorvald(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Artsariiv:
                    Logic = new Artsariiv(id, agentData);
                    break;
                case ParseEnum.TargetIDS.Arkk:
                    Logic = new Arkk(id, agentData);
                    break;
                case ParseEnum.TargetIDS.WorldVersusWorld:
                    Logic = new WvWFight(id, agentData);
                    break;
                case ParseEnum.TargetIDS.MassiveGolem:
                case ParseEnum.TargetIDS.AvgGolem:
                case ParseEnum.TargetIDS.LGolem:
                case ParseEnum.TargetIDS.MedGolem:
                case ParseEnum.TargetIDS.StdGolem:
                    Logic = new Golem(id, agentData);
                    break;
                default:
                    // Unknown
                    Logic = new UnknownFightLogic(id, agentData);
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
            _phases.RemoveAll(x => x.DurationInMS <= 1000);
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

        public long ToFightSpace(long time)
        {
            return time - FightStart;
        }

        public long ToLogSpace(long time)
        {
            return time + FightStart;
        }

        // Setters
        public void SetCM(ParsedEvtcContainer evtcContainer)
        {
            if (_isCM == -1)
            {
                _isCM = Logic.IsCM(evtcContainer);
            }
        }
        public void SetSuccess(ParsedEvtcContainer evtcContainer)
        {
            Logic.SetSuccess(evtcContainer);
        }
    }
}