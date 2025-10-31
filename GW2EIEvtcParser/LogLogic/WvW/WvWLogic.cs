using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIGW2API;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.MapIDs;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class WvWLogic : LogLogic
{
    private readonly string _defaultName;
    private readonly bool _detailed;
    private bool _foundSkillMode;
    private bool _isGuildHall;
    private readonly bool _isFromInstance;
    public WvWLogic(int triggerID, bool detailed, bool full = false) : base(triggerID)
    {
        ParseMode = ParseModeEnum.WvW;
        SkillMode = SkillModeEnum.WvW;
        Icon = EncounterIconWvW;
        _detailed = detailed;
        Extension = _detailed ? "detailed_wvw" : "wvw";
        _defaultName = _detailed ? "Detailed WvW" : "World vs World";
        if (full)
        {
            Extension += "_full";
            _defaultName += " Full";
        }
        _isFromInstance = full;
        LogCategoryInformation.Category = LogCategory.WvW;
        LogID |= LogIDs.LogMasks.WvWMask;
        if (_isFromInstance)
        {
            LogID |= LogIDs.WvWMasks.FullInstanceMask;
        }
        MechanicList.Add(new MechanicGroup([
            new PlayerDamageMechanic(new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Blue), "Kllng.Blw.Player", "Killing Blows inflicted by Squad Players to enemy Players", "Killing Blows to enemy Players", 0, (log, a) => {
                if (a.Type != AgentItem.AgentType.Player)
                {
                    return new List<HealthDamageEvent>();
                }
                return log.FindActor(a).GetDamageEvents(null, log); //TODO_PERF(Rennorb)
            }).UsingChecker((x, log) => x.HasKilled && (x.To.Type == AgentItem.AgentType.NonSquadPlayer || x.To.IsSpecies(TargetID.WorldVersusWorld))),
            new EnemyDamageMechanic(new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "Kllng.Blw.Enemy", "Killing Blows inflicted enemy Players by Squad Players", "Killing Blows received by enemies", 0, (log, a) => {
                return log.FindActor(a).GetDamageTakenEvents(null, log); //TODO_PERF(Rennorb)
            }).UsingChecker((x, log) => x.HasKilled && x.CreditedFrom.Type == AgentItem.AgentType.Player),
        ]));
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.WorldVersusWorld)) ?? throw new MissingKeyActorsException("Main target of the fight not found");
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }
        if (_detailed)
        {
            PhaseData detailedPhase = _isFromInstance ? new InstancePhaseData(phases[0].Start, phases[0].End, "Detailed Full Instance", log) : new EncounterPhaseData(phases[0].Start, phases[0].End, "Detailed Full Fight", log);
            detailedPhase.AddTargets(Targets, log);
            if (detailedPhase.Targets.Any())
            {
                detailedPhase.RemoveTarget(mainTarget);
            }
            if (detailedPhase.Targets.Any())
            {
                phases[0] = detailedPhase;
            }
        }
        if (_isFromInstance && log.CombatData.GetEvtcVersionEvent().Build >= ArcDPSEnums.ArcDPSBuilds.LogStartLogEndPerCombatSequenceOnInstanceLogs)
        {
            var fightPhases = GetPhasesBySquadCombatStartEnd(log);
            fightPhases.ForEach(x =>
            {
                x.AddTargets(phases[0].Targets.Keys, log);
                x.AddParentPhase(phases[0]);
            });
            phases.AddRange(fightPhases);
        }
        return phases;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        return GetGenericLogOffset(logData);
    }
    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.LogMode.NotApplicable;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        MapIDEvent? mapID = log.CombatData.GetMapIDEvents().LastOrDefault();
        if (mapID == null)
        {
            return base.GetCombatMapInternal(log, arenaDecorations);
        }
        CombatReplayMap crMap;
        var lifespan = (log.LogData.LogStart, log.LogData.LogEnd);
        switch (mapID.MapID)
        {
            case EternalBattleground:
                crMap = new CombatReplayMap((954, 1000), (-36864 + 950, -36864 + 2250, 36864 + 950, 36864 + 2250));
                arenaDecorations.Add(new ArenaDecoration(lifespan, CombatReplayEternalBattlegrounds, crMap));
                break;
            case GreenAlpineBorderland:
                crMap = new CombatReplayMap((697, 1000), (-30720, -43008, 30720, 43008));
                arenaDecorations.Add(new ArenaDecoration(lifespan, CombatReplayAlpineBorderlands, crMap));
                break;
            case BlueAlpineBorderland:
                crMap = new CombatReplayMap((697, 1000), (-30720, -43008, 30720, 43008));
                arenaDecorations.Add(new ArenaDecoration(lifespan, CombatReplayAlpineBorderlands, crMap));
                break;
            case RedDesertBorderland:
                crMap = new CombatReplayMap((1000, 1000), (-36864, -36864, 36864, 36864));
                arenaDecorations.Add(new ArenaDecoration(lifespan, CombatReplayDesertBorderlands, crMap));
                break;
            case EdgeOfTheMists:
                crMap = new CombatReplayMap((3556, 3646), (-36864, -36864, 36864, 36864));
                arenaDecorations.Add(new ArenaDecoration(lifespan, CombatReplayEdgeOfTheMists, crMap));
                break;
            default:
                crMap = base.GetCombatMapInternal(log, arenaDecorations);
                break;
        }
        return crMap;
    }
    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        MapIDEvent? mapEvent = combatData.GetMapIDEvents().LastOrDefault();
        if (mapEvent == null)
        {
            return _defaultName;
        }
        switch (mapEvent.MapID)
        {
            case EternalBattleground:
                LogCategoryInformation.SubCategory = SubLogCategory.EternalBattlegrounds;
                LogID |= LogIDs.WvWMasks.EternalBattlegroundsMask;
                Icon = InstanceIconEternalBattlegrounds;
                return _defaultName + " - Eternal Battlegrounds";
            case GreenAlpineBorderland:
                LogCategoryInformation.SubCategory = SubLogCategory.GreenAlpineBorderlands;
                LogID |= LogIDs.WvWMasks.GreenAlpineBorderlandsMask;
                Icon = InstanceIconGreenBorderlands;
                return _defaultName + " - Green Alpine Borderlands";
            case BlueAlpineBorderland:
                LogCategoryInformation.SubCategory = SubLogCategory.BlueAlpineBorderlands;
                LogID |= LogIDs.WvWMasks.BlueAlpineBorderlandsMask;
                Icon = InstanceIconBlueBorderlands;
                return _defaultName + " - Blue Alpine Borderlands";
            case RedDesertBorderland:
                LogCategoryInformation.SubCategory = SubLogCategory.RedDesertBorderlands;
                LogID |= LogIDs.WvWMasks.RedDesertBorderlandsMask;
                Icon = InstanceIconRedBorderlands;
                return _defaultName + " - Red Desert Borderlands";
            case ObsidianSanctum:
                LogCategoryInformation.SubCategory = SubLogCategory.ObsidianSanctum;
                LogID |= LogIDs.WvWMasks.ObsidianSanctumMask;
                Icon = InstanceIconEternalBattlegrounds;
                return _defaultName + " - Obsidian Sanctum";
            case EdgeOfTheMists:
                LogCategoryInformation.SubCategory = SubLogCategory.EdgeOfTheMists;
                LogID |= LogIDs.WvWMasks.EdgeOfTheMistsMask;
                return _defaultName + " - Edge of the Mists";
            case ArmisticeBastion:
                LogCategoryInformation.SubCategory = SubLogCategory.ArmisticeBastion;
                LogID |= LogIDs.WvWMasks.ArmisticeBastionMask;
                Icon = InstanceIconEternalBattlegrounds;
                return _defaultName + " - Armistice Bastion";
            case GildedHollow1:
            case GildedHollow2:
            case GildedHollow3:
            case GildedHollow4:
            case GildedHollow5:
                _isGuildHall = true;
                LogCategoryInformation.SubCategory = SubLogCategory.GuildHall;
                LogID |= LogIDs.WvWMasks.GildedHollowMask;
                Extension = _detailed ? "detailed_gh" : "gh";
                if (!_foundSkillMode)
                {
                    SkillMode = SkillModeEnum.PvE;
                }
                //Icon = InstanceIconEternalBattlegrounds;
                return (_detailed ? "Detailed " : "") + "Gilded Hollow";
            case LostPrecipice1:
            case LostPrecipice2:
            case LostPrecipice3:
            case LostPrecipice4:
            case LostPrecipice5:
                _isGuildHall = true;
                LogCategoryInformation.SubCategory = SubLogCategory.GuildHall;
                LogID |= LogIDs.WvWMasks.LostPrecipiceMask;
                Extension = _detailed ? "detailed_gh" : "gh";
                if (!_foundSkillMode)
                {
                    SkillMode = SkillModeEnum.PvE;
                }
                //Icon = InstanceIconEternalBattlegrounds;
                return (_detailed ? "Detailed " : "") + "Lost Precipice";
            case WindsweptHaven1:
            case WindsweptHaven2:
            case WindsweptHaven3:
            case WindsweptHaven4:
            case WindsweptHaven5:
                _isGuildHall = true;
                LogCategoryInformation.SubCategory = SubLogCategory.GuildHall;
                LogID |= LogIDs.WvWMasks.WindsweptHavenMask;
                Extension = _detailed ? "detailed_gh" : "gh";
                if (!_foundSkillMode)
                {
                    SkillMode = SkillModeEnum.PvE;
                }
                //Icon = InstanceIconEternalBattlegrounds;
                return (_detailed ? "Detailed " : "") + "Windswept Haven";
            case IsleOfReflection1:
            case IsleOfReflection2:
            case IsleOfReflection3:
            case IsleOfReflection4:
            case IsleOfReflection5:
                _isGuildHall = true;
                LogCategoryInformation.SubCategory = SubLogCategory.GuildHall;
                LogID |= LogIDs.WvWMasks.IsleOfReflectionMask;
                Extension = _detailed ? "detailed_gh" : "gh";
                if (!_foundSkillMode)
                {
                    SkillMode = SkillModeEnum.PvE;
                }
                //Icon = InstanceIconEternalBattlegrounds;
                return (_detailed ? "Detailed " : "") + "Isle of Reflection";
            default:
                var map = apiController.GetAPIMap(mapEvent.MapID);
                if (map != null)
                {
                    return (_detailed ? "Detailed " : "") + map.Name;
                }
                break;
        }
        return _defaultName;
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        if (_isGuildHall)
        {
            var encounterPhases = log.LogData.GetPhases(log).OfType<EncounterPhaseData>();
            long[] usedModes = [GuildHallPvEMode, GuildHallsPvPMode, GuildHallWvWMode];
            foreach (var encounterPhase in encounterPhases)
            {
                long end = encounterPhase.Success ? encounterPhase.End : (encounterPhase.End + encounterPhase.Start) / 2;
                foreach (long buffID in usedModes)
                {
                    int modeStack = (int)log.PlayerList.Select(x =>
                    {
                        if (x.GetBuffGraphs(log).TryGetValue(buffID, out var graph))
                        {
                            return graph.Values.Where(y => y.Intersects(encounterPhase.Start, end)).Max(y => y.Value);
                        }
                        else
                        {
                            return 0;
                        }
                    }).Max();
                    if (modeStack > 0)
                    {
                        instanceBuffs.Add(new(log.Buffs.BuffsByIDs[buffID], 1, encounterPhase));
                    }
                }
            }
        }
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        logData.SetSuccess(true, logData.LogEnd);
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        AgentItem dummyAgent = agentData.AddCustomNPCAgent(logData.LogStart, logData.LogEnd, _detailed ? "Dummy PvP Agent" : "Enemy Players", ParserHelper.Spec.NPC, TargetID.WorldVersusWorld, true);
        CombatItem? modeEvent = combatData.FirstOrDefault(x => (x.IsBuffApply() || x.IsBuffRemoval()) && (x.SkillID == GuildHallPvEMode || x.SkillID == GuildHallsPvPMode || x.SkillID == GuildHallWvWMode));
        if (modeEvent != null)
        {
            _foundSkillMode = true;
            switch ((long)modeEvent.SkillID)
            {
                case GuildHallPvEMode:
                    SkillMode = SkillModeEnum.PvE;
                    break;
                case GuildHallsPvPMode:
                    SkillMode = SkillModeEnum.sPvP;
                    break;
                case GuildHallWvWMode:
                    SkillMode = SkillModeEnum.WvW;
                    break;
            }
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        // Handle non squad players
        IReadOnlyList<AgentItem> aList = agentData.GetAgentByType(AgentItem.AgentType.NonSquadPlayer);
        //
        var auxTargets = new List<SingleActor>(aList.Count);
        var auxFriendlies = new List<SingleActor>(aList.Count);
        foreach (AgentItem a in aList)
        {
            var nonSquadPlayer = new PlayerNonSquad(a);
            List<SingleActor> actorListToFill = nonSquadPlayer.IsFriendlyPlayer ? auxFriendlies : auxTargets;
            actorListToFill.Add(nonSquadPlayer);
        }
        _nonSquadFriendlies.AddRange(auxFriendlies.OrderBy(x => (int)x.Spec).ThenBy(x => x.AgentItem.InstID));
        //
        if (!_detailed)
        {
            var enemyPlayerList = auxTargets;
            var enemyPlayerDicts = enemyPlayerList.GroupBy(x => x.AgentItem.Agent).ToDictionary(x => x.Key, x => x.ToList());
            foreach (CombatItem c in combatData)
            {
                if (c.IsDamage(extensions))
                {
                    if (enemyPlayerDicts.TryGetValue(c.SrcAgent, out var srcs))
                    {
                        foreach (SingleActor src in srcs)
                        {
                            if (c.SrcMatchesAgent(src.AgentItem, extensions))
                            {
                                c.OverrideSrcAgent(dummyAgent);
                                break;
                            }
                        }
                    }
                    if (enemyPlayerDicts.TryGetValue(c.DstAgent, out var dsts))
                    {
                        foreach (SingleActor dst in dsts)
                        {
                            if (c.DstMatchesAgent(dst.AgentItem, extensions))
                            {
                                c.OverrideDstAgent(dummyAgent);
                                break;
                            }
                        }
                    }
                }
            }
        } 
        else
        {
            _targets.AddRange(auxTargets.OrderBy(x => (int)x.Spec).ThenBy(x => x.AgentItem.InstID));
        }
        FinalizeComputeLogTargets();
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return new[] { TargetID.WorldVersusWorld };
    }
}
