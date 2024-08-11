using System.Collections.Generic;


namespace GW2EIJSON
{
    /// <summary>
    /// Class representing outgoing buff volumes by player actors
    /// </summary>
    public class JsonPlayerBuffOutgoingVolumes
    {
        /// <summary>
        /// Player buffs outgoing volume item
        /// </summary>
        public class JsonBuffOutgoingVolumesData
        {
            
            /// <summary>
            /// Outgoing volume
            /// </summary>
            public double Outgoing { get; set; }

            /// <summary>
            /// Outgoing volume by extension, included in <see cref="Outgoing"/>
            /// </summary>
            public double OutgoingByExtension { get; set; }

            
            public JsonBuffOutgoingVolumesData()
            {

            }

        }

        
        /// <summary>
        /// ID of the buff
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }

        /// <summary>
        /// Array of buff volume data \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonBuffOutgoingVolumesData"/>
        public IReadOnlyList<JsonBuffOutgoingVolumesData> BuffVolumeData { get; set; }
    }
}
