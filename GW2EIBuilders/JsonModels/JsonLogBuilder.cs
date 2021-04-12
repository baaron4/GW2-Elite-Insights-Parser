using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
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
        internal static SkillDesc BuildSkillDesc(SkillItem item, ulong gw2Build, SkillData skillData)
        {
            var skillDesc = new SkillDesc();
            skillDesc.Name = item.Name;
            skillDesc.AutoAttack = item.AA;
            skillDesc.Icon = item.Icon;
            skillDesc.CanCrit = SkillItem.CanCrit(item.ID, gw2Build);
            skillDesc.IsSwap = item.IsSwap;
            skillDesc.IsNotAccurate = skillData.IsNotAccurate(item.ID);
            return skillDesc;
        }

        internal static BuffDesc BuildBuffDesc(Buff item, ParsedEvtcLog log)
        {
            var buffDesc = new BuffDesc();
            buffDesc.Name = item.Name;
            buffDesc.Icon = item.Link;
            buffDesc.Stacking = item.Type == Buff.BuffType.Intensity;
            BuffInfoEvent buffInfoEvent = log.CombatData.GetBuffInfoEvent(item.ID);
            if (buffInfoEvent != null)
            {
                var descriptions = new List<string>(){
                        "Max Stack(s) " + item.Capacity
                    };
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
            var damageModDesc = new DamageModDesc();
            damageModDesc.Name = item.Name;
            damageModDesc.Icon = item.Icon;
            damageModDesc.Description = item.Tooltip;
            damageModDesc.NonMultiplier = !item.Multiplier;
            damageModDesc.SkillBased = item.SkillBased;
            return damageModDesc;
        }

        public static JsonLog BuildJsonLog(ParsedEvtcLog log, RawFormatSettings settings, Version parserVersion, string[] uploadLinks)
        {
            var jsonLog = new JsonLog();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Meta Data");
            jsonLog.TriggerID = log.FightData.TriggerID;
            jsonLog.FightName = log.FightData.GetFightName(log);
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

            if (log.StatisticsHelper.PresentFractalInstabilities.Any())
            {
                var presentFractalInstabilities = new List<long>();
                foreach (Buff fractalInstab in log.StatisticsHelper.PresentFractalInstabilities)
                {
                    presentFractalInstabilities.Add(fractalInstab.ID);
                    if (!buffMap.ContainsKey("b" + fractalInstab.ID))
                    {
                        buffMap["b" + fractalInstab.ID] = BuildBuffDesc(fractalInstab, log);
                    }
                }
                jsonLog.PresentFractalInstabilities = presentFractalInstabilities;
            }
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Mechanics");
            MechanicData mechanicData = log.MechanicData;
            var mechanicLogs = new List<MechanicEvent>();
            foreach (List<MechanicEvent> mLog in mechanicData.GetAllMechanics(log))
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
            jsonLog.Players = log.PlayerList.Select(x => JsonPlayerBuilder.BuildJsonPlayer(x, log, settings, skillMap, buffMap, damageModMap, personalBuffs)).ToList();
            //
            if (log.LogData.LogErrors.Count > 0)
            {
                jsonLog.LogErrors = new List<string>(log.LogData.LogErrors);
            }
            //
            jsonLog.PersonalBuffs = personalBuffs.ToDictionary(x => x.Key, x => (IReadOnlyCollection<long>) x.Value);
            jsonLog.SkillMap = skillMap;
            jsonLog.BuffMap = buffMap;
            jsonLog.DamageModMap = damageModMap;
            //
            return jsonLog;
        }

    }
}
