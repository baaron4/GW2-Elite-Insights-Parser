using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIBuilders.JsonModels.JsonActors;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using Newtonsoft.Json;
using static GW2EIJSON.JsonLog;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// The root of the JSON
    /// </summary>
    internal static class JsonLogBuilder
    {
        internal static SkillDesc BuildSkillDesc(SkillItem item, ParsedEvtcLog log)
        {
            var skillDesc = new SkillDesc
            {
                Name = item.Name,
                AutoAttack = item.AA,
                Icon = item.Icon,
                CanCrit = SkillItem.CanCrit(item.ID, log.LogData.GW2Build),
                IsSwap = item.IsSwap,
                IsNotAccurate = log.SkillData.IsNotAccurate(item.ID),
                ConversionBasedHealing = log.CombatData.HasEXTHealing && log.CombatData.EXTHealingCombatData.GetHealingType(item, log) == HealingStatsExtensionHandler.EXTHealingType.ConversionBased,
                HybridHealing = log.CombatData.HasEXTHealing && log.CombatData.EXTHealingCombatData.GetHealingType(item, log) == HealingStatsExtensionHandler.EXTHealingType.Hybrid
            };
            return skillDesc;
        }

        internal static BuffDesc BuildBuffDesc(Buff item, ParsedEvtcLog log)
        {
            var buffDesc = new BuffDesc
            {
                Name = item.Name,
                Icon = item.Link,
                Stacking = item.Type == Buff.BuffType.Intensity,
                ConversionBasedHealing = log.CombatData.HasEXTHealing ? log.CombatData.EXTHealingCombatData.GetHealingType(item, log) == GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler.EXTHealingType.ConversionBased : false,
                HybridHealing = log.CombatData.HasEXTHealing ? log.CombatData.EXTHealingCombatData.GetHealingType(item, log) == GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler.EXTHealingType.Hybrid : false
            };
            BuffInfoEvent buffInfoEvent = log.CombatData.GetBuffInfoEvent(item.ID);
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
                    string desc = formula.GetDescription(false, log.Buffs.BuffsByIds);
                    if (desc.Length > 0)
                    {
                        descriptions.Add(desc);
                    }
                }
                buffDesc.Descriptions = descriptions;
            }
            return buffDesc;
        }

        internal static DamageModDesc BuildDamageModDesc(DamageModifier item)
        {
            var damageModDesc = new DamageModDesc
            {
                Name = item.Name,
                Icon = item.Icon,
                Description = item.Tooltip,
                NonMultiplier = !item.Multiplier,
                SkillBased = item.SkillBased,
                Approximate = item.Approximate
            };
            return damageModDesc;
        }

        public static JsonLog BuildJsonLog(ParsedEvtcLog log, RawFormatSettings settings, Version parserVersion, string[] uploadLinks)
        {
            var jsonLog = new JsonLog();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Meta Data");
            jsonLog.TriggerID = log.FightData.TriggerID;
            jsonLog.EIEncounterID = log.FightData.Logic.EncounterID;
            jsonLog.FightName = log.FightData.FightName;
            jsonLog.FightIcon = log.FightData.Logic.Icon;
            jsonLog.EliteInsightsVersion = parserVersion.ToString();
            jsonLog.ArcVersion = log.LogData.ArcVersion;
            jsonLog.RecordedBy = log.LogData.PoVName;
            jsonLog.TimeStart = log.LogData.LogStart;
            jsonLog.TimeEnd = log.LogData.LogEnd;
            jsonLog.TimeStartStd = log.LogData.LogStartStd;
            jsonLog.TimeEndStd = log.LogData.LogEndStd;
            jsonLog.Duration = log.FightData.DurationString;
            jsonLog.Success = log.FightData.Success;
            jsonLog.GW2Build = log.LogData.GW2Build;
            jsonLog.UploadLinks = uploadLinks;
            jsonLog.Language = log.LogData.Language;
            jsonLog.LanguageID = (byte)log.LogData.LanguageID;
            jsonLog.IsCM = log.FightData.IsCM;
            var personalBuffs = new Dictionary<string, HashSet<long>>();
            var skillMap = new Dictionary<string, SkillDesc>();
            var buffMap = new Dictionary<string, BuffDesc>();
            var damageModMap = new Dictionary<string, DamageModDesc>();

            if (log.FightData.Logic.GetInstanceBuffs(log).Any())
            {
                var presentFractalInstabilities = new List<long>();
                var presentInstanceBuffs = new List<long[]>();
                foreach ((Buff instanceBuff, int stack) in log.FightData.Logic.GetInstanceBuffs(log))
                {
                    if (!buffMap.ContainsKey("b" + instanceBuff.ID))
                    {
                        buffMap["b" + instanceBuff.ID] = BuildBuffDesc(instanceBuff, log);
                    }
                    if (instanceBuff.Source == ParserHelper.Source.FractalInstability)
                    {
                        presentFractalInstabilities.Add(instanceBuff.ID);
                    }
                    presentInstanceBuffs.Add(new long[] { instanceBuff.ID, stack });
                }
                jsonLog.PresentFractalInstabilities = presentFractalInstabilities;
                jsonLog.PresentInstanceBuffs = presentInstanceBuffs;
            }
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Mechanics");
            MechanicData mechanicData = log.MechanicData;
            var mechanicLogs = new List<MechanicEvent>();
            foreach (List<MechanicEvent> mLog in mechanicData.GetAllMechanicEvents(log))
            {
                mechanicLogs.AddRange(mLog);
            }
            if (mechanicLogs.Any())
            {
                jsonLog.Mechanics = JsonMechanicsBuilder.GetJsonMechanicsList(mechanicLogs);
            }
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Phases");
            jsonLog.Phases = log.FightData.GetNonDummyPhases(log).Select(x => JsonPhaseBuilder.BuildJsonPhase(x, log)).ToList();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Targets");
            jsonLog.Targets = log.FightData.Logic.Targets.Select(x => JsonNPCBuilder.BuildJsonNPC(x, log, settings, skillMap, buffMap)).ToList();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Players");
            jsonLog.Players = log.Friendlies.Select(x => JsonPlayerBuilder.BuildJsonPlayer(x, log, settings, skillMap, buffMap, damageModMap, personalBuffs)).ToList();
            //
            if (log.LogData.LogErrors.Any())
            {
                jsonLog.LogErrors = new List<string>(log.LogData.LogErrors);
            }
            if (log.LogData.UsedExtensions.Any())
            {
                var usedExtensions = new List<ExtensionDesc>();
                foreach (AbstractExtensionHandler extension in log.LogData.UsedExtensions)
                {
                    var set = new HashSet<string>();
                    if (log.LogData.PoV != null)
                    {
                        set.Add(log.FindActor(log.LogData.PoV).Character);
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
            jsonLog.PersonalBuffs = personalBuffs.ToDictionary(x => x.Key, x => (IReadOnlyCollection<long>) x.Value);
            jsonLog.SkillMap = skillMap;
            jsonLog.BuffMap = buffMap;
            jsonLog.DamageModMap = damageModMap;
            //
            if (log.CanCombatReplay)
            {
                jsonLog.CombatReplayMetaData = JsonCombatReplayMetaDataBuilder.BuildJsonCombatReplayMetaData(log, settings);
            }
            return jsonLog;
        }

    }
}
