using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using Newtonsoft.Json;
using static GW2EIBuilders.JsonModels.JsonMechanics;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// The root of the JSON
    /// </summary>
    public class JsonLog
    {
        /// <summary>
        /// Describes the skill item
        /// </summary>
        public class SkillDesc
        {
            [JsonConstructor]
            internal SkillDesc()
            {

            }
            internal SkillDesc(SkillItem item, ulong gw2Build)
            {
                Name = item.Name;
                AutoAttack = item.AA;
                Icon = item.Icon;
                CanCrit = SkillItem.CanCrit(item.ID, gw2Build);
                IsSwap = item.IsSwap;
            }

            [JsonProperty]
            /// <summary>
            /// Name of the skill
            /// </summary>
            public string Name { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// If the skill is an auto attack
            /// </summary>
            public bool AutoAttack { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// If the skill can crit
            /// </summary>
            public bool CanCrit { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Icon of the skill
            /// </summary>
            public string Icon { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// True if this skill can trigger on weapon swap sigils
            /// </summary>
            public bool IsSwap { get; internal set; }
        }

        /// <summary>
        /// Describs the buff item
        /// </summary>
        public class BuffDesc
        {
            [JsonConstructor]
            internal BuffDesc()
            {
            }
            internal BuffDesc(Buff item, ParsedEvtcLog log)
            {
                Name = item.Name;
                Icon = item.Link;
                Stacking = item.Type == Buff.BuffType.Intensity;
                BuffInfoEvent buffInfoEvent = item.BuffInfo;
                if (buffInfoEvent != null)
                {
                    Descriptions = new List<string>(){
                        "Max Stack(s) " + item.Capacity
                    };
                    foreach (BuffFormula formula in buffInfoEvent.Formulas)
                    {
                        if (formula.TraitSelf > 0 || formula.TraitSrc > 0)
                        {
                            continue;
                        }
                        var desc = formula.GetDescription(false, log.Buffs.BuffsByIds);
                        if (desc.Length > 0)
                        {
                            Descriptions.Add(desc);
                        }
                    }
                }
            }

            [JsonProperty]
            /// <summary>
            /// Name of the buff
            /// </summary>
            public string Name { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Icon of the buff
            /// </summary>
            public string Icon { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// True if the buff is stacking
            /// </summary>
            public bool Stacking { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Descriptions of the buffs (no traits)
            /// </summary>
            public List<string> Descriptions { get; internal set; }
        }

        /// <summary>
        /// Describs the damage modifier item
        /// </summary>
        public class DamageModDesc
        {
            [JsonConstructor]
            internal DamageModDesc()
            {

            }

            internal DamageModDesc(DamageModifier item)
            {
                Name = item.Name;
                Icon = item.Icon;
                Description = item.Tooltip;
                NonMultiplier = !item.Multiplier;
                SkillBased = item.SkillBased;
            }

            [JsonProperty]
            /// <summary>
            /// Name of the damage modifier
            /// </summary>
            public string Name { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Icon of the damage modifier
            /// </summary>
            public string Icon { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Description of the damage modifier
            /// </summary>
            public string Description { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// False if the modifier is multiplicative \n
            /// If true then the correspond <see cref="JsonDamageModifierData.JsonDamageModifierItem.DamageGain"/> are damage done under the effect. One will have to deduce the gain manualy depending on your gear.
            /// </summary>
            public bool NonMultiplier { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// True if the modifier is skill based
            /// </summary>
            public bool SkillBased { get; internal set; }
        }
        [JsonProperty]
        /// <summary>
        /// The used EI version.
        /// </summary>
        public string EliteInsightsVersion { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The id with which the log has been triggered
        /// </summary>
        public int TriggerID { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The name of the fight
        /// </summary>
        public string FightName { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The icon of the fight
        /// </summary>
        public string FightIcon { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The used arcdps version
        /// </summary>
        public string ArcVersion { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// GW2 build
        /// </summary>
        public ulong GW2Build { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Language with which the evtc was generated
        /// </summary>
        public string Language { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// ID of the language
        /// </summary>
        public byte LanguageID { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The player who recorded the fight
        /// </summary>
        public string RecordedBy { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// DEPRECATED: use TimeStartStd instead \n
        /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeStart { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// DEPRECATED: use TimeEndStd instead \n
        /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeEnd { get; internal set; }
        [JsonProperty]

        /// <summary>
        /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zzz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeStartStd { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zzz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeEndStd { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The duration of the fight in "xh xm xs xms" format
        /// </summary>
        public string Duration { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The success status of the fight
        /// </summary>
        public bool Success { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// If the fight is in challenge mode
        /// </summary>
        public bool IsCM { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The list of targets
        /// </summary>
        /// <seealso cref="JsonNPC"/>
        public List<JsonNPC> Targets { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The list of players
        /// </summary>
        /// <seealso cref="JsonPlayer"/>
        public List<JsonPlayer> Players { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// The list of phases
        /// </summary>
        /// <seealso cref="JsonPhase"/>
        public List<JsonPhase> Phases { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// List of mechanics
        /// </summary>
        /// <seealso cref="JsonMechanics"/>
        public List<JsonMechanics> Mechanics { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Upload links to dps.reports/raidar
        /// </summary>
        public string[] UploadLinks { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Dictionary of skills' description, the key is in "'s' + id" format
        /// </summary>
        /// <seealso cref="SkillDesc"/>
        public Dictionary<string, SkillDesc> SkillMap { get; internal set; } = new Dictionary<string, SkillDesc>();
        [JsonProperty]
        /// <summary>
        /// Dictionary of buffs' description, the key is in "'b' + id" format
        /// </summary>
        /// <seealso cref="BuffDesc"/>
        public Dictionary<string, BuffDesc> BuffMap { get; internal set; } = new Dictionary<string, BuffDesc>();
        [JsonProperty]
        /// <summary>
        /// Dictionary of damage modifiers' description, the key is in "'d' + id" format
        /// </summary>
        /// <seealso cref="DamageModDesc"/>
        public Dictionary<string, DamageModDesc> DamageModMap { get; internal set; } = new Dictionary<string, DamageModDesc>();
        [JsonProperty]
        /// <summary>
        /// Dictionary of personal buffs. The key is the profession, the value is a list of buff ids
        /// </summary>
        /// <seealso cref="BuffMap"/>
        public Dictionary<string, HashSet<long>> PersonalBuffs { get; internal set; } = new Dictionary<string, HashSet<long>>();
        [JsonProperty]
        /// <summary>
        /// List of present fractal instabilities, the values are buff ids
        /// </summary>
        /// <seealso cref="BuffMap"/>
        public List<long> PresentFractalInstabilities { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// List of error messages given by ArcDPS
        /// </summary>
        public List<string> LogErrors { get; internal set; }

        [JsonConstructor]
        internal JsonLog()
        {

        }

        internal JsonLog(ParsedEvtcLog log, RawFormatSettings settings, string[] uploadLinks)
        {
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Meta Data");
            TriggerID = log.FightData.TriggerID;
            FightName = log.FightData.GetFightName(log);
            FightIcon = log.FightData.Logic.Icon;
            EliteInsightsVersion = log.ParserVersion.ToString();
            ArcVersion = log.LogData.ArcVersion;
            RecordedBy = log.LogData.PoVName;
            TimeStart = log.LogData.LogStart;
            TimeEnd = log.LogData.LogEnd;
            TimeStartStd = log.LogData.LogStartStd;
            TimeEndStd = log.LogData.LogEndStd;
            Duration = log.FightData.DurationString;
            Success = log.FightData.Success;
            GW2Build = log.LogData.GW2Build;
            UploadLinks = uploadLinks;
            Language = log.LogData.Language;
            LanguageID = (byte)log.LogData.LanguageID;
            IsCM = log.FightData.IsCM;
            if (log.Statistics.PresentFractalInstabilities.Any())
            {
                PresentFractalInstabilities = new List<long>();
                foreach (Buff fractalInstab in log.Statistics.PresentFractalInstabilities)
                {
                    PresentFractalInstabilities.Add(fractalInstab.ID);
                    if (!BuffMap.ContainsKey("b" + fractalInstab.ID))
                    {
                        BuffMap["b" + fractalInstab.ID] = new BuffDesc(fractalInstab, log);
                    }
                }
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
                Mechanics = GetJsonMechanicsList(mechanicLogs);
            }
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Phases");
            Phases = log.FightData.GetPhases(log).Select(x => new JsonPhase(x, log)).ToList();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Targets");
            Targets = log.FightData.Logic.Targets.Select(x => new JsonNPC(x, log, settings, SkillMap, BuffMap)).ToList();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Players");
            Players = log.PlayerList.Select(x => new JsonPlayer(x, log, settings, SkillMap, BuffMap, DamageModMap, PersonalBuffs)).ToList();
            //
            if (log.LogData.LogErrors.Count > 0)
            {
                LogErrors = new List<string>(log.LogData.LogErrors);
            }
        }

    }
}
