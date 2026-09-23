namespace GW2EIJSON;

/// <summary>
/// Class buff volumes
/// </summary>
public class JsonBuffVolumes
{
    /// <summary>
    /// Buff volume data
    /// </summary>
    public class JsonBuffVolumesData
    {
        /// <summary>
        /// Incoming volume of the buff
        /// </summary>
        public double Incoming;

        /// <summary>
        /// Incoming by extension volume of the buff
        /// Included in <see cref="Incoming"/>
        /// </summary>
        public double IncomingByExtension;

        /// <summary>
        /// Incoming by unknown extension volume of the buff \n
        /// Included in <see cref="IncomingByExtension"/>
        /// </summary>
        public double IncomingByUnknownExtension;

        /// <summary>
        /// Buff incoming by
        /// </summary>
        public IReadOnlyDictionary<string, double>? IncomingBy;

        /// <summary>
        /// Buff incoming by extension by
        /// </summary>
        public IReadOnlyDictionary<string, double>? IncomingByExtensionBy;
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
    /// <seealso cref="JsonBuffVolumesData"/>
    public IReadOnlyList<JsonBuffVolumesData>? BuffVolumeData;
}
