namespace GW2EIJSON;

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
        public double Outgoing;

        /// <summary>
        /// Outgoing volume by extension, included in <see cref="Outgoing"/>
        /// </summary>
        public double OutgoingByExtension;
    }


    /// <summary>
    /// ID of the buff
    /// </summary>
    /// <seealso cref="JsonLog.BuffMap"/>
    public long Id;

    /// <summary>
    /// Array of buff volume data \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonBuffOutgoingVolumesData"/>
    public IReadOnlyList<JsonBuffOutgoingVolumesData>? BuffVolumeData;
}
