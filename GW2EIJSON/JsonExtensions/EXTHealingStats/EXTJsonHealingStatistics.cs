

namespace GW2EIJSON
{
    /// <summary>
    /// Class representing general statistics
    /// </summary>
    public static class EXTJsonHealingStatistics
    {
        public class EXTJsonOutgoingHealingStatistics
        {
            public double Hps { get; set; }
            public int Healing { get; set; }
            public double HealingPowerHps { get; set; }
            public int HealingPowerHealing { get; set; }
            public double ConversionHps { get; set; }
            public int ConversionHealing { get; set; }

            public EXTJsonOutgoingHealingStatistics()
            {

            }
        }

        public class EXTJsonIncomingHealingStatistics
        {
            public int Healed { get; set; }
            public int HealingPowerHealed { get; set; }
            public int ConversionHealed { get; set; }

            public EXTJsonIncomingHealingStatistics()
            {

            }
        }
    }
}
