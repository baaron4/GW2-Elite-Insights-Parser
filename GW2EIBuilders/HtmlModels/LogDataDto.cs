﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using GW2EIBuilders.HtmlModels.EXTBarrier;
using GW2EIBuilders.HtmlModels.EXTHealing;
using GW2EIBuilders.HtmlModels.HTMLActors;
using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLMetaData;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using Tracing;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

[assembly: InternalsVisibleTo("GW2EIParser.tst")]
namespace GW2EIBuilders.HtmlModels;

using BuffInstanceItem = long[];

//TODO(Rennorb) @perf
internal class LogDataDto
{
    public List<TargetDto>? Targets;
    public List<PlayerDto>? Players;
    public List<EnemyDto>? Enemies;
    public List<PhaseDto>? Phases;
    // Present buffs and damamge modifiers
    public List<long>? Boons;
    public List<long>? OffBuffs;
    public List<long>? SupBuffs;
    public List<long>? DefBuffs;
    public List<long>? Debuffs;
    public List<long>? GearBuffs;
    public List<long>? Nourishments;
    public List<long>? Enhancements;
    public List<long>? OtherConsumables;
    public List<BuffInstanceItem>? InstanceBuffs;
    public List<long> DmgModifiersItem = new (20);
    public List<long> DmgIncModifiersItem = new(20);
    public List<long> DmgModifiersCommon = new(20);
    public List<long> DmgIncModifiersCommon = new(20);
    public Dictionary<string, List<long>> DmgModifiersPers = new(20);
    public Dictionary<string, List<long>> DmgIncModifiersPers = new(20);
    public Dictionary<string, List<long>> PersBuffs = new(20);
    public List<long> Conditions = [];
    // Dictionaries
    public Dictionary<string, SkillDto> SkillMap = new(200);
    public Dictionary<string, BuffDto> BuffMap = new(100);
    public Dictionary<string, DamageModDto> DamageModMap = new(50);
    public Dictionary<string, DamageModDto> DamageIncModMap = new(50);
    public List<MechanicDto> MechanicMap = new(30);
    // Extra components
    public CombatReplayDto? CrData;
    public ChartDataDto? GraphData;
    public HealingStatsExtension? HealingStatsExtension;
    public BarrierStatsExtension? BarrierStatsExtension;
    // meta data
    public string EncounterDuration;
    public string EvtcRecordingDuration;
    public bool Success;
    public bool Wvw;
    public bool HasCommander;
    public bool Targetless;
    public string FightNameNoMode;
    public string FightName;
    public string FightIcon;
    public string? FightMode;
    public string FightStartStatus;
    public bool LightTheme;
    public bool NoMechanics;
    public bool SingleGroup;
    public bool HasBreakbarDamage;
    public List<string>? LogErrors;
    public string EncounterStart;
    public string EncounterEnd;
    public string? InstanceStart;
    public string? InstanceIP;
    public string InstancePrivacy;
    public string ArcVersion;
    public long EvtcBuild;
    public ulong Gw2Build;
    public long TriggerID;
    public long EncounterID;
    public long MapID;
    public string Parser;
    public string RecordedBy;
    public string RecordedAccountBy;
    public int FractalScale;
    public List<string> UploadLinks;
    public List<string>? UsedExtensions;
    public List<List<string>>? PlayersRunningExtensions;
    //
    private LogDataDto(ParsedEvtcLog log, bool light, Version parserVersion, string[] uploadLinks)
    {
        log.UpdateProgressWithCancellationCheck("HTML: building Meta Data");
        EncounterStart = log.LogData.LogStartStd;
        EncounterEnd = log.LogData.LogEndStd;
        if (log.LogData.LogInstanceStartStd != null)
        {
            InstanceStart = log.LogData.LogInstanceStartStd;
            InstanceIP = log.LogData.LogInstanceIP;
        }
        var mapIDEvent = log.CombatData.GetMapIDEvents().FirstOrDefault();
        if (mapIDEvent != null)
        {
            MapID = mapIDEvent.MapID;
        }
        ArcVersion = log.LogData.ArcVersion;
        EvtcBuild = log.LogData.EvtcBuild;
        Gw2Build = log.LogData.GW2Build;
        TriggerID = log.FightData.TriggerID;
        EncounterID = log.FightData.Logic.EncounterID;
        Parser = "Elite Insights " + parserVersion.ToString();
        RecordedBy = log.LogData.PoVName;
        RecordedAccountBy = log.LogData.PoVAccount;
        var fractaleScaleEvent = log.CombatData.GetFractalScaleEvent();
        FractalScale = fractaleScaleEvent != null ? fractaleScaleEvent.Scale : 0;
        UploadLinks = uploadLinks.ToList();
        if (log.LogData.UsedExtensions.Any())
        {
            UsedExtensions = [];
            PlayersRunningExtensions = [];
            foreach (ExtensionHandler extension in log.LogData.UsedExtensions)
            {
                UsedExtensions.Add(extension.Name + " - " + extension.Version);
                var set = new HashSet<string>();
                if (log.LogData.PoV != null)
                {
                    set.Add(log.FindActor(log.LogData.PoV).Character);
                    foreach (AgentItem agent in extension.RunningExtension)
                    {
                        set.Add(log.FindActor(agent).Character);
                    }
                }
                PlayersRunningExtensions.Add(set.ToList());
            }
        }

        EncounterDuration = log.FightData.DurationString;
        EvtcRecordingDuration = log.FightData.EvtcRecordingDuration;
        Success = log.FightData.Success;
        Wvw = log.FightData.Logic.ParseMode == FightLogic.ParseModeEnum.WvW;
        Targetless = log.FightData.Logic.Targetless;
        FightNameNoMode = log.FightData.FightNameNoMode;
        FightName = log.FightData.FightName;
        FightIcon = log.FightData.Logic.Icon;
        switch (log.FightData.FightMode)
        {
            case FightData.EncounterMode.Unknown:
                FightMode = "Unknown";
                break;
            case FightData.EncounterMode.Story:
                FightMode = "Story Mode";
                break;
            case FightData.EncounterMode.Normal:
                FightMode = log.FightData.Logic.GetInstanceBuffs(log).Any(x => x.buff.ID == Emboldened) ? "Emboldened Normal Mode" : "Normal Mode";
                break;
            case FightData.EncounterMode.CM:
            case FightData.EncounterMode.CMNoName:
                FightMode = "Challenge Mode";
                break;
            case FightData.EncounterMode.LegendaryCM:
                FightMode = "Legendary Challenge Mode";
                break;
            default:
                break;
        }
        switch (log.FightData.FightStartStatus)
        {
            case FightData.EncounterStartStatus.Normal:
                break;
            case FightData.EncounterStartStatus.NotSet:
                break;
            case FightData.EncounterStartStatus.Late:
                FightStartStatus = "Late Start";
                break;
            case FightData.EncounterStartStatus.NoPreEvent:
                FightStartStatus = "No Pre-Event";
                break;
            default:
                break;
        }
        switch (log.FightData.InstancePrivacy)
        {
            case FightData.InstancePrivacyMode.Public:
                InstancePrivacy = "Public Instance";
                break;
            case FightData.InstancePrivacyMode.Private:
                InstancePrivacy = "Private Instance";
                break;
            case FightData.InstancePrivacyMode.NotApplicable:
            case FightData.InstancePrivacyMode.Unknown:
                break;
        }
        LightTheme = light;
        SingleGroup = log.PlayerList.Select(x => x.Group).Distinct().Count() == 1;
        HasBreakbarDamage = log.CombatData.HasBreakbarDamageData;
        NoMechanics = log.FightData.Logic.HasNoFightSpecificMechanics;
        if (log.LogData.LogErrors.Count > 0)
        {
            LogErrors = new List<string>(log.LogData.LogErrors);
        }
    }

    private static Dictionary<Spec, IReadOnlyList<Buff>> BuildPersonalBuffData(ParsedEvtcLog log, Dictionary<string, List<long>> persBuffDict, Dictionary<long, Buff> usedBuffs)
    {
        var boonsBySpec = new Dictionary<Spec, IReadOnlyList<Buff>>(log.FriendliesListBySpec.Count);
        // Collect all personal buffs by spec
        foreach (var pair in log.FriendliesListBySpec)
        {
            var friendlies = pair.Value;
            var specBoonIds = new HashSet<long>(log.Buffs.GetPersonalBuffsList(pair.Key).Select(x => x.ID));
            var boonToUse = new HashSet<Buff>();
            foreach (SingleActor actor in friendlies)
            {
                foreach (PhaseData phase in log.FightData.GetPhases(log))
                {
                    IReadOnlyDictionary<long, BuffStatistics> boons = actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
                    foreach (Buff boon in log.StatisticsHelper.GetPresentRemainingBuffsOnPlayer(actor))
                    {
                        if (boons.TryGetValue(boon.ID, out var uptime))
                        {
                            if (uptime.Uptime > 0 && specBoonIds.Contains(boon.ID))
                            {
                                boonToUse.Add(boon);
                            }
                        }
                    }
                }
            }
            boonsBySpec[pair.Key] = boonToUse.ToList();
        }
        foreach (KeyValuePair<Spec, IReadOnlyList<Buff>> pair in boonsBySpec)
        {
            persBuffDict[pair.Key.ToString()] = new(pair.Value.Count);
            foreach (Buff boon in pair.Value)
            {
                persBuffDict[pair.Key.ToString()].Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
        }
        return boonsBySpec;
    }

    private static Dictionary<Spec, IReadOnlyList<OutgoingDamageModifier>> BuildPersonalOutgoingDamageModData(ParsedEvtcLog log, Dictionary<string, List<long>> dgmModDict, HashSet<OutgoingDamageModifier> usedDamageMods)
    {
        var damageModBySpecs = new Dictionary<Spec, IReadOnlyList<OutgoingDamageModifier>>(log.FriendliesListBySpec.Count);
        // Collect all personal damage mods by spec
        foreach (var pair in log.FriendliesListBySpec)
        {
            var specDamageModsID = new HashSet<int>(log.DamageModifiers.GetOutgoingModifiersPerSpec(pair.Key).Select(x => x.ID));
            var damageModsToUse = new HashSet<OutgoingDamageModifier>(pair.Value.Count);
            foreach (SingleActor actor in pair.Value)
            {
                var presentDamageMods = new HashSet<int>(actor.GetPresentOutgoingDamageModifier(log).Intersect(specDamageModsID));
                foreach (int modID in presentDamageMods)
                {
                    damageModsToUse.Add(log.DamageModifiers.OutgoingDamageModifiersByID[modID]);
                }
            }
            damageModBySpecs[pair.Key] = damageModsToUse.ToList();
        }
        foreach (KeyValuePair<Spec, IReadOnlyList<OutgoingDamageModifier>> pair in damageModBySpecs)
        {
            dgmModDict[pair.Key.ToString()] = new(pair.Value.Count);
            foreach (OutgoingDamageModifier mod in pair.Value)
            {
                dgmModDict[pair.Key.ToString()].Add(mod.ID);
                usedDamageMods.Add(mod);
            }
        }
        return damageModBySpecs;
    }

    private static Dictionary<Spec, IReadOnlyList<IncomingDamageModifier>> BuildPersonalIncomingDamageModData(ParsedEvtcLog log, Dictionary<string, List<long>> dgmModDict, HashSet<IncomingDamageModifier> usedDamageMods)
    {
        var damageModBySpecs = new Dictionary<Spec, IReadOnlyList<IncomingDamageModifier>>(log.FriendliesListBySpec.Count);
        // Collect all personal damage mods by spec
        foreach (var pair in log.FriendliesListBySpec)
        {
            var specDamageModsID = new HashSet<int>(log.DamageModifiers.GetIncomingModifiersPerSpec(pair.Key).Select(x => x.ID));
            var damageModsToUse = new HashSet<IncomingDamageModifier>(pair.Value.Count);
            foreach (SingleActor actor in pair.Value)
            {
                var presentDamageMods = new HashSet<int>(actor.GetPresentIncomingDamageModifier(log).Intersect(specDamageModsID));
                foreach (int modID in presentDamageMods)
                {
                    damageModsToUse.Add(log.DamageModifiers.IncomingDamageModifiersByID[modID]);
                }
            }
            damageModBySpecs[pair.Key] = damageModsToUse.ToList();
        }
        foreach (KeyValuePair<Spec, IReadOnlyList<IncomingDamageModifier>> pair in damageModBySpecs)
        {
            dgmModDict[pair.Key.ToString()] = new (pair.Value.Count);
            foreach (IncomingDamageModifier mod in pair.Value)
            {
                dgmModDict[pair.Key.ToString()].Add(mod.ID);
                usedDamageMods.Add(mod);
            }
        }
        return damageModBySpecs;
    }

    [MemberNotNull(nameof(Boons))]
    [MemberNotNull(nameof(Conditions))]
    [MemberNotNull(nameof(OffBuffs))]
    [MemberNotNull(nameof(SupBuffs))]
    [MemberNotNull(nameof(DefBuffs))]
    [MemberNotNull(nameof(Debuffs))]
    [MemberNotNull(nameof(GearBuffs))]
    [MemberNotNull(nameof(Nourishments))]
    [MemberNotNull(nameof(Enhancements))]
    [MemberNotNull(nameof(OtherConsumables))]
    [MemberNotNull(nameof(InstanceBuffs))]
    private void BuildBuffDictionaries(ParsedEvtcLog log, Dictionary<long, Buff> usedBuffs)
    {
        StatisticsHelper statistics = log.StatisticsHelper;
        Boons = new(statistics.PresentBoons.Count);
        foreach (Buff boon in statistics.PresentBoons)
        {
            Boons.Add(boon.ID);
            usedBuffs[boon.ID] = boon;
        }
        Conditions = new(statistics.PresentConditions.Count);
        foreach (Buff condition in statistics.PresentConditions)
        {
            Conditions.Add(condition.ID);
            usedBuffs[condition.ID] = condition;
        }
        OffBuffs = new(statistics.PresentConditions.Count);
        foreach (Buff offBuff in statistics.PresentOffbuffs)
        {
            OffBuffs.Add(offBuff.ID);
            usedBuffs[offBuff.ID] = offBuff;
        }
        SupBuffs = new(statistics.PresentSupbuffs.Count);
        foreach (Buff supBuff in statistics.PresentSupbuffs)
        {
            SupBuffs.Add(supBuff.ID);
            usedBuffs[supBuff.ID] = supBuff;
        }
        DefBuffs = new(statistics.PresentDefbuffs.Count);
        foreach (Buff defBuff in statistics.PresentDefbuffs)
        {
            DefBuffs.Add(defBuff.ID);
            usedBuffs[defBuff.ID] = defBuff;
        }
        Debuffs = new(statistics.PresentDebuffs.Count);
        foreach (Buff debuff in statistics.PresentDebuffs)
        {
            Debuffs.Add(debuff.ID);
            usedBuffs[debuff.ID] = debuff;
        }
        GearBuffs = new(statistics.PresentGearbuffs.Count);
        foreach (Buff gearBuff in statistics.PresentGearbuffs)
        {
            GearBuffs.Add(gearBuff.ID);
            usedBuffs[gearBuff.ID] = gearBuff;
        }
        Nourishments = new(statistics.PresentNourishements.Count);
        foreach (Buff nourishment in statistics.PresentNourishements)
        {
            Nourishments.Add(nourishment.ID);
            usedBuffs[nourishment.ID] = nourishment;
        }
        Enhancements = new(statistics.PresentEnhancements.Count);
        foreach (Buff enhancement in statistics.PresentEnhancements)
        {
            Enhancements.Add(enhancement.ID);
            usedBuffs[enhancement.ID] = enhancement;
        }
        OtherConsumables = new(statistics.PresentOtherConsumables.Count);
        foreach (Buff otherConsumables in statistics.PresentOtherConsumables)
        {
            OtherConsumables.Add(otherConsumables.ID);
            usedBuffs[otherConsumables.ID] = otherConsumables;
        }
        var instanceBuffs = log.FightData.Logic.GetInstanceBuffs(log);
        InstanceBuffs = new(instanceBuffs.Count);
        foreach ((Buff instanceBuff, int stack) in instanceBuffs)
        {
            InstanceBuffs.Add([instanceBuff.ID, stack]);
            usedBuffs[instanceBuff.ID] = instanceBuff;
        }
    }

    private void BuildOutgoingDamageModDictionaries(ParsedEvtcLog log, HashSet<OutgoingDamageModifier> usedDamageMods,
        HashSet<int> allDamageMods, List<OutgoingDamageModifier> commonDamageModifiers, List<OutgoingDamageModifier> itemDamageModifiers)
    {
        foreach (SingleActor actor in log.Friendlies)
        {
            allDamageMods.UnionWith(actor.GetPresentOutgoingDamageModifier(log));
        }
        if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.Common, out var list))
        {
            foreach (OutgoingDamageModifier dMod in list)
            {
                if (allDamageMods.Contains(dMod.ID))
                {
                    commonDamageModifiers.Add(dMod);
                    DmgModifiersCommon.Add(dMod.ID);
                    usedDamageMods.Add(dMod);
                }
            }
        }
        if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.FightSpecific, out list))
        {
            foreach (OutgoingDamageModifier dMod in list)
            {
                if (allDamageMods.Contains(dMod.ID))
                {
                    commonDamageModifiers.Add(dMod);
                    DmgModifiersCommon.Add(dMod.ID);
                    usedDamageMods.Add(dMod);
                }
            }
        }
        if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.Item, out list))
        {
            foreach (OutgoingDamageModifier dMod in list)
            {
                if (allDamageMods.Contains(dMod.ID))
                {
                    itemDamageModifiers.Add(dMod);
                    DmgModifiersItem.Add(dMod.ID);
                    usedDamageMods.Add(dMod);
                }
            }
        }
        if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.Gear, out list))
        {
            foreach (OutgoingDamageModifier dMod in list)
            {
                if (allDamageMods.Contains(dMod.ID))
                {
                    itemDamageModifiers.Add(dMod);
                    DmgModifiersItem.Add(dMod.ID);
                    usedDamageMods.Add(dMod);
                }
            }
        }
    }

    private void BuildIncomingDamageModDictionaries(ParsedEvtcLog log, HashSet<IncomingDamageModifier> usedIncDamageMods,
        HashSet<int> allIncDamageMods, List<IncomingDamageModifier> commonIncDamageModifiers, List<IncomingDamageModifier> itemIncDamageModifiers)
    {
        foreach (SingleActor actor in log.Friendlies)
        {
            allIncDamageMods.UnionWith(actor.GetPresentIncomingDamageModifier(log));
        }
        if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.Common, out var list))
        {
            foreach (IncomingDamageModifier dMod in list)
            {
                if (allIncDamageMods.Contains(dMod.ID))
                {
                    commonIncDamageModifiers.Add(dMod);
                    DmgIncModifiersCommon.Add(dMod.ID);
                    usedIncDamageMods.Add(dMod);
                }
            }
        }
        if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.FightSpecific, out list))
        {
            foreach (IncomingDamageModifier dMod in list)
            {
                if (allIncDamageMods.Contains(dMod.ID))
                {
                    commonIncDamageModifiers.Add(dMod);
                    DmgIncModifiersCommon.Add(dMod.ID);
                    usedIncDamageMods.Add(dMod);
                }
            }
        }
        if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.Item, out list))
        {
            foreach (IncomingDamageModifier dMod in list)
            {
                if (allIncDamageMods.Contains(dMod.ID))
                {
                    itemIncDamageModifiers.Add(dMod);
                    DmgIncModifiersItem.Add(dMod.ID);
                    usedIncDamageMods.Add(dMod);
                }
            }
        }
        if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.Gear, out list))
        {
            foreach (IncomingDamageModifier dMod in list)
            {
                if (allIncDamageMods.Contains(dMod.ID))
                {
                    itemIncDamageModifiers.Add(dMod);
                    DmgIncModifiersItem.Add(dMod.ID);
                    usedIncDamageMods.Add(dMod);
                }
            }
        }
    }

    public static LogDataDto BuildLogData(ParsedEvtcLog log, bool cr, bool light, Version parserVersion, string[] uploadLinks)
    {
        using var _t = new AutoTrace("BuildLogData");

        var usedBuffs = new Dictionary<long, Buff>(128); //TODO(Rennorb) @perf: find capacity dependencies
        var usedDamageMods = new HashSet<OutgoingDamageModifier>(32); //TODO(Rennorb) @perf: find capacity dependencies
        var usedIncDamageMods = new HashSet<IncomingDamageModifier>(16); //TODO(Rennorb) @perf: find capacity dependencies
        var usedSkills = new Dictionary<long, SkillItem>(256); //TODO(Rennorb) @perf: find capacity dependencies

        log.UpdateProgressWithCancellationCheck("HTML: building Log Data");
        var logData = new LogDataDto(log, light, parserVersion, uploadLinks);
        if (cr)
        {
            log.UpdateProgressWithCancellationCheck("HTML: building Combat Replay");
            logData.CrData = new CombatReplayDto(log, usedSkills, usedBuffs);
        }
        _t.Log("built main data");

        log.UpdateProgressWithCancellationCheck("HTML: building Graph Data");
        logData.GraphData = new ChartDataDto(log);
        _t.Log("built graph data");
        
        log.UpdateProgressWithCancellationCheck("HTML: building Players");
        logData.Players = new(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            logData.HasCommander = logData.HasCommander || (actor is Player p && p.IsCommander(log));
            logData.Players.Add(new PlayerDto(actor, log, ActorDetailsDto.BuildPlayerData(log, actor, usedSkills, usedBuffs)));
        }
        _t.Log("built player data");

        log.UpdateProgressWithCancellationCheck("HTML: building Enemies");
        var enemies = log.MechanicData.GetEnemyList(log, log.FightData.FightStart, log.FightData.FightEnd);
        logData.Enemies = new(enemies.Count);
        foreach (SingleActor enemy in enemies)
        {
            logData.Enemies.Add(new EnemyDto() { Name = enemy.Character });
        }
        _t.Log("built enemy data");

        log.UpdateProgressWithCancellationCheck("HTML: building Targets");
        logData.Targets = new(log.FightData.Logic.Targets.Count);
        foreach (SingleActor target in log.FightData.Logic.Targets)
        {
            var targetDto = new TargetDto(target, log, ActorDetailsDto.BuildTargetData(log, target, usedSkills, usedBuffs, cr));
            logData.Targets.Add(targetDto);
        }
        _t.Log("built target data");
        
        log.UpdateProgressWithCancellationCheck("HTML: building Skill/Buff/Damage Modifier dictionaries");
        Dictionary<Spec, IReadOnlyList<Buff>> persBuffDict = BuildPersonalBuffData(log, logData.PersBuffs, usedBuffs);
        Dictionary<Spec, IReadOnlyList<OutgoingDamageModifier>> persOutDamageModDict = BuildPersonalOutgoingDamageModData(log, logData.DmgModifiersPers, usedDamageMods);
        Dictionary<Spec, IReadOnlyList<IncomingDamageModifier>> persIncDamageModDict = BuildPersonalIncomingDamageModData(log, logData.DmgIncModifiersPers, usedIncDamageMods);
        
        var allOutDamageMods         = new HashSet<int>(60);
        var commonOutDamageModifiers = new List<OutgoingDamageModifier>(20);
        var itemOutDamageModifiers   = new List<OutgoingDamageModifier>(20);
        var allIncDamageMods         = new HashSet<int>(60);
        var commonIncDamageModifiers = new List<IncomingDamageModifier>(20);
        var itemIncDamageModifiers   = new List<IncomingDamageModifier>(20);
        
        logData.BuildBuffDictionaries(log, usedBuffs);
        logData.BuildOutgoingDamageModDictionaries(log, usedDamageMods, allOutDamageMods, commonOutDamageModifiers, itemOutDamageModifiers);
        logData.BuildIncomingDamageModDictionaries(log, usedIncDamageMods, allIncDamageMods, commonIncDamageModifiers, itemIncDamageModifiers);
        _t.Log("built modifier dicts");
        
        log.UpdateProgressWithCancellationCheck("HTML: building Phases");
        IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
        logData.Phases = new(phases.Count);
        for (int i = 0; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            var phaseDto = new PhaseDto(phase, phases, log, persBuffDict,
                commonOutDamageModifiers, itemOutDamageModifiers, persOutDamageModDict,
                commonIncDamageModifiers, itemIncDamageModifiers, persIncDamageModDict
                );
            logData.Phases.Add(phaseDto);
        }
        _t.Log("built phases");
        
        if (log.CombatData.HasEXTHealing)
        {
            log.UpdateProgressWithCancellationCheck("HTML: building Healing Extension");
            logData.HealingStatsExtension = new HealingStatsExtension(log, usedSkills, usedBuffs);
            if (log.CombatData.HasEXTBarrier)
            {
                log.UpdateProgressWithCancellationCheck("HTML: building Barrier Extension");
                logData.BarrierStatsExtension = new BarrierStatsExtension(log, usedSkills, usedBuffs);
            }
        }
        _t.Log("built healing data");
        
        SkillDto.AssembleSkills(usedSkills.Values, logData.SkillMap, log);
        DamageModDto.AssembleDamageModifiers(usedDamageMods, logData.DamageModMap);
        DamageModDto.AssembleDamageModifiers(usedIncDamageMods, logData.DamageIncModMap);
        BuffDto.AssembleBuffs(usedBuffs.Values, logData.BuffMap, log);
        MechanicDto.BuildMechanics(log.MechanicData.GetPresentMechanics(log, log.FightData.FightStart, log.FightData.FightEnd), logData.MechanicMap);

        Trace.TrackAverageStat("usedBuffs", usedBuffs.Count);
        Trace.TrackAverageStat("usedDamageMods", usedDamageMods.Count);
        Trace.TrackAverageStat("usedIncDamageMods", usedIncDamageMods.Count);
        Trace.TrackAverageStat("usedSkills", usedSkills.Count);

        return logData;
    }

}
