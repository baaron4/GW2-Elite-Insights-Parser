

namespace GW2EIJSON
{
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
            public double Hps { get; set; }
            /// <summary>
            /// Total healing
            /// </summary>
            public int Healing { get; set; }
            /// <summary>
            /// Total healing power based hps
            /// </summary>
            public double HealingPowerHps { get; set; }
            /// <summary>
            /// Total healing power based healing
            /// </summary>
            public int HealingPowerHealing { get; set; }
            /// <summary>
            /// Total conversion based hps
            /// </summary>
            public double ConversionHps { get; set; }
            /// <summary>
            /// Total conversion based healing
            /// </summary>
            public int ConversionHealing { get; set; }


            /// <summary>
            /// Total actor hps
            /// </summary>
            public double ActorHps { get; set; }
            /// <summary>
            /// Total actor healing
            /// </summary>
            public int ActorHealing { get; set; }
            /// <summary>
            /// Total actor healing power based hps
            /// </summary>
            public double ActorHealingPowerHps { get; set; }
            /// <summary>
            /// Total actor healing power based healing
            /// </summary>
            public int ActorHealingPowerHealing { get; set; }
            /// <summary>
            /// Total actor conversion based hps
            /// </summary>
            public double ActorConversionHps { get; set; }
            /// <summary>
            /// Total actor conversion based healing
            /// </summary>
            public int ActorConversionHealing { get; set; }

            public EXTJsonOutgoingHealingStatistics()
            {

            }
        }

        /// <summary>
        /// Incoming healing statistics
        /// </summary>
        public class EXTJsonIncomingHealingStatistics
        {
            /// <summary>
            /// Total received healing
            /// </summary>
            public int Healed { get; set; }
            /// <summary>
            /// Total received healing power based healing
            /// </summary>
            public int HealingPowerHealed { get; set; }
            /// <summary>
            /// Total received conversion based healing
            /// </summary>
            public int ConversionHealed { get; set; }

            public EXTJsonIncomingHealingStatistics()
            {

            }
        }
    }
}
