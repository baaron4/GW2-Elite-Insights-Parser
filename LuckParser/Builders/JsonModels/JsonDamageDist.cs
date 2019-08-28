using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding a damage distribution
    /// </summary>
    public class JsonDamageDist
    {
        /// <summary>
        /// Total damage done
        /// </summary>
        public int TotalDamage;
        /// <summary>
        /// Minimum damage done
        /// </summary>
        public int Min;
        /// <summary>
        /// Maximum damage done
        /// </summary>
        public int Max;
        /// <summary>
        /// Number of hits
        /// </summary>
        public int Hits;
        /// <summary>
        /// Number of crits
        /// </summary>
        public int Crit;
        /// <summary>
        /// Number of glances
        /// </summary>
        public int Glance;
        /// <summary>
        /// Number of flanks
        /// </summary>
        public int Flank;
        /// <summary>
        /// Damage done against barrier, not necessarily included in total damage
        /// </summary>
        public int ShieldDamage;
        /// <summary>
        /// ID of the damaging skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id;
        /// <summary>
        /// True if indirect damage
        /// </summary>
        public bool IndirectDamage;

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
