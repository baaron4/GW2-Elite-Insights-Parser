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
        public long FightStartLogTime { get; private set; }
        public long FightEndLogTime { get; private set; } = long.MaxValue;
        public long FightDuration => FightEndLogTime - FightStartLogTime;
        public string DurationString {
            get
            {
                var duration = TimeSpan.FromSeconds(FightDuration);
                string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s " + duration.Milliseconds + "ms";
                if (duration.ToString("hh") != "00")
                {
                    durationString = duration.ToString("hh") + "h " + durationString;
                }
                return durationString;
            }
        }
        public bool Success { get; private set; }
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
        public FightData(ushort id, AgentData agentData, long start, long end)
        {
            FightStartLogTime = start;
            FightEndLogTime = end;
            ID = id;
            _requirePhases = Properties.Settings.Default.ParsePhases;
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
                case ParseEnum.TargetIDS.Zane:
                case ParseEnum.TargetIDS.Berg:
                case ParseEnum.TargetIDS.Narella:
                    Logic = new BanditTrio(id);
                    break;
                case ParseEnum.TargetIDS.Matthias:
                    Logic = new Matthias(id);
                    break;
                /*case ParseEnum.TargetIDS.Escort:
                    Logic = new Escort(id, agentData);
                    break;*/
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
                case ParseEnum.TargetIDS.Desmina:
                    Logic = new River(id);
                    break;
                case ParseEnum.TargetIDS.BrokenKing:
                    Logic = new BrokenKing(id);
                    break;
                case ParseEnum.TargetIDS.SoulEater:
                    Logic = new EaterOfSouls(id);
                    break;
                case ParseEnum.TargetIDS.EyeOfFate:
                case ParseEnum.TargetIDS.EyeOfJudgement:
                    Logic = new DarkMaze(id);
                    break;
                case ParseEnum.TargetIDS.Dhuum:
                    // some eyes log are registered as Dhuum
                    if (agentData.GetAgentsByID((ushort)ParseEnum.TargetIDS.EyeOfFate).Count > 0 ||
                        agentData.GetAgentsByID((ushort)ParseEnum.TargetIDS.EyeOfJudgement).Count > 0)
                    {
                        ID = (ushort)ParseEnum.TargetIDS.EyeOfFate;
                        Logic = new DarkMaze((ushort)ParseEnum.TargetIDS.EyeOfFate);
                        break;
                    }
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
                case ParseEnum.TargetIDS.Freezie:
                    Logic = new Freezie(id);
                    break;
                case ParseEnum.TargetIDS.Adina:
                    Logic = new Adina(id);
                    break;
                case ParseEnum.TargetIDS.Sabir:
                    Logic = new Sabir(id);
                    break;
                case ParseEnum.TargetIDS.PeerlessQadim:
                    Logic = new PeerlessQadim(id);
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
                case ParseEnum.TargetIDS.WorldVersusWorld:
                    Logic = new WvWFight(id);
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
                    Logic = new UnknownFightLogic(id);
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
            return time - FightStartLogTime;
        }

        public long ToLogSpace(long time)
        {
            return time + FightStartLogTime;
        }

        // Setters
        public void SetCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (_isCM == -1)
            {
                _isCM = Logic.IsCM(combatData, agentData, fightData);
            }
        }

        public void SetSuccess(bool success, long fightEndLogTime)
        {
            Success = success;
            FightEndLogTime = fightEndLogTime;
        }

        public void OverrideStart(long fightStartLogTime)
        {
            FightStartLogTime = fightStartLogTime;
        }
    }
}