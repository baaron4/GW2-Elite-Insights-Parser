using System;
using System.Collections.Generic;
using System.Linq;


namespace GW2EIJSON
{
    /// <summary>
    /// Class corresponding a damage distribution
    /// </summary>
    public class JsonDamageDist
    {
        
        /// <summary>
        /// Total damage done
        /// </summary>
        public int TotalDamage { get; set; }
        
        /// <summary>
        /// Minimum damage done
        /// </summary>
        public int Min { get; set; }
        
        /// <summary>
        /// Maximum damage done
        /// </summary>
        public int Max { get; set; }
        
        /// <summary>
        /// Number of hits
        /// </summary>
        public int Hits { get; set; }
        
        /// <summary>
        /// Number of connected hits
        /// </summary>
        public int ConnectedHits { get; set; }
        
        /// <summary>
        /// Number of crits
        /// </summary>
        public int Crit { get; set; }
        
        /// <summary>
        /// Number of glances
        /// </summary>
        public int Glance { get; set; }
        
        /// <summary>
        /// Number of flanks
        /// </summary>
        public int Flank { get; set; }
        
        /// <summary>
        /// Number of times the hit missed due to blindness
        /// </summary>
        public int Missed { get; set; }
        
        /// <summary>
        /// Number of times the hit was invulned
        /// </summary>
        public int Invulned { get; set; }
        
        /// <summary>
        /// Number of times the hit nterrupted
        /// </summary>
        public int Interrupted { get; set; }
        
        /// <summary>
        /// Number of times the hit was evaded
        /// </summary>
        public int Evaded { get; set; }
        
        /// <summary>
        /// Number of times the hit was blocked
        /// </summary>
        public int Blocked { get; set; }
        
        /// <summary>
        /// Damage done against barrier, not necessarily included in total damage
        /// </summary>
        public int ShieldDamage { get; set; }
        
        /// <summary>
        /// Critical damage
        /// </summary>
        public int CritDamage { get; set; }
        
        /// <summary>
        /// ID of the damaging skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }
        
        /// <summary>
        /// True if indirect damage \n
        /// If true, the id is a buff
        /// </summary>
        public bool IndirectDamage { get; set; }

        
        public JsonDamageDist()
        {

        }  

    }
}
