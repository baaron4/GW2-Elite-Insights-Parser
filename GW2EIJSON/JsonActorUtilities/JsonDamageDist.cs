using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GW2EIJSON
{
    /// <summary>
    /// Class corresponding a damage distribution
    /// </summary>
    public class JsonDamageDist
    {
        [JsonProperty]
        /// <summary>
        /// Total damage done
        /// </summary>
        public int TotalDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Minimum damage done
        /// </summary>
        public int Min { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Maximum damage done
        /// </summary>
        public int Max { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of hits
        /// </summary>
        public int Hits { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of connected hits
        /// </summary>
        public int ConnectedHits { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of crits
        /// </summary>
        public int Crit { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of glances
        /// </summary>
        public int Glance { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of flanks
        /// </summary>
        public int Flank { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of times the hit missed due to blindness
        /// </summary>
        public int Missed { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of times the hit was invulned
        /// </summary>
        public int Invulned { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of times the hit nterrupted
        /// </summary>
        public int Interrupted { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of times the hit was evaded
        /// </summary>
        public int Evaded { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Number of times the hit was blocked
        /// </summary>
        public int Blocked { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Damage done against barrier, not necessarily included in total damage
        /// </summary>
        public int ShieldDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// Critical damage
        /// </summary>
        public int CritDamage { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// ID of the damaging skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// True if indirect damage \n
        /// If true, the id is a buff
        /// </summary>
        public bool IndirectDamage { get; internal set; }

        [JsonConstructor]
        internal JsonDamageDist()
        {

        }  

    }
}
