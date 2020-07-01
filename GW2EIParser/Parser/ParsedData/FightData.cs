using System;
using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Logic;

namespace GW2EIParser.Parser.ParsedData
{
    public class FightData
    {
        // Fields
        private List<PhaseData> _phases = new List<PhaseData>();
        public int TriggerID { get; }
        public FightLogic Logic { get; }
        public long FightOffset { get; private set; }
        public long FightStart { get; } = 0;
        public long FightEnd { get; private set; } = long.MaxValue;
        public string DurationString
        {
            get
            {
                var duration = TimeSpan.FromMilliseconds(FightEnd);
                string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s " + duration.Milliseconds + "ms";
                if (duration.Hours > 0)
                {
                    durationString = duration.ToString("hh") + "h " + durationString;
                }
                return durationString;
            }
        }
        public bool Success { get; private set; }

        public enum CMStatus { NotSet, CM, NoCM, CMnoName }

        private CMStatus _isCM = CMStatus.NotSet;
        public bool IsCM => _isCM == CMStatus.CMnoName || _isCM == CMStatus.CM;
        // Constructors
        public FightData(int id, AgentData agentData, long start, long end)
        {
            FightOffset = start;
            FightEnd = end - start;
            TriggerID = id;
            switch (ParseEnum.GetTargetIDS(id))
            {
                //
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
                    // some TC logs are registered as Xera
                    if (agentData.GetNPCsByID((int)ParseEnum.TrashIDS.HauntingStatue).Count > 0)
                    {
                        TriggerID = (int)ParseEnum.TrashIDS.HauntingStatue;
                        Logic = new TwistedCastle((int)ParseEnum.TargetIDS.TwistedCastle);
                        break;
                    }
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
                    // some eyes logs are registered as Dhuum
                    if (agentData.GetNPCsByID((int)ParseEnum.TargetIDS.EyeOfFate).Count > 0 ||
                        agentData.GetNPCsByID((int)ParseEnum.TargetIDS.EyeOfJudgement).Count > 0)
                    {
                        TriggerID = (int)ParseEnum.TargetIDS.EyeOfFate;
                        Logic = new DarkMaze(TriggerID);
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
                    //
                case ParseEnum.TargetIDS.IcebroodConstruct:
                    Logic = new IcebroodConstruct(id);
                    break;
                case ParseEnum.TargetIDS.FraenirOfJormag:
                    Logic = new FraenirOfJormag(id);
                    break;
                case ParseEnum.TargetIDS.VoiceOfTheFallen:
                case ParseEnum.TargetIDS.ClawOfTheFallen:
                    Logic = new SuperKodanBrothers(id);
                    break;
                case ParseEnum.TargetIDS.Boneskinner:
                    Logic = new Boneskinner(id);
                    break;
                case ParseEnum.TargetIDS.WhisperOfJormag:
                    Logic = new WhisperOfJormag(id);
                    break;
                case ParseEnum.TargetIDS.VariniaStormsounder:
                    Logic = new ColdWar(id);
                    break;
                //
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
                    //
                case ParseEnum.TargetIDS.WorldVersusWorld:
                    Logic = new WvWFight(id);
                    break;
                    //
                case ParseEnum.TargetIDS.MassiveGolem:
                case ParseEnum.TargetIDS.AvgGolem:
                case ParseEnum.TargetIDS.LGolem:
                case ParseEnum.TargetIDS.MedGolem:
                case ParseEnum.TargetIDS.StdGolem:
                    Logic = new Golem(id);
                    break;
                    //
                default:
                    switch (ParseEnum.GetTrashIDS(id))
                    {
                        case ParseEnum.TrashIDS.HauntingStatue:
                            Logic = new TwistedCastle((int)ParseEnum.TargetIDS.TwistedCastle);
                            break;
                        default:
                            // Unknown
                            Logic = new UnknownFightLogic(id);
                            break;
                    }
                    break;
            }
        }

        public string GetFightName(ParsedLog log)
        {
            return Logic.GetLogicName(log) + (_isCM == CMStatus.CM ? " CM" : "");
        }
        public List<PhaseData> GetPhases(ParsedLog log)
        {

            if (_phases.Count == 0)
            {
                _phases = log.FightData.Logic.GetPhases(log, log.ParserSettings.ParsePhases);
                _phases.RemoveAll(x => x.Targets.Count == 0);
                _phases.RemoveAll(x => x.DurationInMS < GeneralHelper.PhaseTimeLimit);
            }
            return _phases;
        }

        public List<NPC> GetMainTargets(ParsedLog log)
        {
            if (_phases.Count == 0)
            {
                _phases = log.FightData.Logic.GetPhases(log, log.ParserSettings.ParsePhases);
            }
            return _phases[0].Targets;
        }

        // Setters
        public void SetCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (_isCM == CMStatus.NotSet)
            {
                _isCM = Logic.IsCM(combatData, agentData, fightData);
            }
        }

        public void SetSuccess(bool success, long fightEnd)
        {
            Success = success;
            FightEnd = fightEnd;
        }

        public void OverrideOffset(long offset)
        {
            FightEnd += FightOffset - offset;
            FightOffset = offset;
        }
    }
}
