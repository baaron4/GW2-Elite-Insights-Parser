

namespace GW2EIJSON;

/// <summary>
/// Class representing general statistics
/// </summary>
public static class EXTJsonBarrierStatistics
{
    /// <summary>
    /// Outgoing barrier statistics
    /// </summary>
    public class EXTJsonOutgoingBarrierStatistics
    {
        /// <summary>
        /// Total bps
        /// </summary>
        public int Bps;
        /// <summary>
        /// Total barrier
        /// </summary>
        public int Barrier;


        /// <summary>
        /// Total actor only Bps
        /// </summary>
        public int ActorBps;
        /// <summary>
        /// Total actor only barrier
        /// </summary>
        public int ActorBarrier;
    }

    /// <summary>
    /// Incoming barrier statistics
    /// </summary>
    public class EXTJsonIncomingBarrierStatistics
    {
        /// <summary>
        /// Total received Barrier
        /// </summary>
        public int Barrier;
    }
}
