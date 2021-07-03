using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.Extensions
{
    public static class ExtensionHelper
    {
        // TODO add supported sigs as public consts
        public const uint EXT_HealingStats = 0x9c9b3c99;

        internal static AbstractExtensionHandler GetExtensionHandler(CombatItem c)
        {
            if (!c.IsExtension && c.Pad != 0)
            {
                return null;
            }
            // place holder for sig
            switch (c.SrcAgent & 0x00000000FFFFFFFF)
            {
                case EXT_HealingStats:
                    switch (c.SrcAgent & 0x00FFFFFF00000000)
                    {
                        case 0:
                            return new HealingStatsRev0ExtensionHandler(c);
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return null;
        }

        // FOR TESTING PURPOSES
        internal static AbstractExtensionHandler GetExtensionHandler(uint sig)
        {
            // place holder for sig
            switch (sig)
            {
                case EXT_HealingStats:
                    return new HealingStatsRev0ExtensionHandler();
                default:
                    break;
            }
            return null;
        }
    }
}
