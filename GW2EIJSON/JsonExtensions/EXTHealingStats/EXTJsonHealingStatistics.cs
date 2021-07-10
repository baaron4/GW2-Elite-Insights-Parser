

namespace GW2EIJSON
{
    /// <summary>
    /// Class representing general statistics
    /// </summary>
    public static class EXTJsonHealingStatistics
    {
        public class EXTJsonOutgoingHealingStatistics
        {
            public double Hps { get; internal set; }
            public int Healing { get; internal set; }
            public double HealingPowerHps { get; internal set; }
            public int HealingPowerHealing { get; internal set; }
            public double ConversionHps { get; internal set; }
            public int ConversionHealing { get; internal set; }

            public EXTJsonOutgoingHealingStatistics()
            {

            }
        }

        public class EXTJsonIncomingHealingStatistics
        {
            public int Healed { get; internal set; }
            public int HealingPowerHealed { get; internal set; }
            public int ConversionHealed { get; internal set; }

            public EXTJsonIncomingHealingStatistics()
            {

            }
        }
    }
}
