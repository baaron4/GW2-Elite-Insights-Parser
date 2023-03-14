using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.Extensions
{
    public static class ExtensionHelper
    {
        private const ulong SigMask = 0x00000000FFFFFFFF;
        private const ulong RevMask = 0x00FFFFFF00000000;
        private const byte RevShift = 32;

        internal static AbstractExtensionHandler GetExtensionHandler(CombatItem c)
        {
            if (c.IsStateChange != ArcDPSEnums.StateChange.Extension || c.Pad != 0)
            {
                return null;
            }
            uint sig = (uint)(c.SrcAgent & SigMask);
            uint rev = (uint)((c.SrcAgent & RevMask) >> RevShift);
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
