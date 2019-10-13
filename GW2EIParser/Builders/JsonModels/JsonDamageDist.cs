using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Builders.JsonModels
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
        /// Damage done against barrier, not necessarily included in total damage
        /// </summary>
        public int ShieldDamage { get; set; }
        /// <summary>
        /// ID of the damaging skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }
        /// <summary>
        /// True if indirect damage
        /// </summary>
        public bool IndirectDamage { get; set; }

        public JsonDamageDist(List<AbstractDamageEvent> list, bool indirectDamage, long id)
        {
            Hits = list.Count;
            TotalDamage = list.Sum(x => x.Damage);
            Min = list.Min(x => x.Damage);
            Max = list.Max(x => x.Damage);
            Flank = indirectDamage ? 0 : list.Count(x => x.IsFlanking);
            Crit = indirectDamage ? 0 : list.Count(x => x.HasCrit);
            Glance = indirectDamage ? 0 : list.Count(x => x.HasGlanced);
            ShieldDamage = list.Sum(x => x.ShieldDamage);
            IndirectDamage = indirectDamage;
            Id = id;
        }
    }
}
