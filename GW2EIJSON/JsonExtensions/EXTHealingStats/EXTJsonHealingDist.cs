namespace GW2EIJSON
{
    /// <summary>
    /// Class corresponding a healing distribution
    /// </summary>
    public class EXTJsonHealingDist
    {

        /// <summary>
        /// Total healing done
        /// </summary>
        public int TotalHealing { get; set; }

        /// <summary>
        /// Total healing done against downed
        /// </summary>
        public int TotalDownedHealing { get; set; }

        /// <summary>
        /// Minimum healing done
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// Maximum healing done
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// Number of hits
        /// </summary>
        public int Hits { get; set; }

        /// <summary>
        /// ID of the healing skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }

        /// <summary>
        /// True if indirect healing \n
        /// If true, the id is a buff
        /// </summary>
        public bool IndirectHealing { get; set; }


        public EXTJsonHealingDist()
        {

        }

    }
}
