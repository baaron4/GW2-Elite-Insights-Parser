using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.EncounterLogic.OpenWorld;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParsedData.AgentItem;

namespace GW2EIEvtcParser.ParsedData;

public class FightData
{
    // Fields
    private List<PhaseData> _phases = [];
    public readonly int TriggerID;
    public readonly FightLogic Logic;
    public long FightEnd { get; private set; } = long.MaxValue;
    public readonly long FightStart = 0;
    public long FightDuration => FightEnd;

    public string FightName { get; private set; }
    public long LogStart { get; private set; }
    public long LogEnd { get; private set; }
    public long LogOffset { get; private set; }

    public long FightStartOffset => -LogStart;
    public string DurationString
    {
        get
        {
            return ParserHelper.ToDurationString(FightDuration);
        }
    }
    public bool Success { get; private set; }

    internal enum EncounterMode
    {
        NotSet,
        Story,
        Normal,
        LegendaryCM,
        CM,
        CMNoName
    }
    private EncounterMode _encounterMode = EncounterMode.NotSet;
    public bool IsCM => _encounterMode == EncounterMode.CMNoName || _encounterMode == EncounterMode.CM;
    public bool IsLegendaryCM => _encounterMode == EncounterMode.LegendaryCM;

    internal enum EncounterStartStatus
    {
        NotSet,
        Normal,
        Late,
        NoPreEvent
    }
    private EncounterStartStatus _encounterStartStatus = EncounterStartStatus.NotSet;
    public bool IsLateStart => _encounterStartStatus == EncounterStartStatus.Late || MissingPreEvent;
    public bool MissingPreEvent => _encounterStartStatus == EncounterStartStatus.NoPreEvent;

    // Constructors
    internal FightData(int id, AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings, long start, long end, EvtcVersionEvent evtcVersion)
    {
        LogStart = start;
        LogEnd = end;
        FightEnd = end - start;

        Logic = DetectFight(id, agentData, parserSettings, evtcVersion);
        Logic = Logic.AdjustLogic(agentData, combatData);
        TriggerID = Logic.GetTriggerID();
    }

    static internal FightLogic DetectFight(int id, AgentData agentData, EvtcParserSettings parserSettings, EvtcVersionEvent evtcVersion)
    {
        var targetID = GetTargetID(id);
        var target = agentData.GetNPCsByID(id).FirstOrDefault() ?? agentData.GetGadgetsByID(id).FirstOrDefault();
        switch (target?.Type)
        {
            case null:
                switch (targetID)
                {
                    case TargetID.WorldVersusWorld:
                        if (agentData.GetNPCsByID(TargetID.Desmina).Any())
                        {
                            return new River((int)TargetID.DummyTarget);
                        }
                        else
                        {
                            return new WvWFight(id, parserSettings.DetailedWvWParse);
                        }
                    case TargetID.Instance:
                        return new Instance(id);
                }
                break;

            case AgentType.NPC:
                switch (targetID)
                {
                    case TargetID.Mordremoth:
                        return new Mordremoth(id);
                    // Raids
                    case TargetID.ValeGuardian:
                        return new ValeGuardian(id);
                    case TargetID.Gorseval:
                        return new Gorseval(id);
                    case TargetID.Sabetha:
                        return new Sabetha(id);
                    case TargetID.Slothasor:
                        return new Slothasor(id);
                    case TargetID.Zane:
                    case TargetID.Berg:
                    case TargetID.Narella:
                        return new BanditTrio(id);
                    case TargetID.Matthias:
                        return new Matthias(id);
                    case TargetID.McLeodTheSilent:
                        // No proper escort support by arc dps before that build, redirect to unknown
                        if (evtcVersion.Build >= ArcDPSBuilds.NewLogStart)
                        {
                            return new Escort(id);
                        }
                        break;
                    case TargetID.KeepConstruct:
                        return new KeepConstruct(id);
                    case TargetID.Xera:
                        // some TC logs are registered as Xera
                        if (agentData.GetNPCsByID(TrashID.HauntingStatue).Count > 0)
                        {
                            return new TwistedCastle((int)TargetID.DummyTarget);
                        }
                        else
                        {
                            return new Xera(id);
                        }
                    case TargetID.Cairn:
                        return new Cairn(id);
                    case TargetID.MursaatOverseer:
                        return new MursaatOverseer(id);
                    case TargetID.Samarog:
                        return new Samarog(id);
                    case TargetID.Deimos:
                        return new Deimos(id);
                    case TargetID.SoullessHorror:
                        return new SoullessHorror(id);
                    case TargetID.Desmina:
                        return new River((int)TargetID.DummyTarget);
                    case TargetID.BrokenKing:
                        return new StatueOfIce(id);
                    case TargetID.EaterOfSouls:
                        return new StatueOfDeath(id);
                    case TargetID.EyeOfFate:
                    case TargetID.EyeOfJudgement:
                        return new StatueOfDarkness(id);
                    case TargetID.Dhuum:
                        // some eyes logs are registered as Dhuum
                        if (agentData.GetNPCsByID(TargetID.EyeOfFate).Count > 0 ||
                            agentData.GetNPCsByID(TargetID.EyeOfJudgement).Count > 0)
                        {
                            return new StatueOfDarkness((int)TargetID.EyeOfFate);
                        }
                        else
                        {
                            return new Dhuum(id);
                        }
                    case TargetID.Kenut:
                    case TargetID.Nikare:
                        return new TwinLargos(id);
                    case TargetID.Qadim:
                        return new Qadim(id);
                    case TargetID.Freezie:
                        return new Freezie(id);
                    case TargetID.Adina:
                        return new Adina(id);
                    case TargetID.Sabir:
                        return new Sabir(id);
                    case TargetID.PeerlessQadim:
                        return new PeerlessQadim(id);
                    case TargetID.Greer:
                        return new GreerTheBlightbringer(id);
                    case TargetID.Decima:
                        return new DecimaTheStormsinger(id);
                    case TargetID.Ura:
                        return new UraTheSteamshrieker(id);
                    // Strike Missions
                    case TargetID.IcebroodConstruct:
                        return new IcebroodConstruct(id);
                    case TargetID.FraenirOfJormag:
                        return new FraenirOfJormag(id);
                    case TargetID.VoiceOfTheFallen:
                    case TargetID.ClawOfTheFallen:
                        return new SuperKodanBrothers(id);
                    case TargetID.Boneskinner:
                        return new Boneskinner(id);
                    case TargetID.WhisperOfJormag:
                        return new WhisperOfJormag(id);
                    case TargetID.VariniaStormsounder:
                        return new ColdWar(id);
                    case TargetID.MaiTrinStrike:
                        return new AetherbladeHideout(id);
                    case TargetID.MinisterLi:
                    case TargetID.MinisterLiCM:
                        return new KainengOverlook(id);
                    case TargetID.Ankka:
                        return new XunlaiJadeJunkyard(id);
                    case TargetID.PrototypeVermilion:
                    case TargetID.PrototypeArsenite:
                    case TargetID.PrototypeIndigo:
                    case TargetID.PrototypeVermilionCM:
                    case TargetID.PrototypeArseniteCM:
                    case TargetID.PrototypeIndigoCM:
                        return new OldLionsCourt(id);
                    case TargetID.Dagda:
                        return new CosmicObservatory(id);
                    case TargetID.Cerus:
                        return new TempleOfFebe(id);
                    // Fractals
                    case TargetID.MAMA:
                        return new MAMA(id);
                    case TargetID.Siax:
                        return new Siax(id);
                    case TargetID.Ensolyss:
                        return new Ensolyss(id);
                    case TargetID.Skorvald:
                        return new Skorvald(id);
                    case TargetID.Artsariiv:
                        return new Artsariiv(id);
                    case TargetID.Arkk:
                        return new Arkk(id);
                    case TargetID.AiKeeperOfThePeak:
                        return new AiKeeperOfThePeak(id);
                    case TargetID.KanaxaiScytheOfHouseAurkusCM:
                        return new Kanaxai(id);
                    case TargetID.CerusLonelyTower:
                    case TargetID.DeimosLonelyTower:
                        return new CerusAndDeimos(id);
                    case TargetID.EparchLonelyTower:
                        return new Eparch(id);
                    // Golems
                    case TargetID.MassiveGolem10M:
                    case TargetID.MassiveGolem4M:
                    case TargetID.MassiveGolem1M:
                    case TargetID.VitalGolem:
                    case TargetID.AvgGolem:
                    case TargetID.StdGolem:
                    case TargetID.ConditionGolem:
                    case TargetID.PowerGolem:
                    case TargetID.LGolem:
                    case TargetID.MedGolem:
                        return new Golem(id);
                    default:
                        switch(GetTrashID(id))
                        {
                            case TrashID.WallOfGhosts:
                                return new SpiritRace(id);
                            case TrashID.HauntingStatue:
                                return new TwistedCastle((int)ArcDPSEnums.TargetID.DummyTarget);
                            case TrashID.VoidAmalgamate:
                                return new HarvestTemple(id);
                            case TrashID.AncientInvokedHydra:
                                return new Qadim((int)ArcDPSEnums.TargetID.Qadim);
                            case TrashID.VoidMelter:
                                if (agentData.GetNPCsByID(ArcDPSEnums.TrashID.VoidAmalgamate).Any())
                                {
                                    return new HarvestTemple((int)ArcDPSEnums.TargetID.GadgetTheDragonVoid1);
                                }
                                break;
                        }
                        break;
                }
                break;

            case AgentType.Gadget:
                switch (targetID)
                {
                    // Raids
                    case TargetID.EtherealBarrierGadget:
                        return new SpiritRace(id);
                    case TargetID.ConjuredAmalgamate:
                    case TargetID.ConjuredAmalgamate_CHINA:
                    case TargetID.CALeftArm_CHINA:
                    case TargetID.CARightArm_CHINA:
                        return new ConjuredAmalgamate(id);
                    // Strike Missions
                    case TargetID.GadgetTheDragonVoid1:
                    case TargetID.GadgetTheDragonVoid2:
                        // This will most likely require a chinese client version
                        if (agentData.GetNPCsByID(TrashID.VoidAmalgamate).Any())
                        {
                            return new HarvestTemple((int)TargetID.GadgetTheDragonVoid1);
                        }
                        break;
                    // Open World
                    case TargetID.SooWonOW:
                        return new SooWon(id);
                }
                break;
        }
        return new UnknownFightLogic(id);
    }

    internal void CompleteFightName(CombatData combatData, AgentData agentData)
    {
        FightName = Logic.GetLogicName(combatData, agentData)
            + (_encounterMode == EncounterMode.CM ? " CM" : "")
            + (_encounterMode == EncounterMode.LegendaryCM ? " LCM" : "")
            + (_encounterMode == EncounterMode.Story ? " Story" : "")
            + (IsLateStart && !MissingPreEvent ? " (Late Start)" : "")
            + (MissingPreEvent ? " (No Pre-Event)" : "");
    }

    public IReadOnlyList<PhaseData> GetPhases(ParsedEvtcLog log)
    {

        if (_phases.Count == 0)
        {
            _phases = Logic.GetPhases(log, log.ParserSettings.ParsePhases);
            _phases.AddRange(Logic.GetBreakbarPhases(log, log.ParserSettings.ParsePhases));
            _phases.RemoveAll(x => x.AllTargets.Count == 0);
            if (_phases.Any(phase => phase.AllTargets.Any(target => !Logic.Targets.Contains(target))))
            {
                throw new InvalidOperationException("Phases can only have targets");
            }
            if (_phases.Exists(x => x.BreakbarPhase && x.Targets.Count != 1))
            {
                throw new InvalidOperationException("Breakbar phases can only have one target");
            }
            _phases.RemoveAll(x => x.DurationInMS < ParserHelper.PhaseTimeLimit);
            _phases.Sort((x, y) =>
            {
                int startCompare = x.Start.CompareTo(y.Start);
                if (startCompare == 0)
                {
                    return -x.DurationInMS.CompareTo(y.DurationInMS);
                }
                return startCompare;
            });
        }
        return _phases;
    }

    public IReadOnlyList<SingleActor> GetMainTargets(ParsedEvtcLog log)
    {
        return GetPhases(log)[0].Targets;
    }

    // Setters
    internal void ProcessEncounterStatus(CombatData combatData, AgentData agentData)
    {
        if (_encounterMode == EncounterMode.NotSet)
        {
            _encounterMode = Logic.GetEncounterMode(combatData, agentData, this);
            if (_encounterMode == EncounterMode.Story)
            {
                Logic.InvalidateEncounterID();
            }
            _encounterStartStatus = Logic.GetEncounterStartStatus(combatData, agentData, this);
        }
    }

    internal void SetSuccess(bool success, long fightEnd)
    {
        Success = success;
        FightEnd = fightEnd;
    }

    internal void ApplyOffset(long offset)
    {
        LogOffset = offset;
        FightEnd += LogStart - offset;
        LogStart -= offset;
        LogEnd -= offset;
    }
}
