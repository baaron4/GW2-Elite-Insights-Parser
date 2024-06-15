using System.Collections.Generic;


namespace GW2EIJSON
{
    /// <summary>
    /// Combat Replay meta data
    /// </summary>
    public class JsonCombatReplayMetaData
    {
        /// <summary>
        /// Represents a combat replay map
        /// </summary>
        public class CombatReplayMap
        {
            /// <summary>
            /// Url of the map
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// Interval in ms, between which the map should be used.\n
            /// Interval[0] is start and Interval[1] is end.
            /// </summary>
            public long[] Interval { get; set; }
        }
        /// <summary>
        /// Factor to convert inches (in game unit) to pixels (map unit)
        /// </summary>
        public float InchToPixel { get; set; }
        /// <summary>
        /// Polling rate of the time based information (facings, positions, etc...) in ms. \n
        /// A polling rate of 150 means that 150 ms separates two time based information. \n
        /// Time based 
        /// </summary>
        public int PollingRate { get; set; }
        /// <summary>
        /// Sizes[0] is width of the map in pixel and Sizes[1] is height of the map in pixel. \n
        /// All maps in <see cref="Maps"/> are of the same pixel size.
        /// </summary>
        public int[] Sizes { get; set; }
        /// <summary>
        /// List of maps used for Combat Replay
        /// </summary>
        /// <seealso cref="CombatReplayMap"/>
        public IReadOnlyList<CombatReplayMap> Maps { get; set; }

        public JsonCombatReplayMetaData()
        {

        }

    }
}
