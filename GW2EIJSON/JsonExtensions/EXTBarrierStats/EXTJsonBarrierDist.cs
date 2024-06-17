namespace GW2EIJSON
{
    /// <summary>
    /// Class corresponding a barrier distribution
    /// </summary>
    public class EXTJsonBarrierDist
    {

        /// <summary>
        /// Total barrier done
        /// </summary>
        public int TotalBarrier { get; set; }

        /// <summary>
        /// Minimum barrier done
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// Maximum barrier done
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// Number of hits
        /// </summary>
        public int Hits { get; set; }

        /// <summary>
        /// ID of the barrier skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }

        /// <summary>
        /// True if indirect barrier \n
        /// If true, the id is a buff
        /// </summary>
        public bool IndirectBarrier { get; set; }


        public EXTJsonBarrierDist()
        {

        }

    }
}
