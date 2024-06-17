using System.Collections.Generic;


namespace GW2EIJSON
{
    /// <summary>
    /// Class regrouping minion related Barrier statistics
    /// </summary>
    public class EXTJsonMinionsBarrierStats
    {
        /// <summary>
        /// Total barrier done by minions \n
        /// Length == # of phases
        /// </summary>
        public IReadOnlyList<int> TotalBarrier { get; set; }
        /// <summary>
        /// Total Allied Barrier done by minions \n
        /// Length == # of players and the length of each sub array is equal to # of phases
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> TotalAlliedBarrier { get; set; }
        /// <summary>
        /// Total Outgoing Barrier distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="EXTJsonBarrierDist"/>
        public IReadOnlyList<IReadOnlyList<EXTJsonBarrierDist>> TotalBarrierDist { get; set; }

        /// <summary>
        /// Total Outgoing Allied Barrier distribution array \n
        /// Length == # of players and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="EXTJsonBarrierDist"/>
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<EXTJsonBarrierDist>>> AlliedBarrierDist { get; set; }

        public EXTJsonMinionsBarrierStats()
        {

        }
    }
}
