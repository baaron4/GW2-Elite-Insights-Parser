using System.Collections.Generic;


namespace GW2EIJSON
{
    /// <summary>
    /// Class regrouping minion related Barrier statistics
    /// </summary>
    public class EXTJsonMinionsBarrierStats(
        IReadOnlyList<int> totalBarrier,
        IReadOnlyList<IReadOnlyList<int>> totalAlliedBarrier,
        IReadOnlyList<IReadOnlyList<EXTJsonBarrierDist>> totalBarrierDist,
        IReadOnlyList<IReadOnlyList<IReadOnlyList<EXTJsonBarrierDist>>> alliedBarrierDist
    ) {

        /// <summary>
        /// Total barrier done by minions \n
        /// Length == # of phases
        /// </summary>
        public readonly IReadOnlyList<int> TotalBarrier = totalBarrier;
        /// <summary>
        /// Total Allied Barrier done by minions \n
        /// Length == # of players and the length of each sub array is equal to # of phases
        /// </summary>
        public readonly IReadOnlyList<IReadOnlyList<int>> TotalAlliedBarrier = totalAlliedBarrier;
        /// <summary>
        /// Total Outgoing Barrier distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="EXTJsonBarrierDist"/>
        public readonly IReadOnlyList<IReadOnlyList<EXTJsonBarrierDist>> TotalBarrierDist = totalBarrierDist;

        /// <summary>
        /// Total Outgoing Allied Barrier distribution array \n
        /// Length == # of players and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="EXTJsonBarrierDist"/>
        public readonly IReadOnlyList<IReadOnlyList<IReadOnlyList<EXTJsonBarrierDist>>> AlliedBarrierDist = alliedBarrierDist;
    }
}
