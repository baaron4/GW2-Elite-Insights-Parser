using System;
using System.Collections.Generic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.JsonModels
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
            public string Name;
            /// <summary>
            /// If the skill is an auto attack
            /// </summary>
            public bool AutoAttack;
            /// <summary>
            /// Icon of the skill
            /// </summary>
            public string Icon;
        }

        /// <summary>
        /// Describs the buff item
        /// </summary>
        public class BuffDesc
        {
            public BuffDesc(Boon item)
            {
                Name = item.Name;
                Icon = item.Link;
                Stacking = item.Type == Boon.BoonType.Intensity;
            }

            /// <summary>
            /// Name of the buff
            /// </summary>
            public string Name;
            /// <summary>
            /// Icon of the buff
            /// </summary>
            public string Icon;
            /// <summary>
            /// True if the buff is stacking
            /// </summary>
            public bool Stacking;
        }

        /// <summary>
        /// Describs the damage modifier item
        /// </summary>
        public class DamageModDesc
        {
            public DamageModDesc(DamageModifier item)
            {
                Name = item.Name;
                Icon = item.Url;
            }

            /// <summary>
            /// Name of the damage modifier
            /// </summary>
            public string Name;
            /// <summary>
            /// Icon of the damage modifier
            /// </summary>
            public string Icon;
        }

        /// <summary>
        /// The used EI version
        /// </summary>
        public string EliteInsightsVersion;
        /// <summary>
        /// The id with which the log has been triggered
        /// </summary>
        public int TriggerID;
        /// <summary>
        /// The name of the fight
        /// </summary>
        public string FightName;
        /// <summary>
        /// The used arcdps version
        /// </summary>
        public string ArcVersion;
        /// <summary>
        /// The player who recorded the fight
        /// </summary>
        public string RecordedBy;
        /// <summary>
        /// The time at which the fight started in "yyyy-mm-dd hh:mm:ss" format 
        /// </summary>
        public string TimeStart;
        /// <summary>
        /// The time at which the fight ended in "yyyy-mm-dd hh:mm:ss" format
        /// </summary>
        public string TimeEnd;
        /// <summary>
        /// The duration of the fight in "xh xm xs xms" format
        /// </summary>
        public string Duration;
        /// <summary>
        /// The success status of the fight
        /// </summary>
        public bool Success;
        /// <summary>
        /// The list of targets
        /// </summary>
        /// <seealso cref="JsonTarget"/>
        public List<JsonTarget> Targets;
        /// <summary>
        /// The list of players
        /// </summary>
        /// <seealso cref="JsonPlayer"/>
        public List<JsonPlayer> Players;
        /// <summary>
        /// The list of phases
        /// </summary>
        /// <seealso cref="JsonPhase"/>
        public List<JsonPhase> Phases;
        /// <summary>
        /// List of mechanics
        /// </summary>
        /// <seealso cref="JsonMechanics"/>
        public List<JsonMechanics> Mechanics;
        /// <summary>
        /// Upload links to dps.reports/raidar
        /// </summary>
        public string[] UploadLinks;
        /// <summary>
        /// Dictionary of skills' description, the key is in "'s' + id" format
        /// </summary>
        /// <seealso cref="SkillDesc"/>
        public Dictionary<string, SkillDesc> SkillMap;
        /// <summary>
        /// Dictionary of buffs' description, the key is in "'b' + id" format
        /// </summary>
        /// <seealso cref="BuffDesc"/>
        public Dictionary<string, BuffDesc> BuffMap;
        /// <summary>
        /// Dictionary of damage modifiers' description, the key is in "'d' + id" format
        /// </summary>
        /// <seealso cref="DamageModDesc"/>
        public Dictionary<string, DamageModDesc> DamageModMap;
        /// <summary>
        /// Dictionary of personal buffs. The key is the profession, the value is a list of buff ids
        /// </summary>
        public Dictionary<string, HashSet<long>> PersonalBuffs;
    }
}