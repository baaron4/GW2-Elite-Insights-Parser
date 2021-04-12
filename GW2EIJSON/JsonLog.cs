using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GW2EIJSON
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

            [JsonProperty]
            /// <summary>
            /// Name of the skill
            /// </summary>
            public string Name { get; set; }
            [JsonProperty]
            /// <summary>
            /// If the skill is an auto attack
            /// </summary>
            public bool AutoAttack { get; set; }
            [JsonProperty]
            /// <summary>
            /// If the skill can crit
            /// </summary>
            public bool CanCrit { get; set; }
            [JsonProperty]
            /// <summary>
            /// Icon of the skill
            /// </summary>
            public string Icon { get; set; }
            [JsonProperty]
            /// <summary>
            /// True if this skill can trigger on weapon swap sigils
            /// </summary>
            public bool IsSwap { get; set; }
            /// <summary>
            /// True in case where the skill is an instant cast and the detection may have missed some
            /// </summary>
            public bool IsNotAccurate { get; set; }
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

            [JsonProperty]
            /// <summary>
            /// Name of the buff
            /// </summary>
            public string Name { get; set; }
            [JsonProperty]
            /// <summary>
            /// Icon of the buff
            /// </summary>
            public string Icon { get; set; }
            [JsonProperty]
            /// <summary>
            /// True if the buff is stacking
            /// </summary>
            public bool Stacking { get; set; }
            [JsonProperty]
            /// <summary>
            /// Descriptions of the buffs (no traits)
            /// </summary>
            public IReadOnlyList<string> Descriptions { get; set; }
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

            [JsonProperty]
            /// <summary>
            /// Name of the damage modifier
            /// </summary>
            public string Name { get; set; }
            [JsonProperty]
            /// <summary>
            /// Icon of the damage modifier
            /// </summary>
            public string Icon { get; set; }
            [JsonProperty]
            /// <summary>
            /// Description of the damage modifier
            /// </summary>
            public string Description { get; set; }
            [JsonProperty]
            /// <summary>
            /// False if the modifier is multiplicative \n
            /// If true then the correspond <see cref="JsonDamageModifierData.JsonDamageModifierItem.DamageGain"/> are damage done under the effect. One will have to deduce the gain manualy depending on your gear.
            /// </summary>
            public bool NonMultiplier { get; set; }
            [JsonProperty]
            /// <summary>
            /// True if the modifier is skill based
            /// </summary>
            public bool SkillBased { get; set; }
        }
        [JsonProperty]
        /// <summary>
        /// The used EI version.
        /// </summary>
        public string EliteInsightsVersion { get; set; }
        [JsonProperty]
        /// <summary>
        /// The id with which the log has been triggered
        /// </summary>
        public int TriggerID { get; set; }
        [JsonProperty]
        /// <summary>
        /// The name of the fight
        /// </summary>
        public string FightName { get; set; }
        [JsonProperty]
        /// <summary>
        /// The icon of the fight
        /// </summary>
        public string FightIcon { get; set; }
        [JsonProperty]
        /// <summary>
        /// The used arcdps version
        /// </summary>
        public string ArcVersion { get; set; }
        [JsonProperty]
        /// <summary>
        /// GW2 build
        /// </summary>
        public ulong GW2Build { get; set; }
        [JsonProperty]
        /// <summary>
        /// Language with which the evtc was generated
        /// </summary>
        public string Language { get; set; }
        [JsonProperty]
        /// <summary>
        /// ID of the language
        /// </summary>
        public byte LanguageID { get; set; }
        [JsonProperty]
        /// <summary>
        /// The player who recorded the fight
        /// </summary>
        public string RecordedBy { get; set; }
        [JsonProperty]
        /// <summary>
        /// DEPRECATED: use TimeStartStd instead \n
        /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeStart { get; set; }
        [JsonProperty]
        /// <summary>
        /// DEPRECATED: use TimeEndStd instead \n
        /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeEnd { get; set; }
        [JsonProperty]

        /// <summary>
        /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zzz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeStartStd { get; set; }
        [JsonProperty]
        /// <summary>
        /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zzz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeEndStd { get; set; }
        [JsonProperty]
        /// <summary>
        /// The duration of the fight in "xh xm xs xms" format
        /// </summary>
        public string Duration { get; set; }
        [JsonProperty]
        /// <summary>
        /// The success status of the fight
        /// </summary>
        public bool Success { get; set; }
        [JsonProperty]
        /// <summary>
        /// If the fight is in challenge mode
        /// </summary>
        public bool IsCM { get; set; }
        [JsonProperty]
        /// <summary>
        /// The list of targets
        /// </summary>
        /// <seealso cref="JsonNPC"/>
        public IReadOnlyList<JsonNPC> Targets { get; set; }
        [JsonProperty]
        /// <summary>
        /// The list of players
        /// </summary>
        /// <seealso cref="JsonPlayer"/>
        public IReadOnlyList<JsonPlayer> Players { get; set; }
        [JsonProperty]
        /// <summary>
        /// The list of phases
        /// </summary>
        /// <seealso cref="JsonPhase"/>
        public IReadOnlyList<JsonPhase> Phases { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of mechanics
        /// </summary>
        /// <seealso cref="JsonMechanics"/>
        public IReadOnlyList<JsonMechanics> Mechanics { get; set; }
        [JsonProperty]
        /// <summary>
        /// Upload links to dps.reports/raidar
        /// </summary>
        public IReadOnlyList<string> UploadLinks { get; set; }
        [JsonProperty]
        /// <summary>
        /// Dictionary of skills' description, the key is in "'s' + id" format
        /// </summary>
        /// <seealso cref="SkillDesc"/>
        public IReadOnlyDictionary<string, SkillDesc> SkillMap { get; set; }
        [JsonProperty]
        /// <summary>
        /// Dictionary of buffs' description, the key is in "'b' + id" format
        /// </summary>
        /// <seealso cref="BuffDesc"/>
        public IReadOnlyDictionary<string, BuffDesc> BuffMap { get; set; }
        [JsonProperty]
        /// <summary>
        /// Dictionary of damage modifiers' description, the key is in "'d' + id" format
        /// </summary>
        /// <seealso cref="DamageModDesc"/>
        public IReadOnlyDictionary<string, DamageModDesc> DamageModMap { get; set; }
        [JsonProperty]
        /// <summary>
        /// Dictionary of personal buffs. The key is the profession, the value is a list of buff ids
        /// </summary>
        /// <seealso cref="BuffMap"/>
        public IReadOnlyDictionary<string, IReadOnlyCollection<long>> PersonalBuffs { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of present fractal instabilities, the values are buff ids
        /// </summary>
        /// <seealso cref="BuffMap"/>
        public IReadOnlyList<long> PresentFractalInstabilities { get; set; }
        [JsonProperty]
        /// <summary>
        /// List of error messages given by ArcDPS
        /// </summary>
        public IReadOnlyList<string> LogErrors { get; set; }

        [JsonConstructor]
        internal JsonLog()
        {

        }

    }
}
