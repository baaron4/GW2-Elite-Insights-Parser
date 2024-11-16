

namespace GW2EIJSON;

/// <summary>
/// Class representing general statistics
/// </summary>
public static class EXTJsonHealingStatistics
{
    /// <summary>
    /// Outgoing healing statistics
    /// </summary>
    public class EXTJsonOutgoingHealingStatistics
    {
        /// <summary>
        /// Total hps
        /// </summary>
        public int Hps;
        /// <summary>
        /// Total healing
        /// </summary>
        public int Healing;
        /// <summary>
        /// Total healing power based hps
        /// </summary>
        public int HealingPowerHps;
        /// <summary>
        /// Total healing power based healing
        /// </summary>
        public int HealingPowerHealing;
        /// <summary>
        /// Total conversion based hps
        /// </summary>
        public int ConversionHps;
        /// <summary>
        /// Total conversion based healing
        /// </summary>
        public int ConversionHealing;
        /// <summary>
        /// Total hybrid hps
        /// </summary>
        public int HybridHps;
        /// <summary>
        /// Total hybrid healing
        /// </summary>
        public int HybridHealing;

        /// <summary>
        /// Total hps against downed
        /// </summary>
        public int DownedHps;
        /// <summary>
        /// Total healing against downed
        /// </summary>
        public int DownedHealing;


        /// <summary>
        /// Total actor only hps
        /// </summary>
        public int ActorHps;
        /// <summary>
        /// Total actor only healing
        /// </summary>
        public int ActorHealing;
        /// <summary>
        /// Total actor only healing power based hps
        /// </summary>
        public int ActorHealingPowerHps;
        /// <summary>
        /// Total actor only healing power based healing
        /// </summary>
        public int ActorHealingPowerHealing;
        /// <summary>
        /// Total actor only conversion based hps
        /// </summary>
        public int ActorConversionHps;
        /// <summary>
        /// Total actor only conversion based healing
        /// </summary>
        public int ActorConversionHealing;
        /// <summary>
        /// Total actor only hybrid hps
        /// </summary>
        public int ActorHybridHps;
        /// <summary>
        /// Total actor only hybrid healing
        /// </summary>
        public int ActorHybridHealing;

        /// <summary>
        /// Total actor only hps against downed
        /// </summary>
        public int ActorDownedHps;
        /// <summary>
        /// Total actor only healing against downed
        /// </summary>
        public int ActorDownedHealing;
    }

    /// <summary>
    /// Incoming healing statistics
    /// </summary>
    public class EXTJsonIncomingHealingStatistics
    {
        /// <summary>
        /// Total received healing
        /// </summary>
        public int Healed;
        /// <summary>
        /// Total received healing power based healing
        /// </summary>
        public int HealingPowerHealed;
        /// <summary>
        /// Total received conversion based healing
        /// </summary>
        public int ConversionHealed;
        /// <summary>
        /// Total received hybrid healing
        /// </summary>
        public int HybridHealed;
        /// <summary>
        /// Total healing received while downed
        /// </summary>
        public int DownedHealed;
    }
}
