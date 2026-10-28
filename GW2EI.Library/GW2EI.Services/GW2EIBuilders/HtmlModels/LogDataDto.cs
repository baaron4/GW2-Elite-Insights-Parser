using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using GW2EIBuilders.HtmlModels.EXTBarrier;
using GW2EIBuilders.HtmlModels.EXTHealing;
using GW2EIBuilders.HtmlModels.HTMLActors;
using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLMetaData;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.LogLogic;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using Tracing;
using static GW2EIEvtcParser.ParserHelper;

[assembly: InternalsVisibleTo("GW2EIParser.tst")]
namespace GW2EIBuilders.HtmlModels;

using BuffInstanceItem = long[];

//TODO_PERF(Rennorb)
internal class LogDataDto
{
    public List<TargetDto>? Targets;
    public List<PlayerDto>? Players;
    public List<EnemyDto>? MechanicEnemies;
    public List<PhaseDto>? Phases;
    // Present buffs and damamge modifiers
    public List<long>? Boons;
    public List<long>? OffBuffs;
    public List<long>? SupBuffs;
    public List<long>? DefBuffs;
    public List<long>? Debuffs;
    public List<long>? TargetDebuffs;
    public List<long>? TargetOtherBuffs;
    public List<long>? GearBuffs;
    public List<long>? Nourishments;
    public List<long>? Enhancements;
    public List<long>? OtherConsumables;
    public List<BuffInstanceItem>? InstanceBuffs;
    public List<long> DmgModifiersItem = new(20);
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
    public Dictionary<string, string> NpcMap = new();
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
    public string? Region;
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
        var mapIDEvent = log.CombatData.GetMapIDEvent();
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
        var region = log.LogMetadata.Region;
        if (region != ArcDPSEnums.RegionEnum.Unknown)
        {
            Region = ArcDPSEnums.RegionToString(region);
        }
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

    private static Dictionary<Spec, IReadOnlyList<Buff>> BuildPersonalBuffData(ParsedEvtcLog log, Dictionary<string, List<long>> persBuffDict, UsedData used)
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
                used.buffs[boon.ID] = boon;
            }
        }
        return boonsBySpec;
    }

    private static Dictionary<Spec, IReadOnlyList<OutgoingDamageModifier>> BuildPersonalOutgoingDamageModData(ParsedEvtcLog log, Dictionary<string, List<long>> dgmModDict, UsedData used)
    {
        var damageModBySpecs = new Dictionary<Spec, IReadOnlyList<OutgoingDamageModifier>>(log.FriendliesListBySpec.Count);
        // Collect all personal damage mods by spec
        foreach (var pair in log.FriendliesListBySpec)
        {
            var specDamageModsID = new HashSet<int>(log.DamageModifiers.GetPersonalOutgoingModifiersPerSpec(pair.Key).Select(x => x.ID));
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
                used.damageMods.Add(mod);
            }
        }
        return damageModBySpecs;
    }

    private static Dictionary<Spec, IReadOnlyList<IncomingDamageModifier>> BuildPersonalIncomingDamageModData(ParsedEvtcLog log, Dictionary<string, List<long>> dgmModDict, UsedData used)
    {
        var damageModBySpecs = new Dictionary<Spec, IReadOnlyList<IncomingDamageModifier>>(log.FriendliesListBySpec.Count);
        // Collect all personal damage mods by spec
        foreach (var pair in log.FriendliesListBySpec)
        {
            var specDamageModsID = new HashSet<int>(log.DamageModifiers.GetPersonalIncomingModifiersPerSpec(pair.Key).Select(x => x.ID));
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
            dgmModDict[pair.Key.ToString()] = new(pair.Value.Count);
            foreach (IncomingDamageModifier mod in pair.Value)
            {
                dgmModDict[pair.Key.ToString()].Add(mod.ID);
                used.incDamageMods.Add(mod);
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
    [MemberNotNull(nameof(TargetDebuffs))]
    [MemberNotNull(nameof(GearBuffs))]
    [MemberNotNull(nameof(Nourishments))]
    [MemberNotNull(nameof(Enhancements))]
    [MemberNotNull(nameof(OtherConsumables))]
    [MemberNotNull(nameof(InstanceBuffs))]
    private void BuildBuffDictionaries(ParsedEvtcLog log, UsedData used)
    {
        StatisticsHelper statistics = log.StatisticsHelper;
        Boons = new(statistics.PresentBoons.Count);
        foreach (Buff boon in statistics.PresentBoons)
        {
            Boons.Add(boon.ID);
            used.buffs[boon.ID] = boon;
        }
        Conditions = new(statistics.PresentConditions.Count);
        foreach (Buff condition in statistics.PresentConditions)
        {
            Conditions.Add(condition.ID);
            used.buffs[condition.ID] = condition;
        }
        OffBuffs = new(statistics.PresentConditions.Count);
        foreach (Buff offBuff in statistics.PresentOffbuffs)
        {
            OffBuffs.Add(offBuff.ID);
            used.buffs[offBuff.ID] = offBuff;
        }
        SupBuffs = new(statistics.PresentSupbuffs.Count);
        foreach (Buff supBuff in statistics.PresentSupbuffs)
        {
            SupBuffs.Add(supBuff.ID);
            used.buffs[supBuff.ID] = supBuff;
        }
        DefBuffs = new(statistics.PresentDefbuffs.Count);
        foreach (Buff defBuff in statistics.PresentDefbuffs)
        {
            DefBuffs.Add(defBuff.ID);
            used.buffs[defBuff.ID] = defBuff;
        }
        Debuffs = new(statistics.PresentDebuffs.Count);
        foreach (Buff debuff in statistics.PresentDebuffs)
        {
            Debuffs.Add(debuff.ID);
            used.buffs[debuff.ID] = debuff;
        }
        TargetDebuffs = new(statistics.PresentTargetDebuffs.Count);
        foreach (Buff debuff in statistics.PresentTargetDebuffs)
        {
            TargetDebuffs.Add(debuff.ID);
            used.buffs[debuff.ID] = debuff;
        }
        TargetOtherBuffs = new(statistics.PresentTargetOtherBuffs.Count);
        foreach (Buff buff in statistics.PresentTargetOtherBuffs)
        {
            TargetOtherBuffs.Add(buff.ID);
            used.buffs[buff.ID] = buff;
        }
        GearBuffs = new(statistics.PresentGearbuffs.Count);
        foreach (Buff gearBuff in statistics.PresentGearbuffs)
        {
            GearBuffs.Add(gearBuff.ID);
            used.buffs[gearBuff.ID] = gearBuff;
        }
        Nourishments = new(statistics.PresentNourishements.Count);
        foreach (Buff nourishment in statistics.PresentNourishements)
        {
            Nourishments.Add(nourishment.ID);
            used.buffs[nourishment.ID] = nourishment;
        }
        Enhancements = new(statistics.PresentEnhancements.Count);
        foreach (Buff enhancement in statistics.PresentEnhancements)
        {
            Enhancements.Add(enhancement.ID);
            used.buffs[enhancement.ID] = enhancement;
        }
        OtherConsumables = new(statistics.PresentOtherConsumables.Count);
        foreach (Buff otherConsumables in statistics.PresentOtherConsumables)
        {
            OtherConsumables.Add(otherConsumables.ID);
            used.buffs[otherConsumables.ID] = otherConsumables;
        }
        var instanceBuffs = log.LogData.Logic.GetInstanceBuffs(log);
        InstanceBuffs = new(instanceBuffs.Count);
        foreach (var instanceBuff in instanceBuffs)
        {
            InstanceBuffs.Add([instanceBuff.Buff.ID, instanceBuff.Stack, instanceBuff.AttachedPhase.Start, instanceBuff.AttachedPhase.End, instanceBuff.RemainingDuration]);
            used.buffs[instanceBuff.Buff.ID] = instanceBuff.Buff;
        }
    }

    private void BuildOutgoingDamageModDictionaries(ParsedEvtcLog log, UsedData used,
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
                    used.damageMods.Add(dMod);
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
                    used.damageMods.Add(dMod);
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
                    used.damageMods.Add(dMod);
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
                    used.damageMods.Add(dMod);
                }
            }
        }
    }

    private void BuildIncomingDamageModDictionaries(ParsedEvtcLog log, UsedData used,
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
                    used.incDamageMods.Add(dMod);
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
                    used.incDamageMods.Add(dMod);
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
                    used.incDamageMods.Add(dMod);
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
                    used.incDamageMods.Add(dMod);
                }
            }
        }
    }

    internal class UsedData
    {
        //TODO_PERF(Rennorb) @find capacity dependencies
        public readonly Dictionary<long, Buff> buffs = new(128);
        public readonly HashSet<OutgoingDamageModifier> damageMods = new(32);
        public readonly HashSet<IncomingDamageModifier> incDamageMods = new(16);
        public readonly Dictionary<long, SkillItem> skills = new(256);
        public readonly Dictionary<string, AgentItem> npcs = new(16);

        private readonly Dictionary<(long Skill, double TimeScale, bool Water), (int? LastCastBarExecute, int? ControlReturn)> skillDurations = new(32);

        public (int? LastCastBarExecute, int? ControlReturn) GetComputedSkillDuration(ParsedEvtcLog log, long skillID, double timeScale, bool water)
        {
            if (log.LogMetadata.EvtcBuild < ArcDPSEnums.ArcDPSBuilds.SkillInfoRework)
            {
                return (null, null);
            }
            timeScale = TimeScale.NormalizeCommonScales(timeScale);
            var key = (skillID, timeScale, water);
            if (!skillDurations.TryGetValue(key, out var duration))
            {
                var animation = log.SkillData.Get(skillID).SkillInfo?.GetAnimation(water);
                var scaling = TimeScale.GetTickScaling(timeScale);
                duration.LastCastBarExecute = animation?.ComputeCastBarExecute(true, scaling);
                duration.ControlReturn = animation?.ComputeControlReturn(scaling);
                skillDurations.Add(key, duration);
            }
            return duration;
        }
    };

    public static LogDataDto BuildLogData(ParsedEvtcLog log, bool cr, bool light, Version parserVersion, UploadResults uploadLinks)
    {
        using var _t = new AutoTrace("BuildLogData");

        var used = new UsedData();

        log.UpdateProgressWithCancellationCheck("HTML: building Log Data");
        var logData = new LogDataDto(log, light, parserVersion, uploadLinks);
        if (cr)
        {
            log.UpdateProgressWithCancellationCheck("HTML: building Combat Replay");
            logData.CrData = new CombatReplayDto(log, used);
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
            logData.Players.Add(new PlayerDto(actor, log, ActorDetailsDto.BuildPlayerData(log, actor, used)));
        }
        _t.Log("built player data");

        log.UpdateProgressWithCancellationCheck("HTML: building Enemies");
        var mechanicEnemies = log.MechanicData.GetEnemyList(log, log.LogData.LogStart, log.LogData.LogEnd);
        logData.MechanicEnemies = new(mechanicEnemies.Count);
        foreach (SingleActor enemy in mechanicEnemies)
        {
            logData.MechanicEnemies.Add(new EnemyDto() { Name = enemy.Character, FirstAware = enemy.FirstAware / 1000.0, LastAware = enemy.LastAware / 1000.0 });
        }
        foreach (SingleActor enemy in log.LogData.Logic.TrashMobs)
        {
            used.npcs.TryAdd("u" + enemy.AgentItem.UniqueID, enemy.AgentItem);
        }
        var tst = log.AgentData.GetAgentByUniqueID(166);
        _t.Log("built enemy data");

        log.UpdateProgressWithCancellationCheck("HTML: building Targets");
        logData.Targets = new(log.LogData.Logic.Targets.Count);
        foreach (SingleActor target in log.LogData.Logic.Targets)
        {
            var targetDto = new TargetDto(target, log, ActorDetailsDto.BuildTargetData(log, target, used, cr));
            logData.Targets.Add(targetDto);
        }
        _t.Log("built target data");

        log.UpdateProgressWithCancellationCheck("HTML: building Skill/Buff/Damage Modifier dictionaries");
        Dictionary<Spec, IReadOnlyList<Buff>> persBuffDict = BuildPersonalBuffData(log, logData.PersBuffs, used);
        Dictionary<Spec, IReadOnlyList<OutgoingDamageModifier>> persOutDamageModDict = BuildPersonalOutgoingDamageModData(log, logData.DmgModifiersPers, used);
        Dictionary<Spec, IReadOnlyList<IncomingDamageModifier>> persIncDamageModDict = BuildPersonalIncomingDamageModData(log, logData.DmgIncModifiersPers, used);

        var allOutDamageMods = new HashSet<int>(60);
        var commonOutDamageModifiers = new List<OutgoingDamageModifier>(20);
        var itemOutDamageModifiers = new List<OutgoingDamageModifier>(20);
        var allIncDamageMods = new HashSet<int>(60);
        var commonIncDamageModifiers = new List<IncomingDamageModifier>(20);
        var itemIncDamageModifiers = new List<IncomingDamageModifier>(20);

        logData.BuildBuffDictionaries(log, used);
        logData.BuildOutgoingDamageModDictionaries(log, used, allOutDamageMods, commonOutDamageModifiers, itemOutDamageModifiers);
        logData.BuildIncomingDamageModDictionaries(log, used, allIncDamageMods, commonIncDamageModifiers, itemIncDamageModifiers);
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
            logData.HealingStatsExtension = new HealingStatsExtension(log, used);
            if (log.CombatData.HasEXTBarrier)
            {
                log.UpdateProgressWithCancellationCheck("HTML: building Barrier Extension");
                logData.BarrierStatsExtension = new BarrierStatsExtension(log, used);
            }
        }
        _t.Log("built healing data");

        SkillDto.AssembleSkills(used.skills.Values, logData.SkillMap, log);
        DamageModDto.AssembleDamageModifiers(used.damageMods, logData.DamageModMap);
        DamageModDto.AssembleDamageModifiers(used.incDamageMods, logData.DamageIncModMap);
        BuffDto.AssembleBuffs(used.buffs.Values, logData.BuffMap, log);
        MechanicDto.BuildMechanics(log.MechanicData.GetPresentMechanics(log, log.LogData.LogStart, log.LogData.LogEnd), logData.MechanicMap);
        ActorDto.AssembleNPCs(used.npcs, logData.NpcMap, log);

        Trace.TrackAverageStat("usedBuffs", used.buffs.Count);
        Trace.TrackAverageStat("usedDamageMods", used.damageMods.Count);
        Trace.TrackAverageStat("usedIncDamageMods", used.incDamageMods.Count);
        Trace.TrackAverageStat("usedSkills", used.skills.Count);
        Trace.TrackAverageStat("usedNpcs", used.npcs.Count);

        return logData;
    }

}
