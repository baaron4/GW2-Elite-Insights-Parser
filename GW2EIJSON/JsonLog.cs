using System;
using System.Collections.Generic;
using System.Linq;


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
            
            public SkillDesc()
            {

            }

            
            /// <summary>
            /// Name of the skill
            /// </summary>
            public string Name { get; set; }
            
            /// <summary>
            /// If the skill is an auto attack
            /// </summary>
            public bool AutoAttack { get; set; }
            
            /// <summary>
            /// If the skill can crit
            /// </summary>
            public bool CanCrit { get; set; }
            
            /// <summary>
            /// Icon of the skill
            /// </summary>
            public string Icon { get; set; }
            
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
            
            public BuffDesc()
            {
            }

            
            /// <summary>
            /// Name of the buff
            /// </summary>
            public string Name { get; set; }
            
            /// <summary>
            /// Icon of the buff
            /// </summary>
            public string Icon { get; set; }
            
            /// <summary>
            /// True if the buff is stacking
            /// </summary>
            public bool Stacking { get; set; }
            
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
            
            public DamageModDesc()
            {

            }

            
            /// <summary>
            /// Name of the damage modifier
            /// </summary>
            public string Name { get; set; }
            
            /// <summary>
            /// Icon of the damage modifier
            /// </summary>
            public string Icon { get; set; }
            
            /// <summary>
            /// Description of the damage modifier
            /// </summary>
            public string Description { get; set; }
            
            /// <summary>
            /// False if the modifier is multiplicative \n
            /// If true then the correspond <see cref="JsonDamageModifierData.JsonDamageModifierItem.DamageGain"/> are damage done under the effect. One will have to deduce the gain manualy depending on your gear.
            /// </summary>
            public bool NonMultiplier { get; set; }
            
            /// <summary>
            /// True if the modifier is skill based
            /// </summary>
            public bool SkillBased { get; set; }
        }
        
        /// <summary>
        /// The used EI version.
        /// </summary>
        public string EliteInsightsVersion { get; set; }
        
        /// <summary>
        /// The id with which the log has been triggered
        /// </summary>
        public int TriggerID { get; set; }
        
        /// <summary>
        /// The name of the fight
        /// </summary>
        public string FightName { get; set; }
        
        /// <summary>
        /// The icon of the fight
        /// </summary>
        public string FightIcon { get; set; }
        
        /// <summary>
        /// The used arcdps version
        /// </summary>
        public string ArcVersion { get; set; }
        
        /// <summary>
        /// GW2 build
        /// </summary>
        public ulong GW2Build { get; set; }
        
        /// <summary>
        /// Language with which the evtc was generated
        /// </summary>
        public string Language { get; set; }
        
        /// <summary>
        /// ID of the language
        /// </summary>
        public byte LanguageID { get; set; }
        
        /// <summary>
        /// The player who recorded the fight
        /// </summary>
        public string RecordedBy { get; set; }
        
        /// <summary>
        /// DEPRECATED: use TimeStartStd instead \n
        /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeStart { get; set; }
        
        /// <summary>
        /// DEPRECATED: use TimeEndStd instead \n
        /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeEnd { get; set; }
        

        /// <summary>
        /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss zzz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeStartStd { get; set; }
        
        /// <summary>
        /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss zzz" format \n
        /// The value will be <see cref="LogData.DefaultTimeValue"/> if the event does not exist
        /// </summary>
        public string TimeEndStd { get; set; }
        
        /// <summary>
        /// The duration of the fight in "xh xm xs xms" format
        /// </summary>
        public string Duration { get; set; }
        
        /// <summary>
        /// The success status of the fight
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// If the fight is in challenge mode
        /// </summary>
        public bool IsCM { get; set; }
        
        /// <summary>
        /// The list of targets
        /// </summary>
        /// <seealso cref="JsonNPC"/>
        public IReadOnlyList<JsonNPC> Targets { get; set; }
        
        /// <summary>
        /// The list of players
        /// </summary>
        /// <seealso cref="JsonPlayer"/>
        public IReadOnlyList<JsonPlayer> Players { get; set; }
        
        /// <summary>
        /// The list of phases
        /// </summary>
        /// <seealso cref="JsonPhase"/>
        public IReadOnlyList<JsonPhase> Phases { get; set; }
        
        /// <summary>
        /// List of mechanics
        /// </summary>
        /// <seealso cref="JsonMechanics"/>
        public IReadOnlyList<JsonMechanics> Mechanics { get; set; }
        
        /// <summary>
        /// Upload links to dps.reports/raidar
        /// </summary>
        public IReadOnlyList<string> UploadLinks { get; set; }
        
        /// <summary>
        /// Dictionary of skills' description, the key is in "'s' + id" format
        /// </summary>
        /// <seealso cref="SkillDesc"/>
        public IReadOnlyDictionary<string, SkillDesc> SkillMap { get; set; }
        
        /// <summary>
        /// Dictionary of buffs' description, the key is in "'b' + id" format
        /// </summary>
        /// <seealso cref="BuffDesc"/>
        public IReadOnlyDictionary<string, BuffDesc> BuffMap { get; set; }
        
        /// <summary>
        /// Dictionary of damage modifiers' description, the key is in "'d' + id" format
        /// </summary>
        /// <seealso cref="DamageModDesc"/>
        public IReadOnlyDictionary<string, DamageModDesc> DamageModMap { get; set; }
        
        /// <summary>
        /// Dictionary of personal buffs. The key is the profession, the value is a list of buff ids
        /// </summary>
        /// <seealso cref="BuffMap"/>
        public IReadOnlyDictionary<string, IReadOnlyCollection<long>> PersonalBuffs { get; set; }
        
        /// <summary>
        /// List of present fractal instabilities, the values are buff ids
        /// </summary>
        /// <seealso cref="BuffMap"/>
        public IReadOnlyList<long> PresentFractalInstabilities { get; set; }
        
        /// <summary>
        /// List of error messages given by ArcDPS
        /// </summary>
        public IReadOnlyList<string> LogErrors { get; set; }

        
        public JsonLog()
        {

        }

    }
}
