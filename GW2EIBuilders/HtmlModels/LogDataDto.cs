using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using GW2EIBuilders.HtmlModels.EXTBarrier;
using GW2EIBuilders.HtmlModels.EXTHealing;
using GW2EIBuilders.HtmlModels.HTMLActors;
using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLMetaData;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.LogLogic;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using Tracing;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

[assembly: InternalsVisibleTo("GW2EIParser.tst")]
namespace GW2EIBuilders.HtmlModels;

using BuffInstanceItem = long[];

//TODO_PERF(Rennorb)
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
    public string EvtcRecordingDuration;
    public string LogName;
    public bool Wvw;
    public bool HasCommander;
    public bool Targetless;
    public bool LightTheme;
    public bool NoMechanics;
    public bool SingleGroup;
    public bool HasBreakbarDamage;
    public List<string>? LogErrors;
    public string LogStart;
    public string LogEnd;
    public string? InstanceStart;
    public string? InstanceIP;
    public string InstancePrivacy;
    public string ArcVersion;
    public long EvtcBuild;
    public ulong Gw2Build;
    public long TriggerID;
    public long LogID;
    public long MapID;
    public string Parser;
    public string RecordedBy;
    public string RecordedAccountBy;
    public int FractalScale;
    public List<string> UploadLinks;
    public List<string>? UsedExtensions;
    public List<List<string>>? PlayersRunningExtensions;
    //
    private LogDataDto(ParsedEvtcLog log, bool light, Version parserVersion, UploadResults uploadLinks)
    {
        log.UpdateProgressWithCancellationCheck("HTML: building Meta Data");
        LogStart = log.LogMetadata.DateStartStd;
        LogEnd = log.LogMetadata.DateEndStd;
        if (log.LogMetadata.DateInstanceStartStd != null)
        {
            InstanceStart = log.LogMetadata.DateInstanceStartStd;
            InstanceIP = log.LogMetadata.LogInstanceIP;
        }
        var mapIDEvent = log.CombatData.GetMapIDEvents().FirstOrDefault();
        if (mapIDEvent != null)
        {
            MapID = mapIDEvent.MapID;
        }
        LogName = log.LogData.LogNameNoMode;
        ArcVersion = log.LogMetadata.ArcVersion;
        EvtcBuild = log.LogMetadata.EvtcBuild;
        Gw2Build = log.LogMetadata.GW2Build;
        TriggerID = log.LogData.TriggerID;
        LogID = log.LogData.Logic.LogID;
        Parser = "Elite Insights " + parserVersion.ToString();
        RecordedBy = log.LogMetadata.PoVName;
        RecordedAccountBy = log.LogMetadata.PoVAccount;
        var fractaleScaleEvent = log.CombatData.GetFractalScaleEvent();
        FractalScale = fractaleScaleEvent != null ? fractaleScaleEvent.Scale : 0;
        UploadLinks = [uploadLinks.DPSReportEILink];
        if (log.LogMetadata.UsedExtensions.Any())
        {
            UsedExtensions = [];
            PlayersRunningExtensions = [];
            foreach (ExtensionHandler extension in log.LogMetadata.UsedExtensions)
            {
                UsedExtensions.Add(extension.Name + " - " + extension.Version);
                var set = new HashSet<string>();
                if (log.LogMetadata.PoV != null)
                {
                    set.Add(log.FindActor(log.LogMetadata.PoV).Character);
                    foreach (AgentItem agent in extension.RunningExtension)
                    {
                        set.Add(log.FindActor(agent).Character);
                    }
                }
                PlayersRunningExtensions.Add(set.ToList());
            }
        }
        EvtcRecordingDuration = log.LogData.EvtcRecordingDuration;
        Wvw = log.LogData.Logic.ParseMode == LogLogic.ParseModeEnum.WvW;
        Targetless = log.LogData.Logic.Targetless;
        switch (log.LogData.InstancePrivacy)
        {
            case LogData.InstancePrivacyMode.Public:
                InstancePrivacy = "Public Instance";
                break;
            case LogData.InstancePrivacyMode.Private:
                InstancePrivacy = "Private Instance";
                break;
            case LogData.InstancePrivacyMode.NotApplicable:
            case LogData.InstancePrivacyMode.Unknown:
                break;
        }
        LightTheme = light;
        SingleGroup = log.PlayerList.Select(x => x.Group).Distinct().Count() == 1;
        HasBreakbarDamage = log.CombatData.HasBreakbarDamageData;
        NoMechanics = log.LogData.Logic.HasNoEncounterSpecificMechanics;
        if (log.LogMetadata.LogErrors.Count > 0)
        {
            LogErrors = [.. log.LogMetadata.LogErrors];
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
                foreach (PhaseData phase in log.LogData.GetPhases(log))
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
        var instanceBuffs = log.LogData.Logic.GetInstanceBuffs(log);
        InstanceBuffs = new(instanceBuffs.Count);
        foreach (var instanceBuff in instanceBuffs)
        {
            InstanceBuffs.Add([instanceBuff.Buff.ID, instanceBuff.Stack, instanceBuff.AttachedPhase.Start, instanceBuff.AttachedPhase.End]);
            usedBuffs[instanceBuff.Buff.ID] = instanceBuff.Buff;
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
        if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.EncounterSpecific, out list))
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
        if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.EncounterSpecific, out list))
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

    public static LogDataDto BuildLogData(ParsedEvtcLog log, bool cr, bool light, Version parserVersion, UploadResults uploadLinks)
    {
        using var _t = new AutoTrace("BuildLogData");

        var usedBuffs = new Dictionary<long, Buff>(128); //TODO_PERF(Rennorb) @find capacity dependencies
        var usedDamageMods = new HashSet<OutgoingDamageModifier>(32); //TODO_PERF(Rennorb) @find capacity dependencies
        var usedIncDamageMods = new HashSet<IncomingDamageModifier>(16); //TODO_PERF(Rennorb) @find capacity dependencies
        var usedSkills = new Dictionary<long, SkillItem>(256); //TODO_PERF(Rennorb) @find capacity dependencies

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
        var enemies = log.MechanicData.GetEnemyList(log, log.LogData.LogStart, log.LogData.LogEnd);
        logData.Enemies = new(enemies.Count);
        foreach (SingleActor enemy in enemies)
        {
            logData.Enemies.Add(new EnemyDto() { Name = enemy.Character, FirstAware = enemy.FirstAware / 1000.0, LastAware = enemy.LastAware / 1000.0  });
        }
        _t.Log("built enemy data");

        log.UpdateProgressWithCancellationCheck("HTML: building Targets");
        logData.Targets = new(log.LogData.Logic.Targets.Count);
        foreach (SingleActor target in log.LogData.Logic.Targets)
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
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
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
        MechanicDto.BuildMechanics(log.MechanicData.GetPresentMechanics(log, log.LogData.LogStart, log.LogData.LogEnd), logData.MechanicMap);

        Trace.TrackAverageStat("usedBuffs", usedBuffs.Count);
        Trace.TrackAverageStat("usedDamageMods", usedDamageMods.Count);
        Trace.TrackAverageStat("usedIncDamageMods", usedIncDamageMods.Count);
        Trace.TrackAverageStat("usedSkills", usedSkills.Count);

        return logData;
    }

}
