using GW2EIBuilders.JsonModels.JsonActors;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.LogLogic;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIJSON.JsonLog;

namespace GW2EIBuilders.JsonModels;

internal static class JsonLogBuilder
{
    private static SkillDesc BuildSkillDesc(SkillItem skill, ParsedEvtcLog log)
    {
        var skillDesc = new SkillDesc
        {
            Name = skill.Name,
            AutoAttack = skill.IsAutoAttack(log),
            Icon = skill.Icon,
            CanCrit = SkillItem.CanCrit(skill.ID, log.LogMetadata.GW2Build),
            IsSwap = skill.IsSwap,
            IsInstantCast = log.CombatData.GetInstantCastData(skill.ID).Any(),
            IsNotAccurate = log.SkillData.IsNotAccurate(skill.ID),
            IsGearProc = log.SkillData.IsGearProc(skill.ID),
            IsTraitProc = log.SkillData.IsTraitProc(skill.ID),
            IsUnconditionalProc = log.SkillData.IsUnconditionalProc(skill.ID),
            ConversionBasedHealing = log.CombatData.HasEXTHealing && log.CombatData.EXTHealingCombatData.GetHealingType(skill, log) == HealingStatsExtensionHandler.EXTHealingType.ConversionBased,
            HybridHealing = log.CombatData.HasEXTHealing && log.CombatData.EXTHealingCombatData.GetHealingType(skill, log) == HealingStatsExtensionHandler.EXTHealingType.Hybrid
        };
        return skillDesc;
    }

    private static BuffDesc BuildBuffDesc(Buff buff, ParsedEvtcLog log)
    {
        var buffDesc = new BuffDesc
        {
            Name = buff.Name,
            Icon = buff.Link,
            Stacking = buff.Type == Buff.BuffType.Intensity,
            ConversionBasedHealing = log.CombatData.HasEXTHealing ? log.CombatData.EXTHealingCombatData.GetHealingType(buff, log) == HealingStatsExtensionHandler.EXTHealingType.ConversionBased : false,
            HybridHealing = log.CombatData.HasEXTHealing ? log.CombatData.EXTHealingCombatData.GetHealingType(buff, log) == HealingStatsExtensionHandler.EXTHealingType.Hybrid : false
        };
        BuffInfoEvent? buffInfoEvent = log.CombatData.GetBuffInfoEvent(buff.ID);
        if (buffInfoEvent != null)
        {
            var descriptions = new List<string>(){
                    "Max Stack(s) " + buffInfoEvent.MaxStacks
                };
            if (buffInfoEvent.DurationCap > 0)
            {
                descriptions.Add("Duration Cap: " + Math.Round(buffInfoEvent.DurationCap / 1000.0, 3) + " seconds");
            }
            foreach (BuffFormula formula in buffInfoEvent.Formulas)
            {
                if (formula.IsConditional)
                {
                    continue;
                }
                string desc = formula.GetDescription(false, log.Buffs.BuffsByIDs, buff);
                if (desc.Length > 0)
                {
                    descriptions.Add(desc);
                }
            }
            buffDesc.Descriptions = descriptions;
        }
        return buffDesc;
    }

    private static DamageModDesc BuildDamageModDesc(DamageModifier damageModifier)
    {
        var damageModDesc = new DamageModDesc
        {
            Name = damageModifier.Name,
            Icon = damageModifier.Icon,
            Description = damageModifier.Tooltip,
            NonMultiplier = !damageModifier.Multiplier,
            SkillBased = damageModifier.SkillBased,
            Approximate = damageModifier.Approximate,
            Incoming = damageModifier.Incoming,
        };
        return damageModDesc;
    }

    public static JsonLog BuildJsonLog(ParsedEvtcLog log, RawFormatSettings settings, Version parserVersion, string[] uploadLinks)
    {
        var jsonLog = new JsonLog();
        //
        log.UpdateProgressWithCancellationCheck("Raw Format: Building Meta Data");
        jsonLog.TriggerID = log.LogData.TriggerID;
        jsonLog.IsInstanceLog = log.LogData.IsInstance;
        jsonLog.EIEncounterID = log.LogData.Logic.LogID;
        jsonLog.EILogID = log.LogData.Logic.LogID;
        var mapIDEvent = log.CombatData.GetMapIDEvents().FirstOrDefault();
        if (mapIDEvent != null)
        {
            jsonLog.MapID = mapIDEvent.MapID;
        }
        jsonLog.FightName = log.LogData.LogName;
        jsonLog.FightIcon = log.LogData.Logic.Icon;
        jsonLog.Name = log.LogData.LogName;
        jsonLog.Icon = log.LogData.Logic.Icon;
        jsonLog.EliteInsightsVersion = parserVersion.ToString();
        jsonLog.ArcVersion = log.LogMetadata.ArcVersionBuild;
        jsonLog.ArcRevision = log.LogMetadata.EvtcRevision;
        jsonLog.RecordedBy = log.LogMetadata.PoVName;
        jsonLog.RecordedAccountBy = log.LogMetadata.PoVAccount;
        jsonLog.TimeStart = log.LogMetadata.DateStart;
        jsonLog.TimeEnd = log.LogMetadata.DateEnd;
        jsonLog.TimeStartStd = log.LogMetadata.DateStartStd;
        jsonLog.TimeEndStd = log.LogMetadata.DateEndStd;
        jsonLog.Duration = log.LogData.DurationString;
        jsonLog.DurationMS = log.LogData.LogDuration;
        jsonLog.LogStartOffset = log.LogData.LogStartOffset;
        if (log.LogMetadata.DateInstanceStartStd != null )
        {
            jsonLog.InstanceTimeStartStd = log.LogMetadata.DateInstanceStartStd;
            jsonLog.InstanceIP = log.LogMetadata.LogInstanceIP;
        } 
        else
        {
            jsonLog.InstanceTimeStartStd = null;
            jsonLog.InstanceIP = null;
        }
        switch (log.LogData.InstancePrivacy)
        {
            case LogData.InstancePrivacyMode.Public:
                jsonLog.InstancePrivacy = "Public Instance";
                break;
            case LogData.InstancePrivacyMode.Private:
                jsonLog.InstancePrivacy = "Private Instance";
                break;
            case LogData.InstancePrivacyMode.NotApplicable:
                jsonLog.InstancePrivacy = "Not Applicable";
                break;
            case LogData.InstancePrivacyMode.Unknown:
                jsonLog.InstancePrivacy = "Unknown";
                break;
        }
        jsonLog.Success = log.LogData.Success;
        jsonLog.GW2Build = log.LogMetadata.GW2Build;
        jsonLog.UploadLinks = uploadLinks;
        jsonLog.Language = log.LogMetadata.Language;
        jsonLog.LanguageID = (byte)log.LogMetadata.LanguageID;
        jsonLog.FractalScale = log.CombatData.GetFractalScaleEvent() != null ? log.CombatData.GetFractalScaleEvent()!.Scale : 0;
        jsonLog.IsCM = log.LogData.IsCM || log.LogData.IsLegendaryCM;
        jsonLog.IsLegendaryCM = log.LogData.IsLegendaryCM;
        jsonLog.IsLateStart = log.LogData.IsLateStart;
        jsonLog.MissingPreEvent = log.LogData.MissingPreEvent;
        jsonLog.Anonymous = log.ParserSettings.AnonymousPlayers;
        jsonLog.DetailedWvW = log.ParserSettings.DetailedWvWParse && log.LogData.Logic.ParseMode == LogLogic.ParseModeEnum.WvW;
        var personalBuffs = new Dictionary<string, HashSet<long>>(20); //TODO(Rennorb) @perf
        var personalDamageMods = new Dictionary<string, HashSet<long>>(20); //TODO(Rennorb) @perf
        var skillMap = new Dictionary<long, SkillItem>(200); //TODO(Rennorb) @perf
        var skillDescs = new Dictionary<string, SkillDesc>(200); //TODO(Rennorb) @perf
        var buffMap = new Dictionary<long, Buff>(100); //TODO(Rennorb) @perf
        var buffDescs = new Dictionary<string, BuffDesc>(100); //TODO(Rennorb) @perf
        var damageModMap = new Dictionary<int, DamageModifier>(50); //TODO(Rennorb) @perf
        var damageModDesc = new Dictionary<string, DamageModDesc>(50); //TODO(Rennorb) @perf

        var instanceBuffs = log.LogData.Logic.GetInstanceBuffs(log);
        if (instanceBuffs.Any())
        {
            var presentFractalInstabilities = new List<long>(3); //TODO(Rennorb) @perf
            var presentInstanceBuffs = new List<IReadOnlyList<long>>(instanceBuffs.Count);
            foreach ((Buff instanceBuff, int stack) in instanceBuffs)
            {
                if (!buffMap.ContainsKey(instanceBuff.ID))
                {
                    buffMap[instanceBuff.ID] = instanceBuff;
                }
                if (instanceBuff.Source == ParserHelper.Source.FractalInstability)
                {
                    presentFractalInstabilities.Add(instanceBuff.ID);
                }
                presentInstanceBuffs.Add([instanceBuff.ID, stack]);
            }
            jsonLog.PresentFractalInstabilities = presentFractalInstabilities;
            jsonLog.PresentInstanceBuffs = presentInstanceBuffs;
        }
        //
        log.UpdateProgressWithCancellationCheck("Raw Format: Building Mechanics");
        MechanicData mechanicData = log.MechanicData;
        IReadOnlyCollection<Mechanic> presentMechanics = log.MechanicData.GetPresentMechanics(log, log.LogData.LogStart, log.LogData.LogEnd);
        if (presentMechanics.Count != 0)
        {
            jsonLog.Mechanics = JsonMechanicsBuilder.GetJsonMechanicsList(log, mechanicData, presentMechanics);
        }
        //
        log.UpdateProgressWithCancellationCheck("Raw Format: Building Phases");
        jsonLog.Phases = log.LogData.GetPhases(log).Select(x => JsonPhaseBuilder.BuildJsonPhase(x, log)).ToList();
        //
        log.UpdateProgressWithCancellationCheck("Raw Format: Building Targets");
        jsonLog.Targets = log.LogData.Logic.Targets.Select(x => JsonNPCBuilder.BuildJsonNPC(x, log, settings, skillMap, buffMap)).ToList();
        //
        log.UpdateProgressWithCancellationCheck("Raw Format: Building Players");
        jsonLog.Players = log.Friendlies.Select(x => JsonPlayerBuilder.BuildJsonPlayer(x, log, settings, skillMap, buffMap, damageModMap, personalBuffs, personalDamageMods)).ToList();
        //
        if (log.LogMetadata.LogErrors.Any())
        {
            jsonLog.LogErrors = new List<string>(log.LogMetadata.LogErrors);
        }
        if (log.LogMetadata.UsedExtensions.Any())
        {
            var usedExtensions = new List<ExtensionDesc>();
            foreach (ExtensionHandler extension in log.LogMetadata.UsedExtensions)
            {
                var set = new HashSet<string>();
                if (log.LogMetadata.PoV != null)
                {
                    set.Add(log.FindActor(log.LogMetadata.PoV).Character);
                    foreach (AgentItem agent in extension.RunningExtension)
                    {
                        set.Add(log.FindActor(agent).Character);
                    }
                }
                usedExtensions.Add(new ExtensionDesc()
                {
                    Name = extension.Name,
                    Version = extension.Version,
                    Signature = extension.Signature,
                    Revision = extension.Revision,
                    RunningExtension = set.ToList()
                });
            }
            jsonLog.UsedExtensions = usedExtensions;
        }
        //
        jsonLog.PersonalBuffs = personalBuffs.ToDictionary(x => x.Key, x => (IReadOnlyCollection<long>)x.Value);
        jsonLog.PersonalDamageMods = personalDamageMods.ToDictionary(x => x.Key, x => (IReadOnlyCollection<long>)x.Value);
        foreach (KeyValuePair<long, SkillItem> pair in skillMap)
        {
            skillDescs["s" + pair.Key] = BuildSkillDesc(pair.Value, log);
        }
        jsonLog.SkillMap = skillDescs;
        foreach (KeyValuePair<long, Buff> pair in buffMap)
        {
            buffDescs["b" + pair.Key] = BuildBuffDesc(pair.Value, log);
        }
        jsonLog.BuffMap = buffDescs;
        foreach (KeyValuePair<int, DamageModifier> pair in damageModMap)
        {
            damageModDesc["d" + pair.Key] = BuildDamageModDesc(pair.Value);
        }
        jsonLog.DamageModMap = damageModDesc;
        //
        if (log.CanCombatReplay)
        {
            jsonLog.CombatReplayMetaData = JsonCombatReplayMetaDataBuilder.BuildJsonCombatReplayMetaData(log, settings);
        }
        return jsonLog;
    }

}
