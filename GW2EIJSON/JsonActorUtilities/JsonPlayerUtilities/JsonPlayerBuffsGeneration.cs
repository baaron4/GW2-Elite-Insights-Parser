namespace GW2EIJSON;

/// <summary>
/// Class representing buffs generation by player actors
/// </summary>
public class JsonPlayerBuffsGeneration
{
    /// <summary>
    /// Player buffs generation item
    /// </summary>
    public class JsonBuffsGenerationData
    {
        /// <summary>
        /// Generation done
        /// </summary>
        public double Generation;
        /// <summary>
        /// Generation done as presence, only relevant for intensity stacking buffs, will be 0 for duration stacking buffs
        /// </summary>
        public double GenerationPresence;

        /// <summary>
        /// Generation with overstack
        /// </summary>
        public double Overstack;

        /// <summary>
        /// Wasted generation
        /// </summary>
        public double Wasted;

        /// <summary>
        /// Extension from unknown source
        /// </summary>
        public double UnknownExtended;

        /// <summary>
        /// Generation done by extension
        /// </summary>
        public double ByExtension;

        /// <summary>
        /// Buff extended 
        /// </summary>
        public double Extended;
    }


    /// <summary>
    /// ID of the buff
    /// </summary>
    /// <seealso cref="JsonLog.BuffMap"/>
    public long Id;

    /// <summary>
    /// Array of buff data \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonBuffsGenerationData"/>
    public IReadOnlyList<JsonBuffsGenerationData>? BuffData;
}
