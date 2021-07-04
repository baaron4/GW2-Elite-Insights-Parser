using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class HealingStatsExtensionHandler : AbstractExtensionHandler
    {

        public const uint EXT_HealingStats = 0x9c9b3c99;
        public enum EXTHealingType { All, HealingPower, ConversionBased };

        internal HealingStatsExtensionHandler() : base(EXT_HealingStats, "Healing Stats")
        {
            Revision = 0;
        }

    }
}
