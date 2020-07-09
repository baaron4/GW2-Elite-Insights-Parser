using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonMechanics;

namespace GW2EIParser.Builders.JsonModels
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
            public SkillDesc(SkillItem item)
            {
                Name = item.Name;
                AutoAttack = item.AA;
                Icon = item.Icon;
            }

            /// <summary>
            /// Name of the skill
            /// </summary>
            public string Name { get; }
            /// <summary>
            /// If the skill is an auto attack
            /// </summary>
            public bool AutoAttack { get; }
            /// <summary>
            /// Icon of the skill
            /// </summary>
            public string Icon { get; }
        }

        /// <summary>
        /// Describs the buff item
        /// </summary>
        public class BuffDesc
        {
            public BuffDesc(Buff item, ParsedLog log)
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

            /// <summary>
            /// Name of the buff
            /// </summary>
            public string Name { get; }
            /// <summary>
            /// Icon of the buff
            /// </summary>
            public string Icon { get; }
            /// <summary>
            /// True if the buff is stacking
            /// </summary>
            public bool Stacking { get; }
            /// <summary>
            /// Descriptions of the buffs (no traits)
            /// </summary>
            public List<string> Descriptions { get; }
        }

        /// <summary>
        /// Describs the damage modifier item
        /// </summary>
        public class DamageModDesc
        {
            public DamageModDesc(DamageModifier item)
            {
                Name = item.Name;
                Icon = item.Icon;
                Description = item.Tooltip;
                NonMultiplier = !item.Multiplier;
                SkillBased = item.SkillBased;
            }

            /// <summary>
            /// Name of the damage modifier
            /// </summary>
            public string Name { get; }
            /// <summary>
            /// Icon of the damage modifier
            /// </summary>
            public string Icon { get; }
            /// <summary>
            /// Description of the damage modifier
            /// </summary>
            public string Description { get; }
            /// <summary>
            /// False if the modifier is multiplicative \n
            /// If true then the correspond <see cref="JsonDamageModifierData.JsonDamageModifierItem.DamageGain"/> are damage done under the effect. One will have to deduce the gain manualy depending on your gear.
            /// </summary>
            public bool NonMultiplier { get; }
            /// <summary>
            /// True if the modifier is skill based
            /// </summary>
            public bool SkillBased { get; }
        }

        /// <summary>
        /// The used EI version
        /// </summary>
        public string EliteInsightsVersion { get; }
        /// <summary>
        /// The id with which the log has been triggered
        /// </summary>
        public int TriggerID { get; }
        /// <summary>
        /// The name of the fight
        /// </summary>
        public string FightName { get; }
        /// <summary>
        /// The icon of the fight
        /// </summary>
        public string FightIcon { get; }
        /// <summary>
        /// The used arcdps version
        /// </summary>
        public string ArcVersion { get; }
        /// <summary>
        /// GW2 build
        /// </summary>
        public ulong GW2Build { get; }
        /// <summary>
        /// Language with which the evtc was generated
        /// </summary>
        public string Language { get; }
        /// <summary>
        /// ID of the language
        /// </summary>
        public byte LanguageID { get; }
        /// <summary>
        /// The player who recorded the fight
        /// </summary>
        public string RecordedBy { get; }
        /// <summary>
        /// DEPRECATED: use TimeStartStd instead \n
        /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeStart { get; }
        /// <summary>
        /// DEPRECATED: use TimeEndStd instead \n
        /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeEnd { get; }

        /// <summary>
        /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zzz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeStartStd { get; }
        /// <summary>
        /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zzz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeEndStd { get; }
        /// <summary>
        /// The duration of the fight in "xh xm xs xms" format
        /// </summary>
        public string Duration { get; }
        /// <summary>
        /// The success status of the fight
        /// </summary>
        public bool Success { get; }
        /// <summary>
        /// If the fight is in challenge mode
        /// </summary>
        public bool IsCM { get; }
        /// <summary>
        /// The list of targets
        /// </summary>
        /// <seealso cref="JsonNPC"/>
        public List<JsonNPC> Targets { get; }
        /// <summary>
        /// The list of players
        /// </summary>
        /// <seealso cref="JsonPlayer"/>
        public List<JsonPlayer> Players { get; }
        /// <summary>
        /// The list of phases
        /// </summary>
        /// <seealso cref="JsonPhase"/>
        public List<JsonPhase> Phases { get; }
        /// <summary>
        /// List of mechanics
        /// </summary>
        /// <seealso cref="JsonMechanics"/>
        public List<JsonMechanics> Mechanics { get; }
        /// <summary>
        /// Upload links to dps.reports/raidar
        /// </summary>
        public string[] UploadLinks { get; }
        /// <summary>
        /// Dictionary of skills' description, the key is in "'s' + id" format
        /// </summary>
        /// <seealso cref="SkillDesc"/>
        public Dictionary<string, SkillDesc> SkillMap { get; } = new Dictionary<string, SkillDesc>();
        /// <summary>
        /// Dictionary of buffs' description, the key is in "'b' + id" format
        /// </summary>
        /// <seealso cref="BuffDesc"/>
        public Dictionary<string, BuffDesc> BuffMap { get; } = new Dictionary<string, BuffDesc>();
        /// <summary>
        /// Dictionary of damage modifiers' description, the key is in "'d' + id" format
        /// </summary>
        /// <seealso cref="DamageModDesc"/>
        public Dictionary<string, DamageModDesc> DamageModMap { get; } = new Dictionary<string, DamageModDesc>();
        /// <summary>
        /// Dictionary of personal buffs. The key is the profession, the value is a list of buff ids
        /// </summary>
        /// <seealso cref="BuffMap"/>
        public Dictionary<string, HashSet<long>> PersonalBuffs { get; } = new Dictionary<string, HashSet<long>>();
        /// <summary>
        /// List of present fractal instabilities, the values are buff ids
        /// </summary>
        /// <seealso cref="BuffMap"/>
        public List<long> PresentFractalInstabilities { get; }
        /// <summary>
        /// List of error messages given by ArcDPS
        /// </summary>
        public List<string> LogErrors { get; }

        public JsonLog(ParsedLog log, string[] uploadLinks)
        {
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Meta Data");
            TriggerID = log.FightData.TriggerID;
            FightName = log.FightData.GetFightName(log);
            FightIcon = log.FightData.Logic.Icon;
            EliteInsightsVersion = Application.ProductVersion;
            ArcVersion = log.LogData.BuildVersion;
            RecordedBy = log.LogData.PoVName;
            TimeStart = log.LogData.LogStart;
            TimeEnd = log.LogData.LogEnd;
            TimeStartStd = log.LogData.LogStartStd;
            TimeEndStd = log.LogData.LogEndStd;
            Duration = log.FightData.DurationString;
            Success = log.FightData.Success;
            GW2Build = log.LogData.GW2Version;
            UploadLinks = uploadLinks;
            Language = log.LogData.Language;
            LanguageID = (byte)log.LogData.LanguageID;
            IsCM = log.FightData.IsCM;
            if (log.Statistics.PresentFractalInstabilities.Any())
            {
                PresentFractalInstabilities = new List<long>();
                foreach(Buff fractalInstab in log.Statistics.PresentFractalInstabilities)
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
            Targets = log.FightData.Logic.Targets.Select(x => new JsonNPC(x, log, SkillMap, BuffMap)).ToList();
            //
            log.UpdateProgressWithCancellationCheck("Raw Format: Building Players");
            Players = log.PlayerList.Select(x => new JsonPlayer(x, log, SkillMap, BuffMap, DamageModMap, PersonalBuffs)).ToList();
            //
            if (log.LogData.LogErrors.Count > 0)
            {
                LogErrors = new List<string>(log.LogData.LogErrors);
            }
        }

    }
}
