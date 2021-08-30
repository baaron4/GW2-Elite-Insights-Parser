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
            uint sig = (uint)(c.SrcAgent & 0x00000000FFFFFFFF);
            uint rev = (uint)((c.SrcAgent & 0x00FFFFFF00000000) >> 32);
            switch (sig)
            {
                case HealingStatsExtensionHandler.EXT_HealingStats:
                    switch (rev)
                    {
                        case 1:
                        case 2:
                            return new HealingStatsExtensionHandler(c, rev);
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
