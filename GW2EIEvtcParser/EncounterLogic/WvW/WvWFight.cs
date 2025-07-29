using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.MapIDs;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class WvWFight : FightLogic
{
    private readonly string _defaultName;
    private readonly bool _detailed;
    private bool _foundSkillMode;
    private bool _isGuildHall;
    private readonly bool _isFromInstance;
    public WvWFight(int triggerID, bool detailed, bool full = false) : base(triggerID)
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
        EncounterCategoryInformation.Category = FightCategory.WvW;
        EncounterID |= EncounterIDs.EncounterMasks.WvWMask;
        if (_isFromInstance)
        {
            EncounterID |= EncounterIDs.WvWMasks.FullInstanceMask;
        }
        MechanicList.Add(new MechanicGroup([
            new PlayerDamageMechanic(new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Blue), "Kllng.Blw.Player", "Killing Blows inflicted by Squad Players to enemy Players", "Killing Blows to enemy Players", 0, (log, a) => {
                if (a.Type != AgentItem.AgentType.Player)
                {
                    return new List<HealthDamageEvent>();
                }
                return log.FindActor(a).GetDamageEvents(null, log); //TODO(Rennorb) @perf
            }).UsingChecker((x, log) => x.HasKilled && (x.To.Type == AgentItem.AgentType.NonSquadPlayer || x.To.IsSpecies(TargetID.WorldVersusWorld))),
            new EnemyDamageMechanic(new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "Kllng.Blw.Enemy", "Killing Blows inflicted enemy Players by Squad Players", "Killing Blows received by enemies", 0, (log, a) => {
                return log.FindActor(a).GetDamageTakenEvents(null, log); //TODO(Rennorb) @perf
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
            var detailedPhase = new PhaseData(phases[0].Start, phases[0].End, _isFromInstance ? "Detailed Full Instance" : "Detailed Full Fight");
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

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        return GetGenericFightOffset(fightData);
    }
    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.NotApplicable;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        MapIDEvent? mapID = log.CombatData.GetMapIDEvents().LastOrDefault();
        if (mapID == null)
        {
            return base.GetCombatMapInternal(log);
        }
        return mapID.MapID switch
        {
            EternalBattleground => new CombatReplayMap(CombatReplayEternalBattlegrounds, (954, 1000), (-36864 + 950, -36864 + 2250, 36864 + 950, 36864 + 2250)),
            GreenAlpineBorderland => new CombatReplayMap(CombatReplayAlpineBorderlands, (697, 1000), (-30720, -43008, 30720, 43008)),
            BlueAlpineBorderland => new CombatReplayMap(CombatReplayAlpineBorderlands, (697, 1000), (-30720, -43008, 30720, 43008)),
            RedDesertBorderland => new CombatReplayMap(CombatReplayDesertBorderlands, (1000, 1000), (-36864, -36864, 36864, 36864)),
            EdgeOfTheMists => new CombatReplayMap(CombatReplayEdgeOfTheMists, (3556, 3646), (-36864, -36864, 36864, 36864)),
            _ => base.GetCombatMapInternal(log),
        };
    }
    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        MapIDEvent? mapID = combatData.GetMapIDEvents().LastOrDefault();
        if (mapID == null)
        {
            return _defaultName;
        }
        switch (mapID.MapID)
        {
            case EternalBattleground:
                EncounterCategoryInformation.SubCategory = SubFightCategory.EternalBattlegrounds;
                EncounterID |= EncounterIDs.WvWMasks.EternalBattlegroundsMask;
                Icon = InstanceIconEternalBattlegrounds;
                return _defaultName + " - Eternal Battlegrounds";
            case GreenAlpineBorderland:
                EncounterCategoryInformation.SubCategory = SubFightCategory.GreenAlpineBorderlands;
                EncounterID |= EncounterIDs.WvWMasks.GreenAlpineBorderlandsMask;
                Icon = InstanceIconGreenBorderlands;
                return _defaultName + " - Green Alpine Borderlands";
            case BlueAlpineBorderland:
                EncounterCategoryInformation.SubCategory = SubFightCategory.BlueAlpineBorderlands;
                EncounterID |= EncounterIDs.WvWMasks.BlueAlpineBorderlandsMask;
                Icon = InstanceIconBlueBorderlands;
                return _defaultName + " - Blue Alpine Borderlands";
            case RedDesertBorderland:
                EncounterCategoryInformation.SubCategory = SubFightCategory.RedDesertBorderlands;
                EncounterID |= EncounterIDs.WvWMasks.RedDesertBorderlandsMask;
                Icon = InstanceIconRedBorderlands;
                return _defaultName + " - Red Desert Borderlands";
            case ObsidianSanctum:
                EncounterCategoryInformation.SubCategory = SubFightCategory.ObsidianSanctum;
                EncounterID |= EncounterIDs.WvWMasks.ObsidianSanctumMask;
                Icon = InstanceIconEternalBattlegrounds;
                return _defaultName + " - Obsidian Sanctum";
            case EdgeOfTheMists:
                EncounterCategoryInformation.SubCategory = SubFightCategory.EdgeOfTheMists;
                EncounterID |= EncounterIDs.WvWMasks.EdgeOfTheMistsMask;
                return _defaultName + " - Edge of the Mists";
            case ArmisticeBastion:
                EncounterCategoryInformation.SubCategory = SubFightCategory.ArmisticeBastion;
                EncounterID |= EncounterIDs.WvWMasks.ArmisticeBastionMask;
                Icon = InstanceIconEternalBattlegrounds;
                return _defaultName + " - Armistice Bastion";
            case GildedHollow1:
            case GildedHollow2:
            case GildedHollow3:
            case GildedHollow4:
            case GildedHollow5:
                _isGuildHall = true;
                EncounterCategoryInformation.SubCategory = SubFightCategory.GuildHall;
                EncounterID |= EncounterIDs.WvWMasks.GildedHollowMask;
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
                EncounterCategoryInformation.SubCategory = SubFightCategory.GuildHall;
                EncounterID |= EncounterIDs.WvWMasks.LostPrecipiceMask;
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
                EncounterCategoryInformation.SubCategory = SubFightCategory.GuildHall;
                EncounterID |= EncounterIDs.WvWMasks.WindsweptHavenMask;
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
                EncounterCategoryInformation.SubCategory = SubFightCategory.GuildHall;
                EncounterID |= EncounterIDs.WvWMasks.IsleOfReflectionMask;
                Extension = _detailed ? "detailed_gh" : "gh";
                if (!_foundSkillMode)
                {
                    SkillMode = SkillModeEnum.PvE;
                }
                //Icon = InstanceIconEternalBattlegrounds;
                return (_detailed ? "Detailed " : "") + "Isle of Reflection";
        }
        return _defaultName;
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);
        if (_isGuildHall)
        {
            var modes = new List<BuffEvent>(log.CombatData.GetBuffData(GuildHallPvEMode));
            modes.AddRange(log.CombatData.GetBuffData(GuildHallsPvPMode));
            modes.AddRange(log.CombatData.GetBuffData(GuildHallWvWMode));
            modes.SortByTime();
            var usedModes = modes.Select(x => x.BuffID).Distinct();
            foreach (long buffID in usedModes)
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIDs[buffID], 1));
            }
            // When buff is missing on a player, they are in PvE mode
            if (!usedModes.Contains(GuildHallPvEMode) && log.PlayerList.Any(x => !modes.Any(y => y.To.Is(x.AgentItem))))
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIDs[GuildHallPvEMode], 1));
            }
        }
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        fightData.SetSuccess(true, fightData.FightEnd);
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        AgentItem dummyAgent = agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, _detailed ? "Dummy PvP Agent" : "Enemy Players", ParserHelper.Spec.NPC, TargetID.WorldVersusWorld, true);
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
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
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
        FinalizeComputeFightTargets();
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return new[] { TargetID.WorldVersusWorld };
    }
}
