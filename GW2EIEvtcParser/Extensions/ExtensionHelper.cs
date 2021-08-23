using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.Extensions
{
    public static class ExtensionHelper
    {
        internal static AbstractExtensionHandler GetExtensionHandler(CombatItem c)
        {
            if (!c.IsExtension && c.Pad != 0)
            {
                return null;
            }
            // place holder for sig
            ulong sig = c.SrcAgent & 0x00000000FFFFFFFF;
            ulong rev = (c.SrcAgent & 0x00FFFFFF00000000) >> 32;
            switch (sig)
            {
                case HealingStatsExtensionHandler.EXT_HealingStats:
                    switch (rev)
                    {
                        case 1:
                            return new HealingStatsRev1ExtensionHandler(c);
                        case 2:
                            return new HealingStatsRev2ExtensionHandler(c);
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return null;
        }
    }
}
