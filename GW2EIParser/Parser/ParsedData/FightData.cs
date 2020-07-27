using System;
using System.Collections.Generic;
using System.IO;
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
            switch (ParseEnum.GetTargetID(id))
            {
                //
                case ParseEnum.TargetID.ValeGuardian:
                    Logic = new ValeGuardian(id);
                    break;
                case ParseEnum.TargetID.Gorseval:
                    Logic = new Gorseval(id);
                    break;
                case ParseEnum.TargetID.Sabetha:
                    Logic = new Sabetha(id);
                    break;
                case ParseEnum.TargetID.Slothasor:
                    Logic = new Slothasor(id);
                    break;
                case ParseEnum.TargetID.Zane:
                case ParseEnum.TargetID.Berg:
                case ParseEnum.TargetID.Narella:
                    Logic = new BanditTrio(id);
                    break;
                case ParseEnum.TargetID.Matthias:
                    Logic = new Matthias(id);
                    break;
                /*case ParseEnum.TargetIDS.Escort:
                    Logic = new Escort(id, agentData);
                    break;*/
                case ParseEnum.TargetID.KeepConstruct:
                    Logic = new KeepConstruct(id);
                    break;
                case ParseEnum.TargetID.Xera:
                    // some TC logs are registered as Xera
                    if (agentData.GetNPCsByID((int)ParseEnum.TrashID.HauntingStatue).Count > 0)
                    {
                        TriggerID = (int)ParseEnum.TrashID.HauntingStatue;
                        Logic = new TwistedCastle((int)ParseEnum.TargetID.TwistedCastle);
                        break;
                    }
                    Logic = new Xera(id);
                    break;
                case ParseEnum.TargetID.Cairn:
                    Logic = new Cairn(id);
                    break;
                case ParseEnum.TargetID.MursaatOverseer:
                    Logic = new MursaatOverseer(id);
                    break;
                case ParseEnum.TargetID.Samarog:
                    Logic = new Samarog(id);
                    break;
                case ParseEnum.TargetID.Deimos:
                    Logic = new Deimos(id);
                    break;
                case ParseEnum.TargetID.SoullessHorror:
                    Logic = new SoullessHorror(id);
                    break;
                case ParseEnum.TargetID.Desmina:
                    Logic = new River(id);
                    break;
                case ParseEnum.TargetID.BrokenKing:
                    Logic = new BrokenKing(id);
                    break;
                case ParseEnum.TargetID.SoulEater:
                    Logic = new EaterOfSouls(id);
                    break;
                case ParseEnum.TargetID.EyeOfFate:
                case ParseEnum.TargetID.EyeOfJudgement:
                    Logic = new DarkMaze(id);
                    break;
                case ParseEnum.TargetID.Dhuum:
                    // some eyes logs are registered as Dhuum
                    if (agentData.GetNPCsByID((int)ParseEnum.TargetID.EyeOfFate).Count > 0 ||
                        agentData.GetNPCsByID((int)ParseEnum.TargetID.EyeOfJudgement).Count > 0)
                    {
                        TriggerID = (int)ParseEnum.TargetID.EyeOfFate;
                        Logic = new DarkMaze(TriggerID);
                        break;
                    }
                    Logic = new Dhuum(id);
                    break;
                case ParseEnum.TargetID.ConjuredAmalgamate:
                    Logic = new ConjuredAmalgamate(id);
                    break;
                case ParseEnum.TargetID.Kenut:
                case ParseEnum.TargetID.Nikare:
                    Logic = new TwinLargos(id);
                    break;
                case ParseEnum.TargetID.Qadim:
                    Logic = new Qadim(id);
                    break;
                case ParseEnum.TargetID.Freezie:
                    Logic = new Freezie(id);
                    break;
                case ParseEnum.TargetID.Adina:
                    Logic = new Adina(id);
                    break;
                case ParseEnum.TargetID.Sabir:
                    Logic = new Sabir(id);
                    break;
                case ParseEnum.TargetID.PeerlessQadim:
                    Logic = new PeerlessQadim(id);
                    break;
                    //
                case ParseEnum.TargetID.IcebroodConstruct:
                    Logic = new IcebroodConstruct(id);
                    break;
                case ParseEnum.TargetID.FraenirOfJormag:
                    Logic = new FraenirOfJormag(id);
                    break;
                case ParseEnum.TargetID.VoiceOfTheFallen:
                case ParseEnum.TargetID.ClawOfTheFallen:
                    Logic = new SuperKodanBrothers(id);
                    break;
                case ParseEnum.TargetID.Boneskinner:
                    Logic = new Boneskinner(id);
                    break;
                case ParseEnum.TargetID.WhisperOfJormag:
                    Logic = new WhisperOfJormag(id);
                    break;
                case ParseEnum.TargetID.VariniaStormsounder:
                    Logic = new ColdWar(id);
                    break;
                //
                case ParseEnum.TargetID.MAMA:
                    Logic = new MAMA(id);
                    break;
                case ParseEnum.TargetID.Siax:
                    Logic = new Siax(id);
                    break;
                case ParseEnum.TargetID.Ensolyss:
                    Logic = new Ensolyss(id);
                    break;
                case ParseEnum.TargetID.Skorvald:
                    Logic = new Skorvald(id);
                    break;
                case ParseEnum.TargetID.Artsariiv:
                    Logic = new Artsariiv(id);
                    break;
                case ParseEnum.TargetID.Arkk:
                    Logic = new Arkk(id);
                    break;
                    //
                case ParseEnum.TargetID.WorldVersusWorld:
                    Logic = new WvWFight(id);
                    break;
                    //
                case ParseEnum.TargetID.MassiveGolem:
                case ParseEnum.TargetID.AvgGolem:
                case ParseEnum.TargetID.LGolem:
                case ParseEnum.TargetID.MedGolem:
                case ParseEnum.TargetID.StdGolem:
                    Logic = new Golem(id);
                    break;
                    //
                default:
                    switch (ParseEnum.GetTrashID(id))
                    {
                        case ParseEnum.TrashID.HauntingStatue:
                            Logic = new TwistedCastle((int)ParseEnum.TargetID.TwistedCastle);
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
                _phases = Logic.GetPhases(log, log.ParserSettings.ParsePhases);
                _phases.AddRange(Logic.GetBreakbarPhases(log, log.ParserSettings.ParsePhases));
                _phases.RemoveAll(x => x.Targets.Count == 0);
                if (_phases.Exists(x => x.BreakbarPhase && x.Targets.Count != 1))
                {
                    throw new InvalidDataException("Breakbar phases can only have one target");
                }
                _phases.RemoveAll(x => x.DurationInMS < GeneralHelper.PhaseTimeLimit);
                _phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            }
            return _phases;
        }

        public List<NPC> GetMainTargets(ParsedLog log)
        {
            return GetPhases(log)[0].Targets;
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
