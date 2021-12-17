

namespace GW2EIJSON
{
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
            public int Bps { get; set; }
            /// <summary>
            /// Total barrier
            /// </summary>
            public int Barrier { get; set; }


            /// <summary>
            /// Total actor only Bps
            /// </summary>
            public int ActorBps { get; set; }
            /// <summary>
            /// Total actor only barrier
            /// </summary>
            public int ActorBarrier { get; set; }

            public EXTJsonOutgoingBarrierStatistics()
            {

            }
        }

        /// <summary>
        /// Incoming barrier statistics
        /// </summary>
        public class EXTJsonIncomingBarrierStatistics
        {
            /// <summary>
            /// Total received Barrier
            /// </summary>
            public int Barrier { get; set; }

            public EXTJsonIncomingBarrierStatistics()
            {

            }
        }
    }
}
